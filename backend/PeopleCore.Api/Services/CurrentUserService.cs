using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using PeopleCore.Application.Identity.Services;

namespace PeopleCore.Api.Services;

public sealed class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    private ClaimsPrincipal Principal => httpContextAccessor.HttpContext?.User ?? new ClaimsPrincipal();

    public bool IsAuthenticated => Principal.Identity?.IsAuthenticated == true;

    public string UserId =>
        Principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
        ?? Principal.FindFirst(ClaimTypes.NameIdentifier)?.Value
        ?? throw new UnauthorizedAccessException();
    public string? Email =>
        Principal.FindFirst(JwtRegisteredClaimNames.Email)?.Value
        ?? Principal.FindFirst(ClaimTypes.Email)?.Value;
    public IReadOnlyCollection<string> Roles =>
        Principal.FindAll(ClaimTypes.Role).Select(claim => claim.Value).ToArray();
    public int? EmployeeId
    {
        get
        {
            var claim = Principal.FindFirst("employee_id")?.Value;
            return int.TryParse(claim,out var  employeeId) ? employeeId : null;
        }
    }
}