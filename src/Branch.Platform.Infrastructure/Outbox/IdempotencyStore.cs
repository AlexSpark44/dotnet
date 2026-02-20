using Branch.Platform.Domain.Orders;
using Branch.Platform.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Branch.Platform.Infrastructure.Outbox;

public sealed class IdempotencyStore(PlatformDbContext dbContext) : IIdempotencyStore
{
    public async Task<Guid?> TryGetOrderIdAsync(string idempotencyKey, CancellationToken cancellationToken)
    {
        var existing = await dbContext.IdempotencyRecords.AsNoTracking().FirstOrDefaultAsync(x => x.Key == idempotencyKey, cancellationToken);
        return existing?.OrderId;
    }

    public async Task SaveAsync(string idempotencyKey, Guid orderId, CancellationToken cancellationToken)
    {
        await dbContext.IdempotencyRecords.AddAsync(new IdempotencyRecord { Key = idempotencyKey, OrderId = orderId }, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
