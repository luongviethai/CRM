using CRM.Application.Common.Interfaces;
using CRM.Application.Customers.Dtos;
using Microsoft.EntityFrameworkCore;

namespace CRM.Application.Customers;

public class CustomerService : ICustomerService
{
    private readonly IApplicationDbContext _context;

    public CustomerService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<CustomerDto>> GetAllAsync()
    {
        var customers = await _context.Customers
            .Select(c => c.ToDto())
            .ToListAsync();

        return customers;
    }

    public async Task<CustomerDto?> GetByIdAsync(int id)
    {
        var customer = await _context.Customers
            .FirstOrDefaultAsync(c => c.Id == id);

        return customer?.ToDto();
    }

    public async Task<CustomerDto> CreateAsync(CreateCustomerRequest request)
    {
        var customer = request.ToEntity();

        _context.Customers.Add(customer);

        await _context.SaveChangesAsync();

        return customer.ToDto();
    }

    public async Task<CustomerDto?> UpdateAsync(int id, UpdateCustomerRequest request)
    {
        var customer = await _context.Customers.FindAsync(id);

        if (customer == null)
        {
            return null;
        }

        request.ApplyTo(customer);

        await _context.SaveChangesAsync();

        return customer.ToDto();
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var customer = await _context.Customers.FindAsync(id);

        if (customer == null)
        {
            return false;
        }

        _context.Customers.Remove(customer);

        await _context.SaveChangesAsync();

        return true;
    }
}
