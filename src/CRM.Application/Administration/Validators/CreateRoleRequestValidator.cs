using CRM.Application.Administration.Dtos;
using FluentValidation;

namespace CRM.Application.Administration.Validators;

public class CreateRoleRequestValidator : AbstractValidator<CreateRoleRequest>
{
    public CreateRoleRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Role name is required.")
            .MaximumLength(256).WithMessage("Role name must not exceed 256 characters.");
    }
}
