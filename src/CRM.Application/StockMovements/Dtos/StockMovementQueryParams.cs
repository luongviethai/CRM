using CRM.Domain.Entities;

namespace CRM.Application.StockMovements.Dtos;

public record StockMovementQueryParams(
    int Page = 1,
    int PageSize = 10,
    StockMovementType? Type = null,
    DateTime? From = null,
    DateTime? To = null
);
