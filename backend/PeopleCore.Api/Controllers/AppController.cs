using Microsoft.AspNetCore.Mvc;

namespace PeopleCore.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AppController : ControllerBase
{
    [HttpGet("")]
    public IActionResult Get()
    {
        return Ok(new
        {
            message = "Welcome to PeopleCore API",
            version = "1.0.0",
            endpoints = new[]
            {
                new { name = "Auth", path = "/api/auth", methods = "POST /api/auth/login, POST /api/auth/register" },
                new { name = "Departments", path = "/api/departments", methods = "GET, POST, PUT, DELETE (HR/Admin only)" },
                new { name = "Employees", path = "/api/employees", methods = "GET, POST, PUT, DELETE (HR/Admin/Employee)" },
                new { name = "Leave", path = "/api/leave", methods = "GET, POST, PUT (HR/Admin/Employee)" },
                new { name = "Attendance", path = "/api/attendance", methods = "GET, POST (HR/Admin/Employee)" },
                new { name = "Payroll", path = "/api/payroll", methods = "GET, POST (HR/Admin only)" }
            }
        });
    }
}