namespace Branch.Platform.Domain.Orders;

public interface IOrderRepository
{
    Task AddAsync(Order order, CancellationToken cancellationToken);
    Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyList<Order>> ListPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);
}
