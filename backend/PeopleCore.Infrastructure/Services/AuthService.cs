using Microsoft.AspNetCore.Identity;
using PeopleCore.Application.Common;
using PeopleCore.Application.Identity.DTOs;
using PeopleCore.Application.Identity.Services;
using PeopleCore.Domain.Identity;

namespace PeopleCore.Infrastructure.Services;

public class AuthService(
    UserManager<ApplicationUser> userManager,
    IJwtTokenService jwtTokenService,
    RoleManager<IdentityRole> roleManager)
    : IAuthService
{
    public async Task<ApiResponse<AuthResponse>> LoginAsync(LoginRequest request)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            return ApiResponse<AuthResponse>.Error("Invalid email or password");
        }

        var passwordValid = await userManager.CheckPasswordAsync(user, request.Password);
        if (!passwordValid)
        {
            return ApiResponse<AuthResponse>.Error("Invalid email or password");
        }

        var roles = await userManager.GetRolesAsync(user);
        var token = jwtTokenService.GenerateToken(user.Id, user.Email!, roles);

        return ApiResponse<AuthResponse>.Ok(new AuthResponse
        {
            Token = token,
            Email = user.Email!,
            Roles = roles
        });
    }

    public async Task<ApiResponse<AuthResponse>> RegisterAsync(RegisterRequest request)
    {
        var existingUser = await userManager.FindByEmailAsync(request.Email);
        if (existingUser != null)
        {
            return ApiResponse<AuthResponse>.Error("Email already registered");
        }

        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => new ValidationError
            {
                Field = e.Code,
                Message = e.Description
            });
            return ApiResponse<AuthResponse>.Error("Registration failed", errors);
        }

        // Only allow Employee role for self-registration
        const string allowedRole = "Employee";
        await userManager.AddToRoleAsync(user, allowedRole);

        var roles = await userManager.GetRolesAsync(user);
        var token = jwtTokenService.GenerateToken(user.Id, user.Email!, roles);

        return ApiResponse<AuthResponse>.Ok(new AuthResponse
        {
            Token = token,
            Email = user.Email!,
            Roles = roles
        }, "Registration successful");
    }
}
