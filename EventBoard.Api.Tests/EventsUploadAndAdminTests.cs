using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using Xunit;

namespace EventBoard.Api.Tests;

/// <summary>
/// Admin-authenticated Events tests, including the image-upload magic-byte validation
/// (UAT-FILE-001) and the full create/update/delete lifecycle.
/// </summary>
public class EventsUploadAndAdminTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    // Minimal valid PNG header (8-byte signature + a few IHDR bytes) — enough for the
    // 12-byte magic-number check the upload endpoint performs.
    private static readonly byte[] PngBytes =
    {
        0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A,
        0x00, 0x00, 0x00, 0x0D, 0x49, 0x48, 0x44, 0x52
    };

    public EventsUploadAndAdminTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    private static MultipartFormDataContent BuildFileContent(byte[] bytes, string fileName, string contentType)
    {
        var fileContent = new ByteArrayContent(bytes);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
        return new MultipartFormDataContent { { fileContent, "file", fileName } };
    }

    [Fact]
    public async Task GetEventsByCategory_ReturnsOk()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/api/events/category/1");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task UploadImage_ValidPng_ReturnsOkWithUrl()
    {
        var client = await TestAuthHelper.CreateAdminClientAsync(_factory);
        var content = BuildFileContent(PngBytes, "photo.png", "image/png");

        var response = await client.PostAsync("/api/events/upload-image", content);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
        Assert.NotNull(body);
        Assert.StartsWith("/uploads/", body!["imageUrl"]);
    }

    [Fact]
    public async Task UploadImage_FakeImage_RenamedToPng_ReturnsBadRequest()
    {
        // The heart of the deep file-type check: a non-image whose name/content-type
        // claim to be a PNG must be rejected because its bytes are not a PNG.
        var client = await TestAuthHelper.CreateAdminClientAsync(_factory);
        var evilBytes = Encoding.ASCII.GetBytes("#!/bin/sh\nrm -rf / # definitely not an image");
        var content = BuildFileContent(evilBytes, "photo.png", "image/png");

        var response = await client.PostAsync("/api/events/upload-image", content);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UploadImage_DisallowedExtension_ReturnsBadRequest()
    {
        var client = await TestAuthHelper.CreateAdminClientAsync(_factory);
        var content = BuildFileContent(PngBytes, "payload.exe", "image/png");

        var response = await client.PostAsync("/api/events/upload-image", content);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UploadImage_WithoutToken_ReturnsUnauthorized()
    {
        var client = _factory.CreateClient();
        var content = BuildFileContent(PngBytes, "photo.png", "image/png");

        var response = await client.PostAsync("/api/events/upload-image", content);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreateUpdateDelete_AsAdmin_FullLifecycle()
    {
        var client = await TestAuthHelper.CreateAdminClientAsync(_factory);

        // Organizer must be the seeded admin's id; fetch it from an existing event.
        var existing = await client.GetFromJsonAsync<List<EventResponse>>("/api/events");
        var organizerId = existing!.First().OrganizerId;

        // Create
        var create = await client.PostAsJsonAsync("/api/events", new
        {
            title = "Lifecycle Test Event",
            description = "created by integration test",
            date = DateTime.UtcNow.AddDays(30),
            location = "Test City, TC",
            imageUrl = "",
            categoryId = 1,
            organizerId
        });
        Assert.Equal(HttpStatusCode.Created, create.StatusCode);
        var created = await create.Content.ReadFromJsonAsync<EventResponse>();
        Assert.NotNull(created);

        // Read back
        var getOne = await client.GetAsync($"/api/events/{created!.Id}");
        Assert.Equal(HttpStatusCode.OK, getOne.StatusCode);

        // Update
        var update = await client.PutAsJsonAsync($"/api/events/{created.Id}", new
        {
            title = "Lifecycle Test Event (updated)"
        });
        Assert.Equal(HttpStatusCode.OK, update.StatusCode);

        // Delete
        var delete = await client.DeleteAsync($"/api/events/{created.Id}");
        Assert.Equal(HttpStatusCode.NoContent, delete.StatusCode);
    }

    [Fact]
    public async Task UpdateEvent_AsAdmin_UnknownId_ReturnsNotFound()
    {
        var client = await TestAuthHelper.CreateAdminClientAsync(_factory);
        var response = await client.PutAsJsonAsync("/api/events/999999", new { title = "x" });
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteEvent_AsAdmin_UnknownId_ReturnsNotFound()
    {
        var client = await TestAuthHelper.CreateAdminClientAsync(_factory);
        var response = await client.DeleteAsync("/api/events/999999");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private sealed class EventResponse
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public Guid OrganizerId { get; set; }
    }
}
