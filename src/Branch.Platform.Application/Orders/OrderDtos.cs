namespace Branch.Platform.Application.Orders;

public sealed record CreateOrderItemDto(Guid ProductId, int Quantity, decimal UnitPrice);
public sealed record CreateOrderDto(Guid CustomerId, string Currency, IReadOnlyList<CreateOrderItemDto> Items, string? IdempotencyKey);
