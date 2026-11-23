using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PeopleCore.Application.Employees.DTOs;
using PeopleCore.Application.Employees.Services;

namespace PeopleCore.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "EmployeeOnly")]
public class EmployeesController(IEmployeeService employeeService) : ControllerBase
{
    [HttpGet]
    [Authorize(Policy = "HROnly")]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] string? search = null, [FromQuery] int? departmentId = null)
        => Ok(await employeeService.GetAllAsync(page, pageSize, search, departmentId));

    [HttpGet("{id}")]
    [Authorize(Policy = "HROnly")]
    public async Task<IActionResult> GetById(int id) => Ok(await employeeService.GetByIdAsync(id));

    [HttpPost]
    [Authorize(Policy = "HROnly")]
    public async Task<IActionResult> Create([FromBody] CreateEmployeeRequest request)
    {
        var result = await employeeService.CreateAsync(request);
        return result.Success ? CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result) : BadRequest(result);
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "HROnly")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateEmployeeRequest request)
    {
        var result = await employeeService.UpdateAsync(id, request);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "HROnly")]
    public async Task<IActionResult> Delete(int id) => Ok(await employeeService.DeleteAsync(id));
}
