using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;
using DbContext = PeopleCore.Infrastructure.Data.DbContext;

namespace PeopleCore.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController(DbContext context, IConfiguration configuration) : ControllerBase
{
    [HttpGet("")]
    public IActionResult GetHealth()
    {
        return Ok(new
        {
            status = "Healthy",
            timestamp = DateTime.UtcNow
        });
    }

    [HttpGet("db")]
    public async Task<IActionResult> GetDatabaseHealth()
    {
        try
        {
            await context.Database.CanConnectAsync();
            return Ok(new
            {
                status = "Database Healthy",
                timestamp = DateTime.UtcNow
            });
        }
        catch (Exception)
        {
            return StatusCode(500, new
            {
                status = "Unhealthy",
                timestamp = DateTime.UtcNow
            });
        }
    }
    
    [HttpGet("auth")]
    public IActionResult GetAuthHealth()
    {
        var jwt = configuration.GetSection("Jwt");
        var secretKey = jwt["SecretKey"];
        var issuer = jwt["Issuer"];
        var audience = jwt["Audience"];

        var secretConfigured = !string.IsNullOrWhiteSpace(secretKey)
            && !secretKey.Contains("CHANGE_ME", StringComparison.OrdinalIgnoreCase)
            && Encoding.UTF8.GetByteCount(secretKey) >= 32;
        var issuerConfigured = !string.IsNullOrWhiteSpace(issuer);
        var audienceConfigured = !string.IsNullOrWhiteSpace(audience);
        var healthy = secretConfigured && issuerConfigured && audienceConfigured;

        var response = new
        {
            status = healthy ? "Auth Healthy" : "Auth Unhealthy",
            timestamp = DateTime.UtcNow,
            secretKeyConfigured = secretConfigured,
            issuerConfigured,
            audienceConfigured
        };

        return healthy ? Ok(response) : StatusCode(503, response);
    }

   
}