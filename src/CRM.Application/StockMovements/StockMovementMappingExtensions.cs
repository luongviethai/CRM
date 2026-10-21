using CRM.Application.StockMovements.Dtos;
using CRM.Domain.Entities;

namespace CRM.Application.StockMovements;

public static class StockMovementMappingExtensions
{
    public static StockMovementDto ToDto(this StockMovement movement, Product product)
    {
        return new StockMovementDto(
            movement.Id,
            movement.ProductId,
            product.Name,
            movement.Type.ToString(),
            movement.Quantity,
            movement.Reference,
            movement.CreatedAtUtc,
            movement.Note
        );
    }
}
