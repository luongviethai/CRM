namespace CRM.Application.StockMovements.Dtos;

public record StockMovementDto(
    int Id,
    int ProductId,
    string ProductName,
    string Type,
    int Quantity,
    int? Reference,
    DateTime CreatedAtUtc,
    string? Note
);
