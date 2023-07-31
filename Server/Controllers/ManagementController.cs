using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Data.Helpers;

namespace Server.Controllers;

[Authorize(Roles = UserRoles.Manager)]
[Route("api/[controller]")]
[ApiController]
public class ManagementController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok("Welcome to ManagementController");
    }
}