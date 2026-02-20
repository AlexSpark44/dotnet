using MediatR;

namespace Branch.Platform.Application.Orders;

public sealed record GetOrderByIdQuery(Guid OrderId) : IRequest<OrderReadModel?>;

public sealed class GetOrderByIdQueryHandler(IOrderReadRepository readRepository, IOrderCache cache, ICacheMetrics metrics)
    : IRequestHandler<GetOrderByIdQuery, OrderReadModel?>
{
    public async Task<OrderReadModel?> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        using var cacheGetSpan = OrdersActivity.Source.StartActivity("orders.cache.get");
        cacheGetSpan?.SetTag("orderId", request.OrderId);

        var cached = await cache.GetAsync(request.OrderId, cancellationToken);
        var cacheHit = cached is not null;
        cacheGetSpan?.SetTag("cacheHit", cacheHit);

        if (cacheHit)
        {
            metrics.RecordHit();
            return cached;
        }

        metrics.RecordMiss();

        using var dbSpan = OrdersActivity.Source.StartActivity("orders.db.get");
        dbSpan?.SetTag("orderId", request.OrderId);
        var readModel = await readRepository.GetByIdAsync(request.OrderId, cancellationToken);
        if (readModel is null) return null;

        using var cacheSetSpan = OrdersActivity.Source.StartActivity("orders.cache.set");
        cacheSetSpan?.SetTag("orderId", request.OrderId);
        cacheSetSpan?.SetTag("cacheHit", false);
        var ttl = TimeSpan.FromMinutes(Math.Clamp(60 - readModel.Items.Count, 5, 60));
        await cache.SetAsync(readModel, ttl, cancellationToken);
        return readModel;
    }
}
