using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Web.Controllers;
using Xunit;

namespace IntegrationTests;

public class BookmarkTests : IClassFixture<WebApplicationFactory<BookmarkController>>, IAsyncLifetime
{
    private readonly WebApplicationFactory<BookmarkController> _factory;
    private HttpClient _httpClient = null!;
    
    public BookmarkTests(WebApplicationFactory<BookmarkController> factory)
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
    public async Task Get_ReturnsOK_GetBookmarkList()
    {
        // Arrange
        const int postId = 19;
        
        var arrangeResponse = await _httpClient.PostAsync($"/api/bookmark/add/{postId}", null, TestContext.Current.CancellationToken);
        
        arrangeResponse.EnsureSuccessStatusCode();
        
        // Act
        var response = await _httpClient.GetAsync($"/api/bookmark?Page=1&PageSize=5", TestContext.Current.CancellationToken);
        
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
    
    [Fact]
    public async Task Post_ReturnsOK_AddBookmark()
    {
        // Arrange
        const int postId = 19;
        
        // Act
        var response = await _httpClient.PostAsync($"/api/bookmark/add/{postId}", null, TestContext.Current.CancellationToken);
      
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
    
    [Fact]
    public async Task Delete_ReturnsOK_DeleteBookmark()
    {
        // Arrange
        const int postId = 19;
        
        var arrangeResponse = await _httpClient.PostAsync($"/api/bookmark/add/{postId}", null, TestContext.Current.CancellationToken);
        
        arrangeResponse.EnsureSuccessStatusCode();
        
        // Act
        var response = await _httpClient.DeleteAsync($"/api/bookmark/delete/{postId}", TestContext.Current.CancellationToken);
        
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
    
    
    [Fact]
    public async Task Delete_ReturnsOK_DeleteAllBookmarks()
    {
        // Arrange
        const int postId = 19;
        
        var arrangeResponse = await _httpClient.PostAsync($"/api/bookmark/add/{postId}", null, TestContext.Current.CancellationToken);
        
        arrangeResponse.EnsureSuccessStatusCode();
        
        // Act
        var response = await _httpClient.DeleteAsync($"/api/bookmark/delete/all", TestContext.Current.CancellationToken);
        
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