
using FluentValidation;
using PeopleCore.Application.Employees.DTOs;

namespace PeopleCore.Application.Employees.Validators;

public class CreateEmployeeRequestValidator: AbstractValidator<CreateEmployeeRequest>
{
    public CreateEmployeeRequestValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(256);
        RuleFor(x => x.PhoneNumber).MaximumLength(20);
        RuleFor(x => x.HireDate).NotEmpty();
        RuleFor(x => x.DepartmentId).GreaterThan(0);
        RuleFor(x => x.PositionId).GreaterThan(0);
    }
}

public class UpdateEmployeeRequestValidator: AbstractValidator<UpdateEmployeeRequest>
{
    public UpdateEmployeeRequestValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(256);
        RuleFor(x => x.PhoneNumber).MaximumLength(20);
        RuleFor(x => x.HireDate).NotEmpty();
        RuleFor(x => x.DepartmentId).GreaterThan(0);
        RuleFor(x => x.PositionId).GreaterThan(0);
        RuleFor(x => x.Status).Must(status => status == null || status == "Active" || status == "Inactive")
            .WithMessage("Status must be either 'Active' or 'Inactive'.");
    }
}