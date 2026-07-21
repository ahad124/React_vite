using System.Net;
using Xunit;

namespace EventBoard.Api.Tests;

public class FavoritesControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public FavoritesControllerIntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetMyFavorites_WithoutToken_ReturnsUnauthorized()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/api/favorites");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetMyFavorites_AsUser_ReturnsOk()
    {
        var client = await TestAuthHelper.CreateUserClientAsync(_factory);
        var response = await client.GetAsync("/api/favorites");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task ToggleFavorite_UnknownEvent_ReturnsNotFound()
    {
        var client = await TestAuthHelper.CreateUserClientAsync(_factory);
        var response = await client.PostAsync("/api/favorites/999999", null);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task ToggleFavorite_AddThenRemove_TogglesState()
    {
        var client = await TestAuthHelper.CreateUserClientAsync(_factory);

        // Event 4 is not in alice's seeded favorites -> first toggle adds it.
        var add = await client.PostAsync("/api/favorites/4", null);
        Assert.Equal(HttpStatusCode.OK, add.StatusCode);

        // Second toggle removes it.
        var remove = await client.PostAsync("/api/favorites/4", null);
        Assert.Equal(HttpStatusCode.OK, remove.StatusCode);
    }

    [Fact]
    public async Task RemoveFavorite_NotFavorited_ReturnsNotFound()
    {
        var client = await TestAuthHelper.CreateUserClientAsync(_factory);
        var response = await client.DeleteAsync("/api/favorites/999999");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task RemoveFavorite_Existing_ReturnsNoContent()
    {
        var client = await TestAuthHelper.CreateUserClientAsync(_factory);

        // Add event 6 to favorites, then remove it via DELETE.
        var add = await client.PostAsync("/api/favorites/6", null);
        Assert.Equal(HttpStatusCode.OK, add.StatusCode);

        var response = await client.DeleteAsync("/api/favorites/6");
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }
}
