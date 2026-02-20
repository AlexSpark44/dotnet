using System.Text.Json;
using Branch.Platform.Domain.Abstractions;
using Branch.Platform.Domain.Orders;
using Branch.Platform.Infrastructure.Persistence;

namespace Branch.Platform.Infrastructure.Outbox;

public sealed class OutboxWriter(PlatformDbContext dbContext) : IOutboxWriter
{
    public async Task WriteAsync(IEnumerable<IDomainEvent> events, CancellationToken cancellationToken)
    {
        foreach (var domainEvent in events)
        {
            await dbContext.OutboxMessages.AddAsync(new OutboxMessage
            {
                Type = domainEvent.GetType().Name,
                Payload = JsonSerializer.Serialize(domainEvent),
                OccurredOnUtc = domainEvent.OccurredOnUtc
            }, cancellationToken);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
