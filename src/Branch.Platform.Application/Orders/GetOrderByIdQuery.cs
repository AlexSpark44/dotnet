using Branch.Platform.Domain.Orders;
using MediatR;

namespace Branch.Platform.Application.Orders;

public sealed record GetOrderByIdQuery(Guid OrderId) : IRequest<OrderReadModel?>;

public sealed class GetOrderByIdQueryHandler(IOrderRepository repository, IOrderCache cache, ICacheMetrics metrics)
    : IRequestHandler<GetOrderByIdQuery, OrderReadModel?>
{
    public async Task<OrderReadModel?> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var cached = await cache.GetAsync(request.OrderId, cancellationToken);
        if (cached is not null)
        {
            metrics.RecordHit();
            return cached;
        }

        metrics.RecordMiss();
        var order = await repository.GetByIdAsync(request.OrderId, cancellationToken);
        if (order is null) return null;

        var readModel = order.ToReadModel();
        var ttl = TimeSpan.FromMinutes(Math.Clamp(60 - readModel.Items.Count, 5, 60));
        await cache.SetAsync(readModel, ttl, cancellationToken);
        return readModel;
    }
}
