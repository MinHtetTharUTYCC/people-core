using Microsoft.EntityFrameworkCore;
using PeopleCore.Application.Common;
using PeopleCore.Application.Leave.DTOs;
using PeopleCore.Application.Leave.Services;
using PeopleCore.Domain.Leave;
using PeopleCore.Infrastructure.Data;
using DbContext = PeopleCore.Infrastructure.Data.DbContext;

namespace PeopleCore.Infrastructure.Services;

public class LeaveService(DbContext context) : ILeaveService
{
    public async Task<ApiResponse<LeaveRequestResponse>> SubmitAsync(CreateLeaveRequest request)
    {
        if (request.StartDate > request.EndDate)
            return ApiResponse<LeaveRequestResponse>.Error("Start date must be before end date");

        if (!await context.Employees.AnyAsync(e => e.Id == request.EmployeeId))
            return ApiResponse<LeaveRequestResponse>.Error("Employee not found");

        if (!await context.LeaveTypes.AnyAsync(l => l.Id == request.LeaveTypeId))
            return ApiResponse<LeaveRequestResponse>.Error("Leave type not found");

        var overlaps = await context.LeaveRequests.AnyAsync(lr =>
            lr.EmployeeId == request.EmployeeId && lr.Status == LeaveStatus.Pending &&
            ((lr.StartDate <= request.EndDate && lr.EndDate >= request.StartDate)));
        if (overlaps)
            return ApiResponse<LeaveRequestResponse>.Error("Overlapping leave request exists");

        var leave = new LeaveRequest
        {
            EmployeeId = request.EmployeeId,
            LeaveTypeId = request.LeaveTypeId,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Reason = request.Reason,
            Status = LeaveStatus.Pending
        };

        context.LeaveRequests.Add(leave);
        await context.SaveChangesAsync();

        return ApiResponse<LeaveRequestResponse>.Ok(MapResponse(leave), "Leave request submitted");
    }

    public async Task<ApiResponse<bool>> ApproveAsync(int id, string approvedBy)
    {
        var leave = await context.LeaveRequests.FindAsync(id);
        if (leave == null) return ApiResponse<bool>.Error("Leave request not found");
        if (leave.Status != LeaveStatus.Pending) return ApiResponse<bool>.Error("Leave request is not pending");

        leave.Status = LeaveStatus.Approved;
        leave.ApprovedBy = approvedBy;
        leave.ApprovedAt = DateTime.UtcNow;
        await context.SaveChangesAsync();
        return ApiResponse<bool>.Ok(true, "Leave request approved");
    }

    public async Task<ApiResponse<bool>> RejectAsync(int id, string approvedBy)
    {
        var leave = await context.LeaveRequests.FindAsync(id);
        if (leave == null) return ApiResponse<bool>.Error("Leave request not found");
        if (leave.Status != LeaveStatus.Pending) return ApiResponse<bool>.Error("Leave request is not pending");

        leave.Status = LeaveStatus.Rejected;
        leave.ApprovedBy = approvedBy;
        leave.ApprovedAt = DateTime.UtcNow;
        await context.SaveChangesAsync();
        return ApiResponse<bool>.Ok(true, "Leave request rejected");
    }

    public async Task<ApiResponse<IEnumerable<LeaveRequestResponse>>> GetByEmployeeAsync(int employeeId)
    {
        var requests = await context.LeaveRequests
            .Include(lr => lr.Employee)
            .Include(lr => lr.LeaveType)
            .Where(lr => lr.EmployeeId == employeeId)
            .OrderByDescending(lr => lr.CreatedAt)
            .ToListAsync();
        return ApiResponse<IEnumerable<LeaveRequestResponse>>.Ok(requests.Select(MapResponse));
    }

    public async Task<ApiResponse<IEnumerable<LeaveRequestResponse>>> GetPendingAsync()
    {
        var requests = await context.LeaveRequests
            .Include(lr => lr.Employee)
            .Include(lr => lr.LeaveType)
            .Where(lr => lr.Status == LeaveStatus.Pending)
            .OrderByDescending(lr => lr.CreatedAt)
            .ToListAsync();
        return ApiResponse<IEnumerable<LeaveRequestResponse>>.Ok(requests.Select(MapResponse));
    }

    public async Task<ApiResponse<LeaveBalanceResponse>> GetBalanceAsync(int employeeId)
    {
        var leaveTypes = await context.LeaveTypes.ToListAsync();
        var usedDays = await context.LeaveRequests
            .Where(lr => lr.EmployeeId == employeeId && lr.Status == LeaveStatus.Approved)
            .GroupBy(lr => lr.LeaveTypeId)
            .ToDictionaryAsync(g => g.Key, g => g.Sum(lr => (lr.EndDate - lr.StartDate).Days + 1));

        var balance = leaveTypes.Select(lt => new LeaveBalanceResponse
        {
            EmployeeId = employeeId,
            LeaveTypeId = lt.Id,
            LeaveTypeName = lt.Name,
            DefaultDays = lt.DefaultDays,
            UsedDays = usedDays.GetValueOrDefault(lt.Id, 0),
            RemainingDays = lt.DefaultDays - (usedDays.GetValueOrDefault(lt.Id, 0))
        }).FirstOrDefault();

        if (balance == null) return ApiResponse<LeaveBalanceResponse>.Error("No leave types configured");
        return ApiResponse<LeaveBalanceResponse>.Ok(balance);
    }

    public async Task<int?> GetEmployeeByEmailAsync(string email)
    {
        var employee = await context.Employees.FirstOrDefaultAsync(e => e.Email == email);
        return employee?.Id;
    }

    private static LeaveRequestResponse MapResponse(LeaveRequest lr) => new()
    {
        Id = lr.Id, EmployeeId = lr.EmployeeId,
        EmployeeName = lr.Employee != null ? $"{lr.Employee.FirstName} {lr.Employee.LastName}" : "Unknown",
        LeaveTypeId = lr.LeaveTypeId, LeaveTypeName = lr.LeaveType?.Name ?? "Unknown",
        StartDate = lr.StartDate, EndDate = lr.EndDate, Reason = lr.Reason,
        Status = lr.Status.ToString(), ApprovedBy = lr.ApprovedBy, ApprovedAt = lr.ApprovedAt
    };
}
