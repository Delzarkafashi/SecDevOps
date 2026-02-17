using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace Api.IntegrationTests;

public class Posts_AdminTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public Posts_AdminTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Admin_Can_Delete_Post()
    {
        Auth(await Login("admin@demo.local", "Admin123!"));

        var create = await _client.PostAsJsonAsync("/api/posts", new { title = "a", content = "a" });
        var json = JsonDocument.Parse(await create.Content.ReadAsStringAsync());
        var id = json.RootElement.GetProperty("id").GetInt64();

        var del = await _client.DeleteAsync($"/api/posts/{id}");
        Assert.Equal(HttpStatusCode.NoContent, del.StatusCode);
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