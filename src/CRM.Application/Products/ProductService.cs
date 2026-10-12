using CRM.Application.Common.Interfaces;
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

    public async Task<IEnumerable<ProductDto>> GetAllAsync()
    {
        var products = await _context
            .Products.Include(p => p.Category)
            .Include(p => p.Supplier)
            .Select(p => p.ToDto())
            .ToListAsync();

        return products;
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
