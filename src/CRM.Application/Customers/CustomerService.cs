using CRM.Application.Common.Interfaces;
using CRM.Application.Common.Models;
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

    public async Task<PageResult<CustomerDto>> GetAllAsync(CustomerQueryParams? queryParams)
    {
        var pageNumber = queryParams?.Page ?? 1;

        var pageSize = queryParams?.PageSize ?? 10;

        var query = _context.Customers.AsQueryable();

        if (!string.IsNullOrWhiteSpace(queryParams?.Name))
        {
            query = query.Where(c => c.Name.Contains(queryParams.Name));
        }

        query = queryParams?.SortBy?.ToLower() switch
        {
            "email" => queryParams.SortDescending
                ? query.OrderByDescending(c => c.Email)
                : query.OrderBy(c => c.Email),
            "name" => queryParams.SortDescending
                ? query.OrderByDescending(c => c.Name)
                : query.OrderBy(c => c.Name),
            _ => query.OrderBy(c => c.Id)
        };

        var totalCount = await query.CountAsync();

        var customers = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(c => c.ToDto())
            .ToListAsync();

        return new PageResult<CustomerDto>
        {
            Items = customers,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize,
        };
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
