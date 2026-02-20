using Branch.Platform.Domain.Orders;

namespace Branch.Platform.Application.Orders;

public static class OrderMappers
{
    public static OrderReadModel ToReadModel(this Order order) =>
        new(order.Id, order.CustomerId, order.Currency, order.TotalAmount, order.Status, order.CreatedAtUtc,
            order.Items.Select(i => new OrderItemReadModel(i.ProductId, i.Quantity, i.UnitPrice.Amount, i.LineTotal)).ToList());
}
