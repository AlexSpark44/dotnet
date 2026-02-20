using Branch.Platform.Domain.Orders;
using FluentAssertions;

namespace Branch.Platform.UnitTests.Orders;

public class OrderTests
{
    [Fact]
    public void Create_ShouldCalculateTotal()
    {
        var items = new[]
        {
            new OrderItem(Guid.NewGuid(), 2, new Money(10, "USD")),
            new OrderItem(Guid.NewGuid(), 1, new Money(5, "USD"))
        };

        var order = Order.Create(Guid.NewGuid(), "USD", items);

        order.TotalAmount.Should().Be(25);
        order.Items.Should().HaveCount(2);
    }
}
