using CRM.Application.Categories.Dtos;
using CRM.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CRM.Application.Categories;

public class CategoryService : ICategoryService
{
    private readonly IApplicationDbContext _context;

    public CategoryService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<CategoryDto>> GetAllAsync()
    {
        var categories = await _context.Categories
            .Select(c => c.ToDto())
            .ToListAsync();

        return categories;
    }

    public async Task<CategoryDto?> GetByIdAsync(int id)
    {
        var category = await _context.Categories
            .FirstOrDefaultAsync(c => c.Id == id);

        return category?.ToDto();
    }

    public async Task<CategoryDto> CreateAsync(CreateCategoryRequest request)
    {
        var category = request.ToEntity();

        _context.Categories.Add(category);

        await _context.SaveChangesAsync();

        return category.ToDto();
    }

    public async Task<CategoryDto?> UpdateAsync(int id, UpdateCategoryRequest request)
    {
        var category = await _context.Categories.FindAsync(id);

        if (category == null)
        {
            return null;
        }

        request.ApplyTo(category);

        await _context.SaveChangesAsync();

        return category.ToDto();
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var category = await _context.Categories.FindAsync(id);

        if (category == null)
        {
            return false;
        }

        _context.Categories.Remove(category);

        await _context.SaveChangesAsync();

        return true;
    }
}
