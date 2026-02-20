using MediatR;

namespace Branch.Platform.Application.Orders;

public sealed record ListOrdersQuery(int PageNumber, int PageSize) : IRequest<IReadOnlyList<OrderReadModel>>;

public sealed class ListOrdersQueryHandler(IOrderReadRepository readRepository) : IRequestHandler<ListOrdersQuery, IReadOnlyList<OrderReadModel>>
{
    public Task<IReadOnlyList<OrderReadModel>> Handle(ListOrdersQuery request, CancellationToken cancellationToken) =>
        readRepository.ListPagedAsync(request.PageNumber, request.PageSize, cancellationToken);
}
