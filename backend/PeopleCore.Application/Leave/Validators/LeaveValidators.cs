using FluentValidation;
using PeopleCore.Application.Leave.DTOs;

namespace PeopleCore.Application.Leave.Validators;

public class CreateLeaveRequestValidator : AbstractValidator<CreateLeaveRequest>
{
    public CreateLeaveRequestValidator()
    {
        RuleFor(x => x.EmployeeId).GreaterThan(0);
        RuleFor(x => x.LeaveTypeId).GreaterThan(0);
        RuleFor(x => x.StartDate).NotEmpty();
        RuleFor(x => x.EndDate).NotEmpty().GreaterThanOrEqualTo(x => x.StartDate).WithMessage("End date must be greater than or equal to start date.");
        RuleFor(x => x.Reason).MaximumLength(1000);
    }
}