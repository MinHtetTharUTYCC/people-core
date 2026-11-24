using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PeopleCore.Application.Leave.DTOs;
using PeopleCore.Application.Leave.Services;
using PeopleCore.Application.Identity.Services;

namespace PeopleCore.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "EmployeeOnly")]
public class LeaveController(
    ILeaveService leaveService,
    ICurrentUserService currentUser) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Submit([FromBody] CreateLeaveRequest request)
    {
        if (!User.IsInRole("HR") && !User.IsInRole("Admin"))
        {
            var employeeId = currentUser.EmployeeId;
            if (employeeId == null)
                return Unauthorized(new { success = false, message = "Employee not found for current user" });

            request.EmployeeId = employeeId.Value;
        }

        var result = await leaveService.SubmitAsync(request);
        return result.Success ? CreatedAtAction(nameof(GetByEmployee), new { employeeId = request.EmployeeId }, result) : BadRequest(result);
    }

    [HttpPost("{id}/approve")]
    [Authorize(Policy = "HROnly")]
    public async Task<IActionResult> Approve(int id)
    {
        var approvedBy = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "";
        var result = await leaveService.ApproveAsync(id, approvedBy);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("{id}/reject")]
    [Authorize(Policy = "HROnly")]
    public async Task<IActionResult> Reject(int id)
    {
        var approvedBy = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "";
        var result = await leaveService.RejectAsync(id, approvedBy);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("employee/{employeeId}")]
    public async Task<IActionResult> GetByEmployee(int employeeId)
    {
        if (User.IsInRole("HR") == false && User.IsInRole("Admin") == false)
        {
            var currentEmployeeId = currentUser.EmployeeId;
            if (currentEmployeeId == null || currentEmployeeId.Value != employeeId)
            {
                return Forbid();
            }
        }
        return Ok(await leaveService.GetByEmployeeAsync(employeeId));
    }

    [HttpGet("pending")]
    [Authorize(Policy = "HROnly")]
    public async Task<IActionResult> GetPending() => Ok(await leaveService.GetPendingAsync());

    [HttpGet("balance/{employeeId}")]
    public async Task<IActionResult> GetBalance(int employeeId)
    {
        if (User.IsInRole("HR") == false && User.IsInRole("Admin") == false)
        {
            var currentEmployeeId = currentUser.EmployeeId;
            if (currentEmployeeId == null || currentEmployeeId.Value != employeeId)
            {
                return Forbid();
            }
        }
        return Ok(await leaveService.GetBalanceAsync(employeeId));
    }

}
