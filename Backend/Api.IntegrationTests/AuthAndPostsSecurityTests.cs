using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace Api.IntegrationTests;

public class AuthAndPostsSecurityTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AuthAndPostsSecurityTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Viewer_Can_Get_Posts()
    {
        var token = await LoginAndGetToken("viewer@demo.local", "Viewer123!");

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var res = await _client.GetAsync("/api/posts");

        Assert.Equal(HttpStatusCode.OK, res.StatusCode);
    }

    private async Task<string> LoginAndGetToken(string identifier, string password)
    {
        var res = await _client.PostAsJsonAsync("/api/auth/login", new
        {
            identifier,
            password
        });

        Assert.Equal(HttpStatusCode.OK, res.StatusCode);

        var json = await res.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);

        return doc.RootElement.GetProperty("token").GetString()!;
    }
}