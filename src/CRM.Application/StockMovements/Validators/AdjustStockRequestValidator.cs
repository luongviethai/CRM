using CRM.Application.StockMovements.Dtos;
using FluentValidation;

namespace CRM.Application.StockMovements.Validators;

public class AdjustStockRequestValidator : AbstractValidator<AdjustStockRequest>
{
    public AdjustStockRequestValidator()
    {
        RuleFor(x => x.Quantity)
            .NotEqual(0).WithMessage("Số lượng điều chỉnh không được bằng 0.");
    }
}
