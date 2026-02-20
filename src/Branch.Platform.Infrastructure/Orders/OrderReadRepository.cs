using Branch.Platform.Application.Orders;
using Branch.Platform.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Branch.Platform.Infrastructure.Orders;

public sealed class OrderReadRepository(PlatformDbContext dbContext) : IOrderReadRepository
{
    public Task<OrderReadModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        dbContext.Orders
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new OrderReadModel(
                x.Id,
                x.CustomerId,
                x.Currency,
                x.Items.Sum(i => i.Quantity * i.UnitPrice.Amount),
                x.Status,
                x.CreatedAtUtc,
                x.Items.Select(i => new OrderItemReadModel(
                    i.ProductId,
                    i.Quantity,
                    i.UnitPrice.Amount,
                    i.Quantity * i.UnitPrice.Amount)).ToList()))
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<IReadOnlyList<OrderReadModel>> ListPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken) =>
        await dbContext.Orders
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedAtUtc)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new OrderReadModel(
                x.Id,
                x.CustomerId,
                x.Currency,
                x.Items.Sum(i => i.Quantity * i.UnitPrice.Amount),
                x.Status,
                x.CreatedAtUtc,
                x.Items.Select(i => new OrderItemReadModel(
                    i.ProductId,
                    i.Quantity,
                    i.UnitPrice.Amount,
                    i.Quantity * i.UnitPrice.Amount)).ToList()))
            .ToListAsync(cancellationToken);
}
