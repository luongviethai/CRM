using Microsoft.Extensions.DependencyInjection;

namespace CRM.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Đăng ký service của tầng Application (FluentValidation, AutoMapper/Mapster, MediatR...)
        // sẽ bổ sung dần từ tuần 1 ngày 5 trở đi.
        return services;
    }
}
