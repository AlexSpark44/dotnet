using Branch.Platform.Domain.Abstractions;

namespace Branch.Platform.Domain.Orders;

public sealed record OrderCreatedDomainEvent(Guid OrderId, DateTime OccurredOnUtc) : IDomainEvent;
