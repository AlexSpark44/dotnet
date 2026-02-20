using Branch.Platform.Domain.Orders;
using MediatR;

namespace Branch.Platform.Application.Orders;

public sealed record ListOrdersQuery(int PageNumber, int PageSize) : IRequest<IReadOnlyList<OrderReadModel>>;

public sealed class ListOrdersQueryHandler(IOrderRepository repository) : IRequestHandler<ListOrdersQuery, IReadOnlyList<OrderReadModel>>
{
    public async Task<IReadOnlyList<OrderReadModel>> Handle(ListOrdersQuery request, CancellationToken cancellationToken)
    {
        var orders = await repository.ListPagedAsync(request.PageNumber, request.PageSize, cancellationToken);
        return orders.Select(x => x.ToReadModel()).ToList();
    }
}
