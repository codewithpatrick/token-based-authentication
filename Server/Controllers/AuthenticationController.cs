using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Server.Data;
using Server.Data.Models;
using Server.Data.ViewModels;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthenticationController : ControllerBase
{
    private readonly IConfiguration _configuration;

    private readonly AppDbContext _context;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public AuthenticationController(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        AppDbContext context,
        IConfiguration configuration)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _context = context;
        _configuration = configuration;
    }

    [HttpPost("register-user")]
    public async Task<IActionResult> Register([FromBody] RegisterVM registerVm)
    {
        if (!ModelState.IsValid) return BadRequest("Please, provide all the required fields.");

        var userExists = await _userManager.FindByEmailAsync(registerVm.EmailAddress);

        if (userExists != null) return BadRequest($"user {registerVm.EmailAddress} already exists");

        var newUser = new ApplicationUser
        {
            FirstName = registerVm.FirstName,
            LastName = registerVm.LastName,
            Email = registerVm.EmailAddress,
            UserName = registerVm.UserName,
            SecurityStamp = Guid.NewGuid().ToString()
        };

        var result = await _userManager.CreateAsync(newUser, registerVm.Password);

        if (result.Succeeded) return Ok("User created");

        return BadRequest("User could not be created");
    }

    [HttpPost("login-user")]
    public async Task<IActionResult> Login([FromBody] LoginVm loginVm)
    {
        if (!ModelState.IsValid) return BadRequest("Please, provide all required fields.");

        var userExists = await _userManager.FindByEmailAsync(loginVm.EmailAddress);

        if (userExists != null && await _userManager.CheckPasswordAsync(userExists, loginVm.Password))
        {
            var tokenValue = await GenerateJWTTokenAsync(userExists);

            return Ok(tokenValue);
        }

        return Unauthorized();
    }

    private async Task<AuthResultVM> GenerateJWTTokenAsync(ApplicationUser user)
    {
        var authClaims = new List<Claim>
        {
            new(ClaimTypes.Name, user.UserName),
            new(ClaimTypes.NameIdentifier, user.Id),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(JwtRegisteredClaimNames.Sub, user.Email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var authSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["JWT:Secret"]));

        var token = new JwtSecurityToken(
            _configuration["JWT:Issuer"],
            _configuration["JWT:Audience"],
            expires: DateTime.UtcNow.AddMinutes(1),
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256));

        var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);

        var refreshToken = new RefreshToken
        {
            Token = Guid.NewGuid() + "-" + Guid.NewGuid(),
            JwtId = token.Id,
            IsRevoked = false,
            DateAdded = DateTime.UtcNow,
            DateExpire = DateTime.UtcNow.AddMonths(6),
            UserId = user.Id
        };

        await _context.RefreshTokens.AddRangeAsync(refreshToken);
        await _context.SaveChangesAsync();

        var response = new AuthResultVM
        {
            Token = jwtToken,
            RefreshToken = refreshToken.Token,
            ExpiresAt = token.ValidTo
        };

        return response;
    }
}