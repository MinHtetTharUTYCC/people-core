using PeopleCore.Application.Common;
using PeopleCore.Application.Employees.DTOs;

namespace PeopleCore.Application.Employees.Services;

public interface IEmployeeService
{
    Task<ApiResponse<PagedResult<EmployeeResponse>>> GetAllAsync(int page, int pageSize, string? search, int? departmentId);
    Task<ApiResponse<EmployeeResponse>> GetByIdAsync(int id);
    Task<ApiResponse<EmployeeResponse>> CreateAsync(CreateEmployeeRequest request);
    Task<ApiResponse<EmployeeResponse>> UpdateAsync(int id, UpdateEmployeeRequest request);
    Task<ApiResponse<bool>> DeleteAsync(int id);
}
