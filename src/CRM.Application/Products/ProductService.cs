using CRM.Application.Common.Interfaces;
using CRM.Application.Common.Models;
using CRM.Application.Products.Dtos;
using Microsoft.EntityFrameworkCore;

namespace CRM.Application.Products;

public class ProductService : IProductService
{
    private readonly IApplicationDbContext _context;

    public ProductService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PageResult<ProductDto>> GetAllAsync(ProductQueryParams? queryParams)
    {
        var pageNumber = queryParams?.Page ?? 1;

        var pageSize = queryParams?.PageSize ?? 10;

        var query = _context.Products
            .Include(p => p.Category)
            .Include(p => p.Supplier)
            .AsQueryable();

        if (queryParams?.CategoryId is int categoryId)
        {
            query = query.Where(p => p.CategoryId == categoryId);
        }

        if (!string.IsNullOrWhiteSpace(queryParams?.Name))
        {
            query = query.Where(p => p.Name.Contains(queryParams.Name));
        }

        query = queryParams?.SortBy?.ToLower() switch
        {
            "name" => queryParams.SortDescending
                ? query.OrderByDescending(p => p.Name)
                : query.OrderBy(p => p.Name),
            "price" => queryParams.SortDescending
                ? query.OrderByDescending(p => p.Price)
                : query.OrderBy(p => p.Price),
            _ => query.OrderBy(p => p.Id)
        };

        // Đếm sau khi filter, trước khi phân trang: đây là tổng số dòng KHỚP điều kiện,
        // không phải tổng số dòng toàn bảng.
        var totalCount = await query.CountAsync();

        var products = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(p => p.ToDto())
            .ToListAsync();

        return new PageResult<ProductDto>
        {
            Items = products,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize,
        };
    }

    public async Task<ProductDto?> GetByIdAsync(int id)
    {
        var product = await _context
            .Products.Include(p => p.Category)
            .Include(p => p.Supplier)
            .FirstOrDefaultAsync(p => p.Id == id);

        return product?.ToDto();
    }

    public async Task<ProductDto> CreateAsync(CreateProductRequest request)
    {
        var product = request.ToEntity();

        _context.Products.Add(product);

        await _context.SaveChangesAsync();

        return product.ToDto();
    }

    public async Task<ProductDto?> UpdateAsync(int id, UpdateProductRequest request)
    {
        var product = await _context.Products.FindAsync(id);

        if (product == null)
        {
            return null;
        }

        request.ApplyTo(product);

        await _context.SaveChangesAsync();

        return product.ToDto();
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var product = await _context.Products.FindAsync(id);

        if (product == null)
        {
            return false;
        }

        _context.Products.Remove(product);

        await _context.SaveChangesAsync();

        return true;
    }
}
