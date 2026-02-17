using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace Api.IntegrationTests;

public class Auth_LoginTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public Auth_LoginTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Login_Fails_With_Wrong_Password()
    {
        var res = await _client.PostAsJsonAsync("/api/auth/login", new
        {
            identifier = "viewer@demo.local",
            password = "fel"
        });

        Assert.Equal(HttpStatusCode.Unauthorized, res.StatusCode);
    }
}