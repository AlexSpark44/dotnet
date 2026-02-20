namespace Branch.Platform.Domain.Orders;

public sealed class OrderItem
{
    private OrderItem() { }

    public OrderItem(Guid productId, int quantity, Money unitPrice)
    {
        if (quantity <= 0) throw new ArgumentOutOfRangeException(nameof(quantity));
        ProductId = productId;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }

    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid ProductId { get; private set; }
    public int Quantity { get; private set; }
    public Money UnitPrice { get; private set; } = default!;
    public decimal LineTotal => UnitPrice.Amount * Quantity;
}
