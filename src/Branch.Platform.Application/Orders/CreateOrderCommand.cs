using Branch.Platform.Domain.Orders;
using MediatR;

namespace Branch.Platform.Application.Orders;

public sealed record CreateOrderCommand(CreateOrderDto Payload) : IRequest<Guid>;

public sealed class CreateOrderCommandHandler(IOrderRepository repository, IOutboxWriter outboxWriter, IIdempotencyStore idempotencyStore)
    : IRequestHandler<CreateOrderCommand, Guid>
{
    public async Task<Guid> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(request.Payload.IdempotencyKey))
        {
            var existing = await idempotencyStore.TryGetOrderIdAsync(request.Payload.IdempotencyKey, cancellationToken);
            if (existing.HasValue) return existing.Value;
        }

        var items = request.Payload.Items.Select(x => new OrderItem(x.ProductId, x.Quantity, new Money(x.UnitPrice, request.Payload.Currency)));
        var order = Order.Create(request.Payload.CustomerId, request.Payload.Currency, items);

        await repository.AddAsync(order, cancellationToken);
        await outboxWriter.WriteAsync(order.DomainEvents, cancellationToken);

        if (!string.IsNullOrWhiteSpace(request.Payload.IdempotencyKey))
            await idempotencyStore.SaveAsync(request.Payload.IdempotencyKey, order.Id, cancellationToken);

        return order.Id;
    }
}
