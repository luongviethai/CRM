using CRM.Application.Products.Dtos;
using CRM.Domain.Entities;

namespace CRM.Application.Products;

public static class ProductMappingExtensions
{
    // Entity -> Dto (hiển thị ra ngoài)
    public static ProductDto ToDto(this Product product)
    {
        return new ProductDto(
            product.Id,
            product.SKU,
            product.Name,
            product.Description,
            product.Price,
            product.StockQuantity,
            product.Unit,
            product.IsActive,
            product.CategoryId,
            product.Category?.Name ?? string.Empty,
            product.SupplierId,
            product.Supplier?.Name ?? string.Empty
        );
    }

    // CreateRequest -> Entity mới (dùng khi tạo)
    public static Product ToEntity(this CreateProductRequest request)
    {
        return new Product
        {
            SKU = request.SKU,
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            StockQuantity = request.StockQuantity,
            Unit = request.Unit,
            CategoryId = request.CategoryId,
            SupplierId = request.SupplierId,
            IsActive = true
        };
    }

    // UpdateRequest -> ghi đè lên Entity đã tồn tại, đang được EF Core theo dõi (dùng khi sửa)
    public static void ApplyTo(this UpdateProductRequest request, Product product)
    {
        product.SKU = request.SKU;
        product.Name = request.Name;
        product.Description = request.Description;
        product.Price = request.Price;
        product.StockQuantity = request.StockQuantity;
        product.Unit = request.Unit;
        product.IsActive = request.IsActive;
        product.CategoryId = request.CategoryId;
        product.SupplierId = request.SupplierId;
    }
}
