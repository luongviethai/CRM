using CRM.Application.Common.Interfaces;
using CRM.Application.Suppliers.Dtos;
using Microsoft.EntityFrameworkCore;

namespace CRM.Application.Suppliers;

public class SupplierService : ISupplierService
{
    private readonly IApplicationDbContext _context;

    public SupplierService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<SupplierDto>> GetAllAsync()
    {
        var suppliers = await _context.Suppliers
            .Select(s => s.ToDto())
            .ToListAsync();

        return suppliers;
    }

    public async Task<SupplierDto?> GetByIdAsync(int id)
    {
        var supplier = await _context.Suppliers
            .FirstOrDefaultAsync(s => s.Id == id);

        return supplier?.ToDto();
    }

    public async Task<SupplierDto> CreateAsync(CreateSupplierRequest request)
    {
        var supplier = request.ToEntity();

        _context.Suppliers.Add(supplier);

        await _context.SaveChangesAsync();

        return supplier.ToDto();
    }

    public async Task<SupplierDto?> UpdateAsync(int id, UpdateSupplierRequest request)
    {
        var supplier = await _context.Suppliers.FindAsync(id);

        if (supplier == null)
        {
            return null;
        }

        request.ApplyTo(supplier);

        await _context.SaveChangesAsync();

        return supplier.ToDto();
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var supplier = await _context.Suppliers.FindAsync(id);

        if (supplier == null)
        {
            return false;
        }

        _context.Suppliers.Remove(supplier);

        await _context.SaveChangesAsync();

        return true;
    }
}
