namespace PeopleCore.Application.Identity.Services;

public interface ICurrentUserService
{
    string UserId { get; }
    string? Email { get; }
    IReadOnlyCollection<string> Roles { get; }
    int? EmployeeId { get; }
    bool IsAuthenticated { get; }
}