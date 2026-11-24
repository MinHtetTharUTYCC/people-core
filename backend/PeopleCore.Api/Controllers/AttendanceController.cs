using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PeopleCore.Application.Attendance.Services;
using PeopleCore.Application.Identity.Services;

namespace PeopleCore.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "EmployeeOnly")]
public class AttendanceController(IAttendanceService service, ICurrentUserService currentUser) : ControllerBase
{
    [HttpPost("clock-in")]
    public async Task<IActionResult> ClockIn()
    {
        if (currentUser.EmployeeId is not int employeeId)
        {
            return Unauthorized(new { success = false, message = "Employee not found for current user" });
        }

        var result = await service.ClockInAsync(employeeId);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("clock-out")]
    public async Task<IActionResult> ClockOut()
    {
        if (currentUser.EmployeeId is not int employeeId)
        {
            return Unauthorized(new { success = false, message = "Employee not found for current user" });
        }

        var result = await service.ClockOutAsync(employeeId);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
    {
        var employeeId = currentUser.EmployeeId;
        if (employeeId == null)
        {
            return Unauthorized(new { success = false, message = "Employee not found for current user" });
        }

        return Ok(await service.GetByEmployeeAsync(employeeId.Value, startDate, endDate));
    }
}
