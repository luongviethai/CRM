using CRM.Application.Products;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace CRM.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IProductService, ProductService>();

        // Quét toàn bộ assembly Application, tự đăng ký mọi class kế thừa AbstractValidator<T>
        // (dùng IProductService chỉ để trỏ đúng assembly cần quét, không liên quan tới validator).
        services.AddValidatorsFromAssemblyContaining<IProductService>();

        return services;
    }
}
