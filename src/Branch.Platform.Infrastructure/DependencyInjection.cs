using Branch.Platform.Application.Orders;
using Branch.Platform.Domain.Orders;
using Branch.Platform.Infrastructure.Orders;
using Branch.Platform.Infrastructure.Outbox;
using Branch.Platform.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Branch.Platform.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<PlatformDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("Postgres")));

        services.AddStackExchangeRedisCache(options =>
            options.Configuration = configuration.GetConnectionString("Redis"));

        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IOrderReadRepository, OrderReadRepository>();
        services.AddScoped<IOrderCache, RedisOrderCache>();
        services.AddScoped<IOutboxWriter, OutboxWriter>();
        services.AddScoped<IIdempotencyStore, IdempotencyStore>();
        services.AddHostedService<OutboxDispatcherService>();

        return services;
    }
}
