namespace Branch.Platform.Application.Orders;

public sealed record OrderReadModel(
    Guid Id,
    Guid CustomerId,
    string Currency,
    decimal TotalAmount,
    string Status,
    DateTime CreatedAtUtc,
    IReadOnlyList<OrderItemReadModel> Items);

public sealed record OrderItemReadModel(Guid ProductId, int Quantity, decimal UnitPrice, decimal LineTotal);
