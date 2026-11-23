using Microsoft.AspNetCore.Identity;

namespace PeopleCore.Domain.Identity;

public class ApplicationUser : IdentityUser
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime? HireDate { get; set; }
    public bool IsActive { get; set; } = true;
}
