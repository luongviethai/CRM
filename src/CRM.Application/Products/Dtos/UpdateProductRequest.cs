namespace CRM.Application.Products.Dtos;

public record UpdateProductRequest(
    string SKU,
    string Name,
    string? Description,
    decimal Price,
    int StockQuantity,
    string Unit,
    bool IsActive,
    int CategoryId,
    int SupplierId
);
