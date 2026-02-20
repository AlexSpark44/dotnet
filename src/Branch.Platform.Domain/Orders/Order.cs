using Branch.Platform.Domain.Abstractions;

namespace Branch.Platform.Domain.Orders;

public sealed class Order : Entity
{
    private readonly List<OrderItem> _items = new();

    private Order() { }

    private Order(Guid id, Guid customerId, string currency)
    {
        Id = id;
        CustomerId = customerId;
        Currency = currency;
        CreatedAtUtc = DateTime.UtcNow;
        Status = OrderStatus.Created;
    }

    public Guid Id { get; private set; }
    public Guid CustomerId { get; private set; }
    public OrderStatus Status { get; private set; }
    public string Currency { get; private set; } = default!;
    public DateTime CreatedAtUtc { get; private set; }
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();
    public decimal TotalAmount => _items.Sum(i => i.LineTotal);

    public static Order Create(Guid customerId, string currency, IEnumerable<OrderItem> items)
    {
        var order = new Order(Guid.NewGuid(), customerId, currency);
        foreach (var item in items) order.AddItem(item);
        if (!order._items.Any()) throw new InvalidOperationException("Order must contain at least one item.");

        order.AddDomainEvent(new OrderCreatedDomainEvent(order.Id, DateTime.UtcNow));
        return order;
    }

    private void AddItem(OrderItem item)
    {
        if (item.UnitPrice.Currency != Currency)
            throw new InvalidOperationException("All order items must share the same currency.");
        _items.Add(item);
    }
}
