using CRM.Application.Products.Dtos;
using CRM.Application.Common.Models;

namespace CRM.Application.Products;

public interface IProductService
{
    Task<PageResult<ProductDto>> GetAllAsync(ProductQueryParams? queryParams);
    Task<ProductDto?> GetByIdAsync(int id);
    Task<ProductDto> CreateAsync(CreateProductRequest request);
    Task<ProductDto?> UpdateAsync(int id, UpdateProductRequest request);
    Task<bool> DeleteAsync(int id);
}
