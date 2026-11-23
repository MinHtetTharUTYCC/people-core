using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PeopleCore.Application.Attendance.Services;

namespace PeopleCore.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "EmployeeOnly")]
public class AttendanceController(IAttendanceService service) : ControllerBase
{
    [HttpPost("clock-in")]
    public async Task<IActionResult> ClockIn()
    {
        var employeeId = await GetEmployeeIdFromTokenAsync();
        if (employeeId == null)
        {
            return Unauthorized(new { success = false, message = "Employee not found for current user" });
        }

        var result = await service.ClockInAsync(employeeId.Value);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("clock-out")]
    public async Task<IActionResult> ClockOut()
    {
        var employeeId = await GetEmployeeIdFromTokenAsync();
        if (employeeId == null)
        {
            return Unauthorized(new { success = false, message = "Employee not found for current user" });
        }

        var result = await service.ClockOutAsync(employeeId.Value);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
    {
        var employeeId = await GetEmployeeIdFromTokenAsync();
        if (employeeId == null)
        {
            return Unauthorized(new { success = false, message = "Employee not found for current user" });
        }

        // HR and Admin can optionally query by employeeId
        if (User.IsInRole("HR") || User.IsInRole("Admin"))
        {
            return Ok(await service.GetByEmployeeAsync(employeeId.Value, startDate, endDate));
        }

        return Ok(await service.GetByEmployeeAsync(employeeId.Value, startDate, endDate));
    }

    private async Task<int?> GetEmployeeIdFromTokenAsync()
    {
        var currentUserEmail = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
        if (string.IsNullOrEmpty(currentUserEmail))
        {
            return null;
        }
        return await service.GetEmployeeByEmailAsync(currentUserEmail);
    }
}
