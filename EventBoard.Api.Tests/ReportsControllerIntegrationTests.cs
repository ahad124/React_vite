using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace EventBoard.Api.Tests;

/// <summary>
/// Exercises the raw-SQL admin report end to end. This is why the test factory uses a
/// real in-memory SQLite database rather than the EF InMemory provider — FromSqlRaw
/// would not execute against InMemory.
/// </summary>
public class ReportsControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public ReportsControllerIntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetEventsReport_WithoutToken_ReturnsUnauthorized()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/api/reports/events");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetEventsReport_AsUser_ReturnsForbidden()
    {
        var client = await TestAuthHelper.CreateUserClientAsync(_factory);
        var response = await client.GetAsync("/api/reports/events");
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task GetEventsReport_AsAdmin_ReturnsAggregatedRows()
    {
        var client = await TestAuthHelper.CreateAdminClientAsync(_factory);
        var response = await client.GetAsync("/api/reports/events");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var rows = await response.Content.ReadFromJsonAsync<List<ReportRow>>();
        Assert.NotNull(rows);
        Assert.NotEmpty(rows!);
        // At least one seeded event has bookings, so total should be > 0 somewhere.
        Assert.Contains(rows!, r => r.TotalBookings > 0);
    }

    [Fact]
    public async Task GetEventsReport_AsAdmin_WithDateFilter_ReturnsOk()
    {
        var client = await TestAuthHelper.CreateAdminClientAsync(_factory);
        var from = DateTime.UtcNow.AddYears(-1).ToString("o");
        var to = DateTime.UtcNow.AddYears(1).ToString("o");

        var response = await client.GetAsync($"/api/reports/events?from={from}&to={to}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    private sealed class ReportRow
    {
        public int EventId { get; set; }
        public string Title { get; set; } = string.Empty;
        public int TotalBookings { get; set; }
        public int FavoritesCount { get; set; }
    }
}
