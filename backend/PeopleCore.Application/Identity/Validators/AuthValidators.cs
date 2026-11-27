using FluentValidation;
using PeopleCore.Application.Identity.DTOs;

namespace PeopleCore.Application.Identity.Validators;

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required");
    }
}

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required")
            .MaximumLength(100).WithMessage("First name must not exceed 100 characters");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required")
            .MaximumLength(100).WithMessage("Last name must not exceed 100 characters");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters")
            .Must(p => p.Any(char.IsUpper)).WithMessage("Password must contain an uppercase letter")
            .Must(p => p.Any(char.IsLower)).WithMessage("Password must contain a lowercase letter")
            .Must(p => p.Any(char.IsDigit)).WithMessage("Password must contain a digit")
            .Must(p => p.Any(c => !char.IsLetterOrDigit(c))).WithMessage("Password must contain a special character");

        RuleFor(x => x.Role)
            .NotEmpty().WithMessage("Role is required")
            .Must(role => role == RoleConstants.Employee)
            .WithMessage("Only Employee role is allowed for self-registration");
    }
}
