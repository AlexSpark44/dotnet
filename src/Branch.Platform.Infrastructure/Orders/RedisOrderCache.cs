using System.Text.Json;
using Branch.Platform.Application.Orders;
using Microsoft.Extensions.Caching.Distributed;
using Polly;
using Polly.Timeout;

namespace Branch.Platform.Infrastructure.Orders;

public sealed class RedisOrderCache(IDistributedCache cache) : IOrderCache
{
    private static readonly IAsyncPolicy RetryWithJitterPolicy = Policy
        .Handle<Exception>(exception => exception is not TimeoutRejectedException)
        .WaitAndRetryAsync(3, attempt => TimeSpan.FromMilliseconds((Math.Pow(2, attempt) * 50) + Random.Shared.Next(0, 100)));

    private static readonly IAsyncPolicy TimeoutPolicy = Policy.TimeoutAsync(TimeSpan.FromSeconds(2));
    private static readonly IAsyncPolicy ResiliencePolicy = Policy.WrapAsync(RetryWithJitterPolicy, TimeoutPolicy);

    public async Task<OrderReadModel?> GetAsync(Guid orderId, CancellationToken cancellationToken)
    {
        var payload = await ResiliencePolicy.ExecuteAsync(async ct => await cache.GetStringAsync(CacheKey(orderId), ct), cancellationToken);
        return payload is null ? null : JsonSerializer.Deserialize<OrderReadModel>(payload);
    }

    public Task SetAsync(OrderReadModel order, TimeSpan ttl, CancellationToken cancellationToken) =>
        ResiliencePolicy.ExecuteAsync(ct => cache.SetStringAsync(CacheKey(order.Id), JsonSerializer.Serialize(order), new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = ttl
        }, ct), cancellationToken);

    private static string CacheKey(Guid orderId) => $"orders:{orderId}";
}
