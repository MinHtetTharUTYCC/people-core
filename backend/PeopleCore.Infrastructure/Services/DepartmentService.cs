using Microsoft.EntityFrameworkCore;
using PeopleCore.Application.Common;
using PeopleCore.Application.Departments.DTOs;
using PeopleCore.Application.Departments.Services;
using PeopleCore.Domain.Departments;
using DbContext = PeopleCore.Infrastructure.Data.DbContext;

namespace PeopleCore.Infrastructure.Services;

public class DepartmentService(DbContext context) : IDepartmentService
{
    public async Task<ApiResponse<IEnumerable<DepartmentResponse>>> GetAllAsync()
    {
        var departments = await context.Departments
            .Include(d => d.ParentDepartment)
            .OrderBy(d => d.CreatedAt)
            .ToListAsync();

        var result = departments.Select(d => new DepartmentResponse
        {
            Id = d.Id,
            Name = d.Name,
            Code = d.Code,
            ParentDepartmentId = d.ParentDepartmentId,
            ParentDepartmentName = d.ParentDepartment?.Name,
            ManagerId = d.ManagerId,
            CreatedAt = d.CreatedAt
        });

        return ApiResponse<IEnumerable<DepartmentResponse>>.Ok(result);
    }

    public async Task<ApiResponse<DepartmentResponse>> GetByIdAsync(int id)
    {
        var department = await context.Departments
            .Include(d => d.ParentDepartment)
            .FirstOrDefaultAsync(d => d.Id == id);

        if (department == null)
            return ApiResponse<DepartmentResponse>.Error("Department not found");

        return ApiResponse<DepartmentResponse>.Ok(new DepartmentResponse
        {
            Id = department.Id,
            Name = department.Name,
            Code = department.Code,
            ParentDepartmentId = department.ParentDepartmentId,
            ParentDepartmentName = department.ParentDepartment?.Name,
            ManagerId = department.ManagerId,
            CreatedAt = department.CreatedAt
        });
    }

    public async Task<ApiResponse<DepartmentResponse>> CreateAsync(CreateDepartmentRequest request)
    {
        var existing = await context.Departments.AnyAsync(d => d.Code == request.Code);
        if (existing)
            return ApiResponse<DepartmentResponse>.Error("Department code already exists");

        var department = new Department
        {
            Name = request.Name,
            Code = request.Code,
            ParentDepartmentId = request.ParentDepartmentId,
            ManagerId = request.ManagerId
        };

        context.Departments.Add(department);
        await context.SaveChangesAsync();

        return ApiResponse<DepartmentResponse>.Ok(new DepartmentResponse
        {
            Id = department.Id,
            Name = department.Name,
            Code = department.Code,
            ParentDepartmentId = department.ParentDepartmentId,
            ManagerId = department.ManagerId,
            CreatedAt = department.CreatedAt
        }, "Department created successfully");
    }

    public async Task<ApiResponse<DepartmentResponse>> UpdateAsync(int id, UpdateDepartmentRequest request)
    {
        var department = await context.Departments.FindAsync(id);
        if (department == null)
            return ApiResponse<DepartmentResponse>.Error("Department not found");

        var existing = await context.Departments.AnyAsync(d => d.Code == request.Code && d.Id != id);
        if (existing)
            return ApiResponse<DepartmentResponse>.Error("Department code already exists");

        department.Name = request.Name;
        department.Code = request.Code;
        department.ParentDepartmentId = request.ParentDepartmentId;
        department.ManagerId = request.ManagerId;
        department.UpdatedAt = DateTime.UtcNow;

        await context.SaveChangesAsync();

        return ApiResponse<DepartmentResponse>.Ok(new DepartmentResponse
        {
            Id = department.Id,
            Name = department.Name,
            Code = department.Code,
            ParentDepartmentId = department.ParentDepartmentId,
            ManagerId = department.ManagerId,
            CreatedAt = department.CreatedAt
        }, "Department updated successfully");
    }

    public async Task<ApiResponse<bool>> DeleteAsync(int id)
    {
        var department = await context.Departments.FindAsync(id);
        if (department == null)
            return ApiResponse<bool>.Error("Department not found");

        context.Departments.Remove(department);
        await context.SaveChangesAsync();

        return ApiResponse<bool>.Ok(true, "Department deleted successfully");
    }

    public async Task<ApiResponse<IEnumerable<PositionResponse>>> GetAllPositionsAsync()
    {
        var positions = await context.Positions
            .Include(p => p.Department)
            .OrderBy(p => p.CreatedAt)
            .ToListAsync();

        var result = positions.Select(p => new PositionResponse
        {
            Id = p.Id,
            Title = p.Title,
            Description = p.Description,
            DepartmentId = p.DepartmentId,
            DepartmentName = p.Department.Name,
            SalaryGrade = p.SalaryGrade,
            CreatedAt = p.CreatedAt
        });

        return ApiResponse<IEnumerable<PositionResponse>>.Ok(result);
    }

    public async Task<ApiResponse<PositionResponse>> GetPositionByIdAsync(int id)
    {
        var position = await context.Positions
            .Include(p => p.Department)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (position == null)
            return ApiResponse<PositionResponse>.Error("Position not found");

        return ApiResponse<PositionResponse>.Ok(new PositionResponse
        {
            Id = position.Id,
            Title = position.Title,
            Description = position.Description,
            DepartmentId = position.DepartmentId,
            DepartmentName = position.Department.Name,
            SalaryGrade = position.SalaryGrade,
            CreatedAt = position.CreatedAt
        });
    }

    public async Task<ApiResponse<PositionResponse>> CreatePositionAsync(CreatePositionRequest request)
    {
        var department = await context.Departments.FindAsync(request.DepartmentId);
        if (department == null)
            return ApiResponse<PositionResponse>.Error("Department not found");

        var position = new Position
        {
            Title = request.Title,
            Description = request.Description,
            DepartmentId = request.DepartmentId,
            SalaryGrade = request.SalaryGrade
        };

        context.Positions.Add(position);
        await context.SaveChangesAsync();

        return ApiResponse<PositionResponse>.Ok(new PositionResponse
        {
            Id = position.Id,
            Title = position.Title,
            Description = position.Description,
            DepartmentId = position.DepartmentId,
            DepartmentName = department.Name,
            SalaryGrade = position.SalaryGrade,
            CreatedAt = position.CreatedAt
        }, "Position created successfully");
    }

    public async Task<ApiResponse<PositionResponse>> UpdatePositionAsync(int id, UpdatePositionRequest request)
    {
        var position = await context.Positions.FindAsync(id);
        if (position == null)
            return ApiResponse<PositionResponse>.Error("Position not found");

        var department = await context.Departments.FindAsync(request.DepartmentId);
        if (department == null)
            return ApiResponse<PositionResponse>.Error("Department not found");

        position.Title = request.Title;
        position.Description = request.Description;
        position.DepartmentId = request.DepartmentId;
        position.SalaryGrade = request.SalaryGrade;
        position.UpdatedAt = DateTime.UtcNow;

        await context.SaveChangesAsync();

        return ApiResponse<PositionResponse>.Ok(new PositionResponse
        {
            Id = position.Id,
            Title = position.Title,
            Description = position.Description,
            DepartmentId = position.DepartmentId,
            DepartmentName = department.Name,
            SalaryGrade = position.SalaryGrade,
            CreatedAt = position.CreatedAt
        }, "Position updated successfully");
    }

    public async Task<ApiResponse<bool>> DeletePositionAsync(int id)
    {
        var position = await context.Positions.FindAsync(id);
        if (position == null)
            return ApiResponse<bool>.Error("Position not found");

        context.Positions.Remove(position);
        await context.SaveChangesAsync();

        return ApiResponse<bool>.Ok(true, "Position deleted successfully");
    }
}
