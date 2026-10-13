using CRM.Application.Suppliers.Dtos;
using CRM.Domain.Entities;

namespace CRM.Application.Suppliers;

public static class SupplierMappingExtensions
{
    public static SupplierDto ToDto(this Supplier supplier)
    {
        return new SupplierDto(
            supplier.Id,
            supplier.Name,
            supplier.ContactName,
            supplier.Email,
            supplier.Phone,
            supplier.Address
        );
    }

    public static Supplier ToEntity(this CreateSupplierRequest request)
    {
        return new Supplier
        {
            Name = request.Name,
            ContactName = request.ContactName,
            Email = request.Email,
            Phone = request.Phone,
            Address = request.Address
        };
    }

    public static void ApplyTo(this UpdateSupplierRequest request, Supplier supplier)
    {
        supplier.Name = request.Name;
        supplier.ContactName = request.ContactName;
        supplier.Email = request.Email;
        supplier.Phone = request.Phone;
        supplier.Address = request.Address;
    }
}
