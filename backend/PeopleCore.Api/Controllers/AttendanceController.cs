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
    public async Task<IActionResult> ClockIn([FromQuery] int employeeId)
    {
        var result = await service.ClockInAsync(employeeId);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("clock-out")]
    public async Task<IActionResult> ClockOut([FromQuery] int employeeId)
    {
        var result = await service.ClockOutAsync(employeeId);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] int employeeId, [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        => Ok(await service.GetByEmployeeAsync(employeeId, startDate, endDate));
}
