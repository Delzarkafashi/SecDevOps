using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace Api.IntegrationTests;

public class Posts_ViewerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public Posts_ViewerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Viewer_Can_Read_Posts()
    {
        Auth(await Login("viewer@demo.local", "Viewer123!"));
        var res = await _client.GetAsync("/api/posts");
        Assert.Equal(HttpStatusCode.OK, res.StatusCode);
    }

    [Fact]
    public async Task Viewer_Cannot_Create_Post()
    {
        Auth(await Login("viewer@demo.local", "Viewer123!"));
        var res = await _client.PostAsJsonAsync("/api/posts", new { title = "x", content = "x" });
        Assert.Equal(HttpStatusCode.Forbidden, res.StatusCode);
    }

    private async Task<string> Login(string id, string pw)
    {
        var r = await _client.PostAsJsonAsync("/api/auth/login", new { identifier = id, password = pw });
        var json = JsonDocument.Parse(await r.Content.ReadAsStringAsync());
        return json.RootElement.GetProperty("token").GetString()!;
    }

    private void Auth(string token)
    {
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }
}