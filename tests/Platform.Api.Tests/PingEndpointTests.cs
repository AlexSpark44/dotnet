using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Platform.Api.Tests;

public class PingEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public PingEndpointTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetPing_ReturnsPong()
    {
        var response = await _client.GetAsync("/api/ping");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<PingResponse>();

        Assert.NotNull(payload);
        Assert.Equal("pong", payload!.Message);
    }

    private sealed record PingResponse(string Message);
}
