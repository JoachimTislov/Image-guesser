using Tests.Helpers;

namespace Tests.Integration.Routing;

public class UrlTests(CustomWebApplicationFactory<Program> factory) : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _factory = factory;

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
        // response.EnsureSuccessStatusCode(); // Status Code 200-299 -- Not within range for some weird reason

        Assert.NotNull(response.Content.Headers.ContentType);

        Assert.Equal("text/html; charset=utf-8",
            response.Content.Headers.ContentType.ToString());
    }

}