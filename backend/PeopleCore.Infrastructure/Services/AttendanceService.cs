using Microsoft.EntityFrameworkCore;
using PeopleCore.Application.Attendance.DTOs;
using PeopleCore.Application.Attendance.Services;
using PeopleCore.Application.Common;
using PeopleCore.Domain.Attendance;
using PeopleCore.Infrastructure.Data;
using DbContext = PeopleCore.Infrastructure.Data.DbContext;

namespace PeopleCore.Infrastructure.Services;

public class AttendanceService(DbContext context) : IAttendanceService
{
    public async Task<ApiResponse<AttendanceResponse>> ClockInAsync(int employeeId)
    {
        var date = DateTime.Today;
        var existing = await context.AttendanceRecords.FirstOrDefaultAsync(a => a.EmployeeId == employeeId && a.Date.Date == date.Date);
        if (existing != null && existing.ClockIn.HasValue)
            return ApiResponse<AttendanceResponse>.Error("Already clocked in today");

        if (existing == null)
        {
            existing = new AttendanceRecord { EmployeeId = employeeId, Date = DateTime.Now };
            context.AttendanceRecords.Add(existing);
        }

        existing.ClockIn = DateTime.Now;
        existing.Status = DateTime.Now.Hour >= 9 ? AttendanceStatus.Late : AttendanceStatus.Present;
        await context.SaveChangesAsync();
        return ApiResponse<AttendanceResponse>.Ok(MapResponse(existing), "Clocked in successfully");
    }

    public async Task<ApiResponse<AttendanceResponse>> ClockOutAsync(int employeeId)
    {
        var date = DateTime.Today;
        var record = await context.AttendanceRecords.FirstOrDefaultAsync(a => a.EmployeeId == employeeId && a.Date.Date == date.Date);
        if (record == null || !record.ClockIn.HasValue)
            return ApiResponse<AttendanceResponse>.Error("No clock-in record found");
        if (record.ClockOut.HasValue)
            return ApiResponse<AttendanceResponse>.Error("Already clocked out");

        record.ClockOut = DateTime.Now;
        await context.SaveChangesAsync();
        return ApiResponse<AttendanceResponse>.Ok(MapResponse(record), "Clocked out successfully");
    }

    public async Task<ApiResponse<IEnumerable<AttendanceResponse>>> GetByEmployeeAsync(int employeeId, DateTime? startDate, DateTime? endDate)
    {
        var query = context.AttendanceRecords.Where(a => a.EmployeeId == employeeId).AsQueryable();
        if (startDate.HasValue) query = query.Where(a => a.Date >= startDate.Value);
        if (endDate.HasValue) query = query.Where(a => a.Date <= endDate.Value);

        var records = await query.OrderByDescending(a => a.Date).ToListAsync();
        return ApiResponse<IEnumerable<AttendanceResponse>>.Ok(records.Select(MapResponse));
    }

    public async Task<int?> GetEmployeeByEmailAsync(string email)
    {
        var employee = await context.Employees.FirstOrDefaultAsync(e => e.Email == email);
        return employee?.Id;
    }

    private static AttendanceResponse MapResponse(AttendanceRecord a) => new()
    {
        Id = a.Id, EmployeeId = a.EmployeeId, Date = a.Date, ClockIn = a.ClockIn,
        ClockOut = a.ClockOut, BreakMinutes = a.BreakMinutes, Status = a.Status.ToString()
    };
}
