using CRM.Application.Categories;
using CRM.Application.Customers;
using CRM.Application.Products;
using CRM.Application.Suppliers;
using CRM.Application.Orders;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace CRM.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<ISupplierService, SupplierService>();
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<IOrderService, OrderService>();


        // Quét toàn bộ assembly Application, tự đăng ký mọi class kế thừa AbstractValidator<T>
        // (dùng IProductService chỉ để trỏ đúng assembly cần quét, không liên quan tới validator).
        services.AddValidatorsFromAssemblyContaining<IProductService>();

        return services;
    }
}
