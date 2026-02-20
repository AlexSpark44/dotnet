using Branch.Platform.Domain.Abstractions;

namespace Branch.Platform.Domain.Orders;

public interface IOutboxWriter
{
    Task WriteAsync(IEnumerable<IDomainEvent> events, CancellationToken cancellationToken);
}
