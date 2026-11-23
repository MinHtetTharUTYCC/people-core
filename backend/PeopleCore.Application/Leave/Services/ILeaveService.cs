using PeopleCore.Application.Common;
using PeopleCore.Application.Leave.DTOs;

namespace PeopleCore.Application.Leave.Services;

public interface ILeaveService
{
    Task<ApiResponse<LeaveRequestResponse>> SubmitAsync(CreateLeaveRequest request);
    Task<ApiResponse<bool>> ApproveAsync(int id, string approvedBy);
    Task<ApiResponse<bool>> RejectAsync(int id, string approvedBy);
    Task<ApiResponse<IEnumerable<LeaveRequestResponse>>> GetByEmployeeAsync(int employeeId);
    Task<ApiResponse<IEnumerable<LeaveRequestResponse>>> GetPendingAsync();
    Task<ApiResponse<LeaveBalanceResponse>> GetBalanceAsync(int employeeId);
}
