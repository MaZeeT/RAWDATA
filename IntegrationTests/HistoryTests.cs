using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using WebService.Controllers;
using Xunit;

namespace IntegrationTests;

public class HistoryTests : IClassFixture<WebApplicationFactory<HistoryController>>, IClassFixture<WebApplicationFactory<QuestionsController>>, IAsyncLifetime
{
    private readonly WebApplicationFactory<HistoryController> _historyFactory;
    private readonly WebApplicationFactory<QuestionsController> _questionFactory;
    private HttpClient _httpClientHistory = null!;
    private HttpClient _httpClientQuestion = null!;
    
    public HistoryTests(WebApplicationFactory<HistoryController> historyFactory, WebApplicationFactory<QuestionsController> questionFactory)
    {
        _historyFactory = historyFactory;
        _questionFactory = questionFactory;
    }
    
    public async ValueTask InitializeAsync()
    {
        _httpClientHistory = _historyFactory.CreateClient();
        _httpClientQuestion = _questionFactory.CreateClient();

        var token = await GetToken();

        _httpClientHistory.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);
        _httpClientQuestion.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);
    }
    
    [Fact]
    public async Task Get_ReturnsOK_GetHistory()
    {
        // Arrange
        const int postId = 19;
        
        var arrangeResponse = await _httpClientQuestion.GetAsync($"/api/questions/thread/{postId}", TestContext.Current.CancellationToken);
        
        arrangeResponse.EnsureSuccessStatusCode();
        
        // Act
        var response = await _httpClientHistory.GetAsync($"/api/history", TestContext.Current.CancellationToken);
        
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
    
    [Fact]
    public async Task Delete_ReturnsOK_ClearHistory()
    {
        // Arrange
        const int postId = 19;
        
        var arrangeResponse = await _httpClientQuestion.GetAsync($"/api/questions/thread/{postId}", TestContext.Current.CancellationToken);
        
        arrangeResponse.EnsureSuccessStatusCode();
        
        // Act
        var response = await _httpClientHistory.DeleteAsync($"/api/history/delete/all", TestContext.Current.CancellationToken);
        
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
    
    
    public ValueTask DisposeAsync() => ValueTask.CompletedTask;

    private async Task<string> GetToken()
    {
        var response = await _httpClientQuestion.PostAsJsonAsync(
            "/api/auth/tokens",  
            new { username = "mazeettest", password = "testtest" }, 
            TestContext.Current.CancellationToken
        );
        using var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var token = doc.RootElement.GetProperty("token").GetString() ?? string.Empty;
        return token;
    }
}