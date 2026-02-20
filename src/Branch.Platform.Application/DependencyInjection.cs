using Branch.Platform.Application.Orders;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using MediatR;

namespace Branch.Platform.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
        services.AddScoped<ICacheMetrics, CacheMetrics>();
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(TelemetryBehavior<,>));
        return services;
    }
}
