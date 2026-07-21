using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace EventBoard.Api.Tests;

/// <summary>
/// Shared helpers for integration tests: logs in through the real /api/auth/login
/// endpoint (which also exercises AuthController + JwtTokenService) and returns a
/// client with the bearer token attached. Credentials come from the seeded users
/// in <see cref="EventBoard.Api.Data.DbInitializer"/>.
/// </summary>
public static class TestAuthHelper
{
    public const string AdminEmail = "admin@eventboard.com";
    public const string AdminPassword = "Admin123!";
    public const string UserEmail = "alice@example.com";
    public const string UserPassword = "Alice123!";

    public static async Task<string> GetTokenAsync(HttpClient client, string email, string password)
    {
        var response = await client.PostAsJsonAsync("/api/auth/login", new { email, password });
        response.EnsureSuccessStatusCode();

        using var stream = await response.Content.ReadAsStreamAsync();
        using var doc = await JsonDocument.ParseAsync(stream);
        return doc.RootElement.GetProperty("token").GetString()!;
    }

    public static async Task<HttpClient> CreateAdminClientAsync(CustomWebApplicationFactory factory)
    {
        var client = factory.CreateClient();
        var token = await GetTokenAsync(client, AdminEmail, AdminPassword);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return client;
    }

    public static async Task<HttpClient> CreateUserClientAsync(CustomWebApplicationFactory factory)
    {
        var client = factory.CreateClient();
        var token = await GetTokenAsync(client, UserEmail, UserPassword);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return client;
    }
}
