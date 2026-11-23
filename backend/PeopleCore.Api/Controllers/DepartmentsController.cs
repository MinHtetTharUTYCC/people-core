using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PeopleCore.Application.Departments.DTOs;
using PeopleCore.Application.Departments.Services;

namespace PeopleCore.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "HROnly")]
public class DepartmentsController(IDepartmentService departmentService) : ControllerBase
{

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await departmentService.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id) => Ok(await departmentService.GetByIdAsync(id));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateDepartmentRequest request)
    {
        var result = await departmentService.CreateAsync(request);
        return result.Success ? CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result) : BadRequest(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateDepartmentRequest request)
    {
        var result = await departmentService.UpdateAsync(id, request);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id) => Ok(await departmentService.DeleteAsync(id));
}

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "HROnly")]
public class PositionsController : ControllerBase
{
    private readonly IDepartmentService _departmentService;

    public PositionsController(IDepartmentService departmentService)
    {
        _departmentService = departmentService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _departmentService.GetAllPositionsAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id) => Ok(await _departmentService.GetPositionByIdAsync(id));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePositionRequest request)
    {
        var result = await _departmentService.CreatePositionAsync(request);
        return result.Success ? CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result) : BadRequest(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdatePositionRequest request)
    {
        var result = await _departmentService.UpdatePositionAsync(id, request);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id) => Ok(await _departmentService.DeletePositionAsync(id));
}
