using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using WebService.Controllers;
using Xunit;

namespace IntegrationTests;

public class SearchHistoryTests : IClassFixture<WebApplicationFactory<SearchHistoryController>>, IClassFixture<WebApplicationFactory<SearchController>>, IAsyncLifetime
{
    private readonly WebApplicationFactory<SearchHistoryController> _factorySearchHistory;
    private readonly WebApplicationFactory<SearchController> _factorySearch;
    private HttpClient _httpClientSearchHistory = null!;
    private HttpClient _httpClientSearch = null!;
    
    public SearchHistoryTests(WebApplicationFactory<SearchHistoryController> factorySearchHistory, WebApplicationFactory<SearchController> factorySearch)
    {
        _factorySearchHistory = factorySearchHistory;
        _factorySearch = factorySearch;
    }
    
    public async ValueTask InitializeAsync()
    {
        _httpClientSearchHistory = _factorySearchHistory.CreateClient();
        _httpClientSearch = _factorySearch.CreateClient();

        var token = await GetToken();

        _httpClientSearchHistory.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);
        _httpClientSearch.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);
    }
    
    [Fact]
    public async Task Get_ReturnsOK_GetSearchHistory()
    {
        // Arrange
        var arrangeResponse = await _httpClientSearch.GetAsync($"/api/search?s=code&stype=0", TestContext.Current.CancellationToken);
        
        arrangeResponse.EnsureSuccessStatusCode();
        
        // Act
        var response = await _httpClientSearchHistory.GetAsync("/api/history/searches", TestContext.Current.CancellationToken);
        
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
    
    [Fact]
    public async Task Delete_ReturnsOK_DeleteAllBookmarks()
    {
        // Arrange
        var arrangeResponse1 = await _httpClientSearch.GetAsync($"/api/search?s=code&stype=0", TestContext.Current.CancellationToken);
        var arrangeResponse2 = await _httpClientSearch.GetAsync($"/api/search?s=app&stype=0", TestContext.Current.CancellationToken);
        var arrangeResponse3 = await _httpClientSearch.GetAsync($"/api/search?s=test&stype=0", TestContext.Current.CancellationToken);
        
        arrangeResponse1.EnsureSuccessStatusCode();
        arrangeResponse2.EnsureSuccessStatusCode();
        arrangeResponse3.EnsureSuccessStatusCode();
        
        // Act
        var response = await _httpClientSearchHistory.DeleteAsync($"/api/history/searches/delete/all", TestContext.Current.CancellationToken);
        
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
    
    public ValueTask DisposeAsync() => ValueTask.CompletedTask;

    private async Task<string> GetToken()
    {
        var response = await _httpClientSearchHistory.PostAsJsonAsync(
            "/api/auth/tokens",  
            new { username = "mazeettest", password = "testtest" }, 
            TestContext.Current.CancellationToken
        );
        using var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var token = doc.RootElement.GetProperty("token").GetString() ?? string.Empty;
        return token;
    }
}