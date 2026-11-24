using Microsoft.AspNetCore.Mvc;

namespace PeopleCore.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AppController : ControllerBase
{
    [HttpGet()]
    public IActionResult Get()
    {
        return Ok(new
        {
            message = "Welcome to PeopleCore API",
            version = "1.0.0",
            timestamp = DateTime.UtcNow
        });
    }
}