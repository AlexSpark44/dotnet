namespace Branch.Platform.Contracts.Orders.V1;

public sealed record CreateOrderRequest(Guid CustomerId, string Currency, IReadOnlyList<CreateOrderItemRequest> Items);
public sealed record CreateOrderItemRequest(Guid ProductId, int Quantity, decimal UnitPrice);
