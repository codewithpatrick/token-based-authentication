using Microsoft.AspNetCore.Mvc;

namespace Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class HomeController : ControllerBase
{
    public HomeController()
    {

    }

    [HttpGet]
    public IActionResult Get()
    {
        return Ok("Welcome to HomeController");
    }
}