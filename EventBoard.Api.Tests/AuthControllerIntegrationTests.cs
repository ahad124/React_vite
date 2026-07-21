using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace EventBoard.Api.Tests;

public class AuthControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AuthControllerIntegrationTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Register_NewUser_ReturnsOk()
    {
        var request = new
        {
            userName = "New Tester",
            email = $"tester_{Guid.NewGuid():N}@example.com",
            password = "Password123"
        };

        var response = await _client.PostAsJsonAsync("/api/auth/register", request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Register_IgnoresClientSuppliedAdminRole_CreatesPlainUser()
    {
        // Even if a caller tries to inject an Admin role, the account must be a User
        // and therefore must NOT be allowed to hit an Admin-only endpoint.
        var email = $"sneaky_{Guid.NewGuid():N}@example.com";
        var register = await _client.PostAsJsonAsync("/api/auth/register", new
        {
            userName = "Sneaky",
            email,
            password = "Password123",
            role = "Admin"
        });
        Assert.Equal(HttpStatusCode.OK, register.StatusCode);

        var token = await TestAuthHelper.GetTokenAsync(_client, email, "Password123");
        var authed = _client;
        var req = new HttpRequestMessage(HttpMethod.Post, "/api/categories")
        {
            Content = JsonContent.Create(new { name = "ShouldNotWork" })
        };
        req.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await authed.SendAsync(req);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Register_DuplicateEmail_ReturnsConflict()
    {
        var request = new
        {
            userName = "Dup",
            email = TestAuthHelper.AdminEmail, // already seeded
            password = "Password123"
        };

        var response = await _client.PostAsJsonAsync("/api/auth/register", request);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task Register_InvalidModel_ReturnsBadRequest()
    {
        var request = new { userName = "x", email = "not-an-email", password = "123" };

        var response = await _client.PostAsJsonAsync("/api/auth/register", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Login_ValidCredentials_ReturnsToken()
    {
        var response = await _client.PostAsJsonAsync("/api/auth/login", new
        {
            email = TestAuthHelper.AdminEmail,
            password = TestAuthHelper.AdminPassword
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<Dictionary<string, object>>();
        Assert.NotNull(body);
        Assert.True(body!.ContainsKey("token"));
    }

    [Fact]
    public async Task Login_WrongPassword_ReturnsUnauthorized()
    {
        var response = await _client.PostAsJsonAsync("/api/auth/login", new
        {
            email = TestAuthHelper.AdminEmail,
            password = "wrong-password"
        });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Login_UnknownEmail_ReturnsUnauthorized()
    {
        var response = await _client.PostAsJsonAsync("/api/auth/login", new
        {
            email = "nobody@example.com",
            password = "whatever"
        });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
