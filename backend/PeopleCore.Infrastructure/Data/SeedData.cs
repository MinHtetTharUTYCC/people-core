using Microsoft.AspNetCore.Identity;
using PeopleCore.Application.Identity;

namespace PeopleCore.Infrastructure.Data;

public static class SeedData
{
    public static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
    {
        foreach (var role in RoleConstants.All)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }
}
