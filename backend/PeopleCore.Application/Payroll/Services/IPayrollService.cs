using PeopleCore.Application.Common;
using PeopleCore.Application.Payroll.DTOs;

namespace PeopleCore.Application.Payroll.Services;

public interface IPayrollService
{
    Task<ApiResponse<IEnumerable<SalaryComponentResponse>>> GetSalaryComponentsAsync(int employeeId);
    Task<ApiResponse<SalaryComponentResponse>> AddSalaryComponentAsync(CreateSalaryComponentRequest request);
    Task<ApiResponse<bool>> DeleteSalaryComponentAsync(int id);
    Task<ApiResponse<IEnumerable<PayRunResponse>>> GetPayRunsAsync(int employeeId);
    Task<ApiResponse<PayRunResponse>> GeneratePayRunAsync(GeneratePayRunRequest request);
    Task<ApiResponse<PayRunResponse>> ProcessPayRunAsync(int id);
}
