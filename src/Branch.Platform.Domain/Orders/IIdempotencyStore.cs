namespace Branch.Platform.Domain.Orders;

public interface IIdempotencyStore
{
    Task<Guid?> TryGetOrderIdAsync(string idempotencyKey, CancellationToken cancellationToken);
    Task SaveAsync(string idempotencyKey, Guid orderId, CancellationToken cancellationToken);
}
