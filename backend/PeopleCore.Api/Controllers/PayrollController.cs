using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PeopleCore.Application.Payroll.DTOs;
using PeopleCore.Application.Payroll.Services;

namespace PeopleCore.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "HROnly")]
public class PayrollController(IPayrollService payrollService) : ControllerBase
{
    [HttpGet("salary-components/{employeeId}")]
    public async Task<IActionResult> GetSalaryComponents(int employeeId) => Ok(await payrollService.GetSalaryComponentsAsync(employeeId));

    [HttpPost("salary-components")]
    public async Task<IActionResult> AddSalaryComponent([FromBody] CreateSalaryComponentRequest request)
    {
        var result = await payrollService.AddSalaryComponentAsync(request);
        return result.Success ? CreatedAtAction(nameof(GetSalaryComponents), new { employeeId = request.EmployeeId }, result) : BadRequest(result);
    }

    [HttpDelete("salary-components/{id}")]
    public async Task<IActionResult> DeleteSalaryComponent(int id) => Ok(await payrollService.DeleteSalaryComponentAsync(id));

    [HttpGet("pay-runs/{employeeId}")]
    public async Task<IActionResult> GetPayRuns(int employeeId) => Ok(await payrollService.GetPayRunsAsync(employeeId));

    [HttpPost("pay-runs/generate")]
    public async Task<IActionResult> GeneratePayRun([FromBody] GeneratePayRunRequest request)
    {
        var result = await payrollService.GeneratePayRunAsync(request);
        return result.Success ? CreatedAtAction(nameof(GetPayRuns), new { employeeId = request.EmployeeId }, result) : BadRequest(result);
    }

    [HttpPost("pay-runs/{id}/process")]
    public async Task<IActionResult> ProcessPayRun(int id)
    {
        var result = await payrollService.ProcessPayRunAsync(id);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}
