using CRM.Application.Customers.Dtos;
using CRM.Domain.Entities;

namespace CRM.Application.Customers;

public static class CustomerMappingExtensions
{
    public static CustomerDto ToDto(this Customer customer)
    {
        return new CustomerDto(
            customer.Id,
            customer.Name,
            customer.Email,
            customer.Phone,
            customer.Address
        );
    }

    public static Customer ToEntity(this CreateCustomerRequest request)
    {
        return new Customer
        {
            Name = request.Name,
            Email = request.Email,
            Phone = request.Phone,
            Address = request.Address
        };
    }

    public static void ApplyTo(this UpdateCustomerRequest request, Customer customer)
    {
        customer.Name = request.Name;
        customer.Email = request.Email;
        customer.Phone = request.Phone;
        customer.Address = request.Address;
    }
}
