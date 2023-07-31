using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Data.Helpers;

namespace Server.Controllers;

[Authorize(Roles = UserRoles.Student)]
[Route("api/[controller]")]
[ApiController]
public class StudentController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok("Welcome to StudentController");
    }
}