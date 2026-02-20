using Branch.Platform.Domain.Orders;
using Branch.Platform.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Branch.Platform.Infrastructure.Orders;

public sealed class OrderRepository(PlatformDbContext dbContext) : IOrderRepository
{
    public async Task AddAsync(Order order, CancellationToken cancellationToken)
    {
        await dbContext.Orders.AddAsync(order, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        dbContext.Orders.AsNoTracking().Include(x => x.Items).FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Order>> ListPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken) =>
        await dbContext.Orders
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedAtUtc)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Include(x => x.Items)
            .ToListAsync(cancellationToken);
}
