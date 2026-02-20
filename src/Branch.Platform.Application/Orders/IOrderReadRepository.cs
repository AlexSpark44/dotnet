namespace Branch.Platform.Application.Orders;

public interface IOrderReadRepository
{
    Task<OrderReadModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyList<OrderReadModel>> ListPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);
}
