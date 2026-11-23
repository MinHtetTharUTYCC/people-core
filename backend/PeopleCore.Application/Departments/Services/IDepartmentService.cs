using PeopleCore.Application.Common;
using PeopleCore.Application.Departments.DTOs;

namespace PeopleCore.Application.Departments.Services;

public interface IDepartmentService
{
    Task<ApiResponse<IEnumerable<DepartmentResponse>>> GetAllAsync();
    Task<ApiResponse<DepartmentResponse>> GetByIdAsync(int id);
    Task<ApiResponse<DepartmentResponse>> CreateAsync(CreateDepartmentRequest request);
    Task<ApiResponse<DepartmentResponse>> UpdateAsync(int id, UpdateDepartmentRequest request);
    Task<ApiResponse<bool>> DeleteAsync(int id);

    Task<ApiResponse<IEnumerable<PositionResponse>>> GetAllPositionsAsync();
    Task<ApiResponse<PositionResponse>> GetPositionByIdAsync(int id);
    Task<ApiResponse<PositionResponse>> CreatePositionAsync(CreatePositionRequest request);
    Task<ApiResponse<PositionResponse>> UpdatePositionAsync(int id, UpdatePositionRequest request);
    Task<ApiResponse<bool>> DeletePositionAsync(int id);
}
