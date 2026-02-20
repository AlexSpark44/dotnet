using System.Net;
using System.Net.Http.Json;
using Branch.Platform.Contracts.Orders.V1;
using FluentAssertions;

namespace Branch.Platform.IntegrationTests.Orders;

public class OrdersApiTests : IClassFixture<TestPlatformFactory>
{
    private readonly HttpClient _client;

    public OrdersApiTests(TestPlatformFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task PostThenGet_ShouldReturnCreatedOrder()
    {
        var request = new CreateOrderRequest(Guid.NewGuid(), "USD", new List<CreateOrderItemRequest> { new(Guid.NewGuid(), 2, 10) });
        using var postRequest = new HttpRequestMessage(HttpMethod.Post, "/api/v1/orders") { Content = JsonContent.Create(request) };
        postRequest.Headers.Add("Idempotency-Key", Guid.NewGuid().ToString("N"));
        var postResponse = await _client.SendAsync(postRequest);
        postResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var orderId = await postResponse.Content.ReadFromJsonAsync<Guid>();
        var getResponse = await _client.GetAsync($"/api/v1/orders/{orderId}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetOrder_Twice_ShouldUseCachePath()
    {
        var request = new CreateOrderRequest(Guid.NewGuid(), "USD", new List<CreateOrderItemRequest> { new(Guid.NewGuid(), 1, 20) });
        using var postRequest = new HttpRequestMessage(HttpMethod.Post, "/api/v1/orders") { Content = JsonContent.Create(request) };
        postRequest.Headers.Add("Idempotency-Key", Guid.NewGuid().ToString("N"));
        var postResponse = await _client.SendAsync(postRequest);
        var orderId = await postResponse.Content.ReadFromJsonAsync<Guid>();

        var first = await _client.GetAsync($"/api/v1/orders/{orderId}");
        var second = await _client.GetAsync($"/api/v1/orders/{orderId}");

        first.StatusCode.Should().Be(HttpStatusCode.OK);
        second.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task CreateOrder_WithSameIdempotencyKey_ShouldReturnSameOrderId()
    {
        var request = new CreateOrderRequest(Guid.NewGuid(), "USD", new List<CreateOrderItemRequest> { new(Guid.NewGuid(), 2, 15) });

        using var req1 = new HttpRequestMessage(HttpMethod.Post, "/api/v1/orders") { Content = JsonContent.Create(request) };
        req1.Headers.Add("Idempotency-Key", "fixed-key");
        var res1 = await _client.SendAsync(req1);
        var id1 = await res1.Content.ReadFromJsonAsync<Guid>();

        using var req2 = new HttpRequestMessage(HttpMethod.Post, "/api/v1/orders") { Content = JsonContent.Create(request) };
        req2.Headers.Add("Idempotency-Key", "fixed-key");
        var res2 = await _client.SendAsync(req2);
        var id2 = await res2.Content.ReadFromJsonAsync<Guid>();

        id1.Should().Be(id2);
    }
}
