using Asp.Versioning;
using Branch.Platform.Application.Orders;
using Branch.Platform.Contracts.Orders.V1;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Branch.Platform.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/orders")]
[Authorize]
public sealed class OrdersController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [AllowAnonymous]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] CreateOrderRequest request, CancellationToken cancellationToken)
    {
        var idempotencyKey = HttpContext.Request.Headers["Idempotency-Key"].ToString();
        var command = new CreateOrderCommand(new CreateOrderDto(
            request.CustomerId,
            request.Currency,
            request.Items.Select(x => new CreateOrderItemDto(x.ProductId, x.Quantity, x.UnitPrice)).ToList(),
            idempotencyKey));

        var id = await mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id, version = "1" }, id);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetOrderByIdQuery(id), cancellationToken);
        if (result is null) return NotFound();

        return Ok(new OrderResponse(result.Id, result.CustomerId, result.Currency, result.TotalAmount, result.Status, result.CreatedAtUtc,
            result.Items.Select(i => new OrderItemResponse(i.ProductId, i.Quantity, i.UnitPrice, i.LineTotal)).ToList()));
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<PagedOrdersResponse>> List([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new ListOrdersQuery(pageNumber, pageSize), cancellationToken);
        var payload = new PagedOrdersResponse(result.Select(r => new OrderResponse(r.Id, r.CustomerId, r.Currency, r.TotalAmount, r.Status, r.CreatedAtUtc,
            r.Items.Select(i => new OrderItemResponse(i.ProductId, i.Quantity, i.UnitPrice, i.LineTotal)).ToList())).ToList(), pageNumber, pageSize);
        return Ok(payload);
    }
}
