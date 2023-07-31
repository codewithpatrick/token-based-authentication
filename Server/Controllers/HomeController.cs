using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Data.Helpers;

namespace Server.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = $"{UserRoles.Student}, {UserRoles.Manager}")]
public class HomeController : ControllerBase
{
    public HomeController()
    {

    }

    [HttpGet("student")]
    public IActionResult GetStudent()
    {
        return Ok("Welcome to HomeController - Student");
    }
    
    [HttpGet("manager")]
    public IActionResult GetManager()
    {
        return Ok("Welcome to HomeController - Manager");
    }
}