using FluentValidation;
using PeopleCore.Application.Payroll.DTOs;

namespace PeopleCore.Application.Payroll.Validators;

public class CreateSalaryComponentRequestValidator: AbstractValidator<CreateSalaryComponentRequest>
{
     public CreateSalaryComponentRequestValidator()
    {
        RuleFor(x => x.EmployeeId).GreaterThan(0);
        RuleFor(x => x.Type).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Amount).NotEmpty().GreaterThan(0);
    }
}

public class GeneratePayRunRequestValidator : AbstractValidator<GeneratePayRunRequest>
{
    public GeneratePayRunRequestValidator()
    {
        RuleFor(x => x.EmployeeId).GreaterThan(0);
        RuleFor(x => x.PeriodStart).NotEmpty();
        RuleFor(x => x.PeriodEnd).NotEmpty().GreaterThan(x => x.PeriodStart).WithMessage("Period end must be after period start");
    }
}