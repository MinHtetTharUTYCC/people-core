using Microsoft.EntityFrameworkCore;
using PeopleCore.Application.Common;
using PeopleCore.Application.Employees.DTOs;
using PeopleCore.Application.Employees.Services;
using PeopleCore.Domain.Employees;
using PeopleCore.Infrastructure.Data;
using DbContext = PeopleCore.Infrastructure.Data.DbContext;

namespace PeopleCore.Infrastructure.Services;

public class EmployeeService(DbContext context) : IEmployeeService
{
    public async Task<ApiResponse<PagedResult<EmployeeResponse>>> GetAllAsync(int page, int pageSize, string? search, int? departmentId)
    {
        var query = context.Employees
            .Include(e => e.Department)
            .Include(e => e.Position)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.ToLowerInvariant();
            query = query.Where(e => e.FirstName.ToLower().Contains(s) || e.LastName.ToLower().Contains(s) || e.Email.ToLower().Contains(s));
        }

        if (departmentId.HasValue)
            query = query.Where(e => e.DepartmentId == departmentId.Value);

        var totalCount = await query.CountAsync();
        var employees = await query
            .OrderBy(e => e.LastName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var items = employees.Select(MapResponse);

        return ApiResponse<PagedResult<EmployeeResponse>>.Ok(new PagedResult<EmployeeResponse>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        });
    }

    public async Task<ApiResponse<EmployeeResponse>> GetByIdAsync(int id)
    {
        var employee = await context.Employees
            .Include(e => e.Department)
            .Include(e => e.Position)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (employee == null) return ApiResponse<EmployeeResponse>.Error("Employee not found");
        return ApiResponse<EmployeeResponse>.Ok(MapResponse(employee));
    }

    public async Task<ApiResponse<EmployeeResponse>> CreateAsync(CreateEmployeeRequest request)
    {
        if (await context.Employees.AnyAsync(e => e.Email == request.Email))
            return ApiResponse<EmployeeResponse>.Error("Email already registered");

        if (!await context.Departments.AnyAsync(d => d.Id == request.DepartmentId))
            return ApiResponse<EmployeeResponse>.Error("Department not found");

        if (!await context.Positions.AnyAsync(p => p.Id == request.PositionId))
            return ApiResponse<EmployeeResponse>.Error("Position not found");

        var employee = new Employee
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            DateOfBirth = request.DateOfBirth,
            HireDate = request.HireDate,
            DepartmentId = request.DepartmentId,
            PositionId = request.PositionId
        };

        context.Employees.Add(employee);
        await context.SaveChangesAsync();

        var result = await GetByIdAsync(employee.Id);
        result.Message = "Employee created successfully";
        return result;
    }

    public async Task<ApiResponse<EmployeeResponse>> UpdateAsync(int id, UpdateEmployeeRequest request)
    {
        var employee = await context.Employees.FindAsync(id);
        if (employee == null) return ApiResponse<EmployeeResponse>.Error("Employee not found");

        employee.FirstName = request.FirstName;
        employee.LastName = request.LastName;
        employee.Email = request.Email;
        employee.PhoneNumber = request.PhoneNumber;
        employee.DateOfBirth = request.DateOfBirth;
        employee.HireDate = request.HireDate;
        employee.DepartmentId = request.DepartmentId;
        employee.PositionId = request.PositionId;
        if (request.Status != null) employee.Status = (EmployeeStatus)Enum.Parse(typeof(EmployeeStatus), request.Status);
        employee.UpdatedAt = DateTime.UtcNow;

        await context.SaveChangesAsync();
        var result = await GetByIdAsync(id);
        result.Message = "Employee updated successfully";
        return result;
    }

    public async Task<ApiResponse<bool>> DeleteAsync(int id)
    {
        var employee = await context.Employees.FindAsync(id);
        if (employee == null) return ApiResponse<bool>.Error("Employee not found");
        if (employee.Status == EmployeeStatus.Active)
            return ApiResponse<bool>.Error("Cannot delete active employee. Deactivate first.");

        context.Employees.Remove(employee);
        await context.SaveChangesAsync();
        return ApiResponse<bool>.Ok(true, "Employee deleted successfully");
    }

    private static EmployeeResponse MapResponse(Employee e) => new()
    {
        Id = e.Id,
        FirstName = e.FirstName,
        LastName = e.LastName,
        Email = e.Email,
        PhoneNumber = e.PhoneNumber,
        DateOfBirth = e.DateOfBirth,
        HireDate = e.HireDate,
        DepartmentId = e.DepartmentId,
        DepartmentName = e.Department.Name,
        PositionId = e.PositionId,
        PositionTitle = e.Position.Title,
        Status = e.Status.ToString()
    };
}
