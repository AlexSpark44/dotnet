namespace Branch.Platform.Application.Orders;

public interface IOrderCache
{
    Task<OrderReadModel?> GetAsync(Guid orderId, CancellationToken cancellationToken);
    Task SetAsync(OrderReadModel order, TimeSpan ttl, CancellationToken cancellationToken);
}
