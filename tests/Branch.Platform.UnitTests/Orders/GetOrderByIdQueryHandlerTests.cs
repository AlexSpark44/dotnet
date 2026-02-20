using Branch.Platform.Application.Orders;
using Branch.Platform.Domain.Orders;
using FluentAssertions;
using Moq;

namespace Branch.Platform.UnitTests.Orders;

public class GetOrderByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnCachedValue_WhenPresent()
    {
        var orderId = Guid.NewGuid();
        var readRepository = new Mock<IOrderReadRepository>();
        var cache = new Mock<IOrderCache>();
        var metrics = new Mock<ICacheMetrics>();
        var cached = new OrderReadModel(orderId, Guid.NewGuid(), "USD", 10m, OrderStatus.Created, DateTime.UtcNow,
            new List<OrderItemReadModel> { new(Guid.NewGuid(), 1, 10m, 10m) });

        cache.Setup(x => x.GetAsync(orderId, It.IsAny<CancellationToken>())).ReturnsAsync(cached);

        var sut = new GetOrderByIdQueryHandler(readRepository.Object, cache.Object, metrics.Object);

        var result = await sut.Handle(new GetOrderByIdQuery(orderId), CancellationToken.None);

        result.Should().NotBeNull();
        result!.Id.Should().Be(orderId);
        readRepository.Verify(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        metrics.Verify(x => x.RecordHit(), Times.Once);
    }
}
