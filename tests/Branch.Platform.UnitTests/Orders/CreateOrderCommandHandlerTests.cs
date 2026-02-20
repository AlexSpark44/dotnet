using Branch.Platform.Application.Orders;
using Branch.Platform.Domain.Abstractions;
using Branch.Platform.Domain.Orders;
using FluentAssertions;
using Moq;

namespace Branch.Platform.UnitTests.Orders;

public class CreateOrderCommandHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnExistingOrder_WhenIdempotencyKeyAlreadyExists()
    {
        var existingOrderId = Guid.NewGuid();
        var repository = new Mock<IOrderRepository>();
        var outbox = new Mock<IOutboxWriter>();
        var idempotency = new Mock<IIdempotencyStore>();
        idempotency.Setup(x => x.TryGetOrderIdAsync("key-1", It.IsAny<CancellationToken>())).ReturnsAsync(existingOrderId);

        var sut = new CreateOrderCommandHandler(repository.Object, outbox.Object, idempotency.Object);
        var command = new CreateOrderCommand(new CreateOrderDto(Guid.NewGuid(), "USD", new List<CreateOrderItemDto>
        {
            new(Guid.NewGuid(), 1, 2)
        }, "key-1"));

        var result = await sut.Handle(command, CancellationToken.None);

        result.Should().Be(existingOrderId);
        repository.Verify(x => x.AddAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
