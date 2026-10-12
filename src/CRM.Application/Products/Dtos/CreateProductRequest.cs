namespace CRM.Application.Products.Dtos;

public record CreateProductRequest(
    string SKU,
    string Name,
    string? Description,
    decimal Price,
    int StockQuantity,
    string Unit,
    int CategoryId,
    int SupplierId
);
