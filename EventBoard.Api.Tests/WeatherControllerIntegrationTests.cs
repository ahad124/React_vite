using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace EventBoard.Api.Tests;

/// <summary>
/// Covers UAT-API-001. In the test environment no OpenWeather API key is configured,
/// so the service degrades gracefully: the endpoint still returns 200 with a WeatherDto
/// whose Available flag is false. This is the exact contract the UI relies on.
/// </summary>
public class WeatherControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public WeatherControllerIntegrationTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetWeatherForEvent_ExistingEvent_ReturnsOkAndDegradesGracefully()
    {
        // Event 1 is seeded ("Global Tech Summit 2026", San Francisco).
        var response = await _client.GetAsync("/api/weather/event/1");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<WeatherResponse>();
        Assert.NotNull(body);
        // No API key in test env -> unavailable, but the city is still echoed back.
        Assert.False(body!.Available);
        Assert.False(string.IsNullOrWhiteSpace(body.City));
    }

    [Fact]
    public async Task GetWeatherForEvent_UnknownEvent_ReturnsNotFound()
    {
        var response = await _client.GetAsync("/api/weather/event/999999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private sealed class WeatherResponse
    {
        public bool Available { get; set; }
        public string? City { get; set; }
        public double TemperatureC { get; set; }
        public string? Description { get; set; }
    }
}
