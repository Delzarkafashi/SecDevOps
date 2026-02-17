using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace Api.IntegrationTests;

public class Posts_StaffTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public Posts_StaffTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Staff_Can_Create_Post()
    {
        Auth(await Login("staff@demo.local", "Staff123!"));
        var res = await _client.PostAsJsonAsync("/api/posts", new { title = "staff", content = "ok" });
        Assert.Equal(HttpStatusCode.OK, res.StatusCode);
    }

    [Fact]
    public async Task Staff_Cannot_Delete_Post()
    {
        Auth(await Login("staff@demo.local", "Staff123!"));
        var res = await _client.DeleteAsync("/api/posts/1");
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