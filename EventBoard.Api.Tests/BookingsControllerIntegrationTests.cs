using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace EventBoard.Api.Tests;

public class BookingsControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public BookingsControllerIntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetMyBookings_WithoutToken_ReturnsUnauthorized()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/api/bookings/my");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetMyBookings_AsUser_ReturnsOk()
    {
        var client = await TestAuthHelper.CreateUserClientAsync(_factory);
        var response = await client.GetAsync("/api/bookings/my");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetAllBookings_AsUser_ReturnsForbidden()
    {
        var client = await TestAuthHelper.CreateUserClientAsync(_factory);
        var response = await client.GetAsync("/api/bookings");
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task GetAllBookings_AsAdmin_ReturnsOk()
    {
        var client = await TestAuthHelper.CreateAdminClientAsync(_factory);
        var response = await client.GetAsync("/api/bookings");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetEventBookings_AsAdmin_ReturnsOk()
    {
        var client = await TestAuthHelper.CreateAdminClientAsync(_factory);
        var response = await client.GetAsync("/api/bookings/event/1");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task BookEvent_UnknownEvent_ReturnsNotFound()
    {
        var client = await TestAuthHelper.CreateUserClientAsync(_factory);
        var response = await client.PostAsJsonAsync("/api/bookings", new { eventId = 999999 });
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task BookEvent_NewEvent_ThenDuplicate_ReturnsCreatedThenBadRequest()
    {
        var client = await TestAuthHelper.CreateUserClientAsync(_factory);

        // Event 5 has no seeded booking for alice.
        var first = await client.PostAsJsonAsync("/api/bookings", new { eventId = 5 });
        Assert.Equal(HttpStatusCode.Created, first.StatusCode);

        var created = await first.Content.ReadFromJsonAsync<BookingResponse>();
        Assert.NotNull(created);

        // Owner can read their own booking.
        var getById = await client.GetAsync($"/api/bookings/{created!.Id}");
        Assert.Equal(HttpStatusCode.OK, getById.StatusCode);

        // Booking the same event again is rejected.
        var second = await client.PostAsJsonAsync("/api/bookings", new { eventId = 5 });
        Assert.Equal(HttpStatusCode.BadRequest, second.StatusCode);

        // Admin can update its status.
        var admin = await TestAuthHelper.CreateAdminClientAsync(_factory);
        var update = await admin.PutAsJsonAsync($"/api/bookings/{created.Id}/status", new { status = "Confirmed" });
        Assert.Equal(HttpStatusCode.NoContent, update.StatusCode);
    }

    [Fact]
    public async Task UpdateBookingStatus_UnknownBooking_ReturnsNotFound()
    {
        var admin = await TestAuthHelper.CreateAdminClientAsync(_factory);
        var response = await admin.PutAsJsonAsync("/api/bookings/999999/status", new { status = "Confirmed" });
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private sealed class BookingResponse
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
