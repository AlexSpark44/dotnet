namespace Branch.Platform.Contracts.Orders.V1;

public sealed record OrderResponse(
    Guid Id,
    Guid CustomerId,
    string Currency,
    decimal TotalAmount,
    string Status,
    DateTime CreatedAtUtc,
    IReadOnlyList<OrderItemResponse> Items);

public sealed record OrderItemResponse(Guid ProductId, int Quantity, decimal UnitPrice, decimal LineTotal);

public sealed record PagedOrdersResponse(IReadOnlyList<OrderResponse> Items, int PageNumber, int PageSize);
