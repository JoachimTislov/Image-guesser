

using Microsoft.AspNetCore.Mvc.Testing;

namespace Tests.Integration.Routing;

public class UrlTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory = factory;


    [Theory]
    [InlineData("/")]
    [InlineData("/Auth/Login")]
    [InlineData("/Auth/Register")]
    [InlineData("/Error")]
    [InlineData("/Profile")]
    public async Task Get_EndpointsReturnSuccessAndCorrectContentType(string url)
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync(url);

        // Assert
        response.EnsureSuccessStatusCode(); // Status Code 200-299

        Assert.NotNull(response.Content.Headers.ContentType);

        Assert.Equal("text/html; charset=utf-8",
            response.Content.Headers.ContentType.ToString());
    }

}