using Microsoft.EntityFrameworkCore;
using PeopleCore.Application.Common;
using PeopleCore.Application.Payroll.DTOs;
using PeopleCore.Application.Payroll.Services;
using PeopleCore.Domain.Payroll;
using PeopleCore.Infrastructure.Data;
using DbContext = PeopleCore.Infrastructure.Data.DbContext;

namespace PeopleCore.Infrastructure.Services;

public class PayrollService(DbContext context) : IPayrollService
{
    public async Task<ApiResponse<IEnumerable<SalaryComponentResponse>>> GetSalaryComponentsAsync(int employeeId)
    {
        var components = await context.SalaryComponents
            .Where(sc => sc.EmployeeId == employeeId)
            .ToListAsync();
        return ApiResponse<IEnumerable<SalaryComponentResponse>>.Ok(components.Select(MapSalaryComponent));
    }

    public async Task<ApiResponse<SalaryComponentResponse>> AddSalaryComponentAsync(CreateSalaryComponentRequest request)
    {
        if (!await context.Employees.AnyAsync(e => e.Id == request.EmployeeId))
            return ApiResponse<SalaryComponentResponse>.Error("Employee not found");

        if (!Enum.TryParse<SalaryType>(request.Type, ignoreCase: true, out var type))
        {
            return ApiResponse<SalaryComponentResponse>.Error("Invalid salary type");
        }

        var component = new SalaryComponent
        {
            EmployeeId = request.EmployeeId,
            Type = type,
            Name = request.Name,
            Amount = request.Amount,
            IsFixed = request.IsFixed
        };

        context.SalaryComponents.Add(component);
        await context.SaveChangesAsync();
        return ApiResponse<SalaryComponentResponse>.Ok(MapSalaryComponent(component), "Salary component added");
    }

    public async Task<ApiResponse<bool>> DeleteSalaryComponentAsync(int id)
    {
        var component = await context.SalaryComponents.FindAsync(id);
        if (component == null) return ApiResponse<bool>.Error("Salary component not found");

        context.SalaryComponents.Remove(component);
        await context.SaveChangesAsync();
        return ApiResponse<bool>.Ok(true, "Salary component deleted");
    }

    public async Task<ApiResponse<IEnumerable<PayRunResponse>>> GetPayRunsAsync(int employeeId)
    {
        var payRuns = await context.PayRuns
            .Where(pr => pr.EmployeeId == employeeId)
            .OrderByDescending(pr => pr.CreatedAt)
            .ToListAsync();
        return ApiResponse<IEnumerable<PayRunResponse>>.Ok(payRuns.Select(MapPayRun));
    }

    public async Task<ApiResponse<PayRunResponse>> GeneratePayRunAsync(GeneratePayRunRequest request)
    {
        if (!await context.Employees.AnyAsync(e => e.Id == request.EmployeeId))
            return ApiResponse<PayRunResponse>.Error("Employee not found");

        var components = await context.SalaryComponents
            .Where(sc => sc.EmployeeId == request.EmployeeId)
            .ToListAsync();

        if (!components.Any())
            return ApiResponse<PayRunResponse>.Error("No salary components found");

        var grossPay = components.Where(c => c.Type == SalaryType.Basic || c.Type == SalaryType.Allowance)
            .Sum(c => c.Amount);
        var deductions = components.Where(c => c.Type == SalaryType.Deduction).Sum(c => c.Amount);

        var payRun = new PayRun
        {
            EmployeeId = request.EmployeeId,
            PeriodStart = request.PeriodStart,
            PeriodEnd = request.PeriodEnd,
            GrossPay = grossPay,
            TotalDeductions = deductions,
            NetPay = grossPay - deductions,
            Status = PayRunStatus.Draft
        };

        context.PayRuns.Add(payRun);
        await context.SaveChangesAsync();
        return ApiResponse<PayRunResponse>.Ok(MapPayRun(payRun), "Pay run generated");
    }

    public async Task<ApiResponse<PayRunResponse>> ProcessPayRunAsync(int id)
    {
        var payRun = await context.PayRuns.FindAsync(id);
        if (payRun == null) return ApiResponse<PayRunResponse>.Error("Pay run not found");
        if (payRun.Status != PayRunStatus.Draft)
            return ApiResponse<PayRunResponse>.Error("Pay run is not in draft status");

        payRun.Status = PayRunStatus.Processed;
        payRun.ProcessedAt = DateTime.UtcNow;
        await context.SaveChangesAsync();
        return ApiResponse<PayRunResponse>.Ok(MapPayRun(payRun), "Pay run processed");
    }

    private static SalaryComponentResponse MapSalaryComponent(SalaryComponent sc) => new()
    {
        Id = sc.Id, EmployeeId = sc.EmployeeId, Type = sc.Type.ToString(),
        Name = sc.Name, Amount = sc.Amount, IsFixed = sc.IsFixed
    };

    private static PayRunResponse MapPayRun(PayRun pr) => new()
    {
        Id = pr.Id, EmployeeId = pr.EmployeeId, PeriodStart = pr.PeriodStart,
        PeriodEnd = pr.PeriodEnd, GrossPay = pr.GrossPay, TotalDeductions = pr.TotalDeductions,
        NetPay = pr.NetPay, Status = pr.Status.ToString(), ProcessedAt = pr.ProcessedAt
    };
}
