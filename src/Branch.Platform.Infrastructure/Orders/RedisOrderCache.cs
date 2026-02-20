using System.Text.Json;
using Branch.Platform.Application.Orders;
using Microsoft.Extensions.Caching.Distributed;
using Polly;

namespace Branch.Platform.Infrastructure.Orders;

public sealed class RedisOrderCache(IDistributedCache cache) : IOrderCache
{
    private static readonly IAsyncPolicy RetryPolicy = Policy
        .Handle<Exception>()
        .WaitAndRetryAsync(3, attempt => TimeSpan.FromMilliseconds((Math.Pow(2, attempt) * 50) + Random.Shared.Next(0, 100)));

    public async Task<OrderReadModel?> GetAsync(Guid orderId, CancellationToken cancellationToken)
    {
        var payload = await RetryPolicy.ExecuteAsync(async ct => await cache.GetStringAsync(CacheKey(orderId), ct), cancellationToken);
        return payload is null ? null : JsonSerializer.Deserialize<OrderReadModel>(payload);
    }

    public Task SetAsync(OrderReadModel order, TimeSpan ttl, CancellationToken cancellationToken) =>
        RetryPolicy.ExecuteAsync(ct => cache.SetStringAsync(CacheKey(order.Id), JsonSerializer.Serialize(order), new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = ttl
        }, ct), cancellationToken);

    private static string CacheKey(Guid orderId) => $"orders:{orderId}";
}
