using PeopleCore.Application.Common;
using PeopleCore.Application.Identity.DTOs;

namespace PeopleCore.Application.Identity.Services;

public interface IAuthService
{
    Task<ApiResponse<AuthResponse>> LoginAsync(LoginRequest request);
    Task<ApiResponse<AuthResponse>> RegisterAsync(RegisterRequest request);
}
