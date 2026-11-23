using PeopleCore.Application.Common;
using PeopleCore.Application.Attendance.DTOs;

namespace PeopleCore.Application.Attendance.Services;

public interface IAttendanceService
{
    Task<ApiResponse<AttendanceResponse>> ClockInAsync(int employeeId);
    Task<ApiResponse<AttendanceResponse>> ClockOutAsync(int employeeId);
    Task<ApiResponse<IEnumerable<AttendanceResponse>>> GetByEmployeeAsync(int employeeId, DateTime? startDate, DateTime? endDate);
    Task<int?> GetEmployeeByEmailAsync(string email);
}
