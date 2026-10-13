using CRM.Application.Common.Models;
using CRM.Application.Customers.Dtos;

namespace CRM.Application.Customers;

public interface ICustomerService
{
    Task<PageResult<CustomerDto>> GetAllAsync(CustomerQueryParams? queryParams);
    Task<CustomerDto?> GetByIdAsync(int id);
    Task<CustomerDto> CreateAsync(CreateCustomerRequest request);
    Task<CustomerDto?> UpdateAsync(int id, UpdateCustomerRequest request);
    Task<bool> DeleteAsync(int id);
}
