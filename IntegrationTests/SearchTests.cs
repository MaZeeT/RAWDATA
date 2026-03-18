using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using WebService.Controllers;
using Xunit;

namespace IntegrationTests;

public class SearchTests : IClassFixture<WebApplicationFactory<SearchController>>, IAsyncLifetime
{
    private readonly WebApplicationFactory<SearchController> _factory;
    private HttpClient _httpClient = null!;
    
    public SearchTests(WebApplicationFactory<SearchController> factory)
    {
        _factory = factory;
    }
    
    public async ValueTask InitializeAsync()
    {
        _httpClient = _factory.CreateClient();

        var token = await GetToken();

        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);
    }
    
    [Fact]
    public async Task Get_ReturnsOK_WordRank()
    {
        // Arrange
        
        // Act
        var response = await _httpClient.GetAsync($"/api/search/wordrank?s=code&stype=5", TestContext.Current.CancellationToken);
        
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
    
    [Fact]
    public async Task Get_ReturnsOK_Search()
    {
        // Arrange
        
        // Act
        var response = await _httpClient.GetAsync($"/api/search?s=code&stype=0", TestContext.Current.CancellationToken);
        
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
    
    public ValueTask DisposeAsync() => ValueTask.CompletedTask;

    private async Task<string> GetToken()
    {
        var response = await _httpClient.PostAsJsonAsync(
            "/api/auth/tokens",  
            new { username = "mazeettest", password = "testtest" }, 
            TestContext.Current.CancellationToken
        );
        using var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var token = doc.RootElement.GetProperty("token").GetString() ?? string.Empty;
        return token;
    }
}