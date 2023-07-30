using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Server.Data;
using Server.Data.Models;
using Server.Data.ViewModels;

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
            return Ok("User signed in.");

        return Unauthorized();
    }
}