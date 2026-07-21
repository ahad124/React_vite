using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace EventBoard.Api.Tests;

public class CategoriesControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public CategoriesControllerIntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetCategories_Anonymous_ReturnsOk()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/api/categories");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task CreateCategory_WithoutToken_ReturnsUnauthorized()
    {
        var client = _factory.CreateClient();
        var response = await client.PostAsJsonAsync("/api/categories", new { name = "Nope" });
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreateCategory_AsUser_ReturnsForbidden()
    {
        var client = await TestAuthHelper.CreateUserClientAsync(_factory);
        var response = await client.PostAsJsonAsync("/api/categories", new { name = "Nope" });
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task CreateCategory_AsAdmin_UniqueName_ReturnsCreated()
    {
        var client = await TestAuthHelper.CreateAdminClientAsync(_factory);
        var name = $"Category_{Guid.NewGuid():N}";
        var response = await client.PostAsJsonAsync("/api/categories", new { name });
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task CreateCategory_AsAdmin_DuplicateName_ReturnsConflict()
    {
        var client = await TestAuthHelper.CreateAdminClientAsync(_factory);
        var response = await client.PostAsJsonAsync("/api/categories", new { name = "Conference" });
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task DeleteCategory_AsAdmin_NonExisting_ReturnsNotFound()
    {
        var client = await TestAuthHelper.CreateAdminClientAsync(_factory);
        var response = await client.DeleteAsync("/api/categories/999999");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteCategory_AsAdmin_CreatedThenDeleted_ReturnsNoContent()
    {
        var client = await TestAuthHelper.CreateAdminClientAsync(_factory);
        var name = $"Temp_{Guid.NewGuid():N}";
        var created = await client.PostAsJsonAsync("/api/categories", new { name });
        var dto = await created.Content.ReadFromJsonAsync<CategoryResponse>();

        var response = await client.DeleteAsync($"/api/categories/{dto!.Id}");
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    private sealed class CategoryResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
