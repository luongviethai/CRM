using CRM.Application.StockMovements.Dtos;

namespace CRM.Application.StockMovements;

public record StockMovementResult(bool Succeeded, StockMovementDto? Movement, string? Error, bool IsConflict = false);
