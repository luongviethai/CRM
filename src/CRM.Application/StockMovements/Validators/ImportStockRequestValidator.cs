using CRM.Application.StockMovements.Dtos;
using FluentValidation;

namespace CRM.Application.StockMovements.Validators;

public class ImportStockRequestValidator : AbstractValidator<ImportStockRequest>
{
    public ImportStockRequestValidator()
    {
        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Số lượng nhập phải lớn hơn 0.");
    }
}
