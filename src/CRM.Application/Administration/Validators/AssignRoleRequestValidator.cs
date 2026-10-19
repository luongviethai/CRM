using CRM.Application.Administration.Dtos;
using FluentValidation;

namespace CRM.Application.Administration.Validators;

public class AssignRoleRequestValidator : AbstractValidator<AssignRoleRequest>
{
    public AssignRoleRequestValidator()
    {
        RuleFor(x => x.RoleName)
            .NotEmpty().WithMessage("Role name is required.")
            .MaximumLength(256).WithMessage("Role name must not exceed 256 characters.");
    }
}
