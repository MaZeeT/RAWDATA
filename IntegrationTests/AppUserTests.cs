using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Domain.AnnotationsDTOs;
using Microsoft.AspNetCore.Mvc.Testing;
using WebService.Controllers;
using Xunit;

namespace IntegrationTests;

public class AppUserTests : IClassFixture<WebApplicationFactory<AppUserController>>, IAsyncLifetime
{
    private readonly WebApplicationFactory<AppUserController> _factory;
    private HttpClient _httpClient = null!;
    
    private const int TestUserId = 305;
    
    public AppUserTests(WebApplicationFactory<AppUserController> factory)
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
    public async Task Get_ReturnsOK_GetAppUser()
    {
        // Arrange
        
        // Act
        var response = await _httpClient.GetAsync($"/api/appuser?id={TestUserId}", TestContext.Current.CancellationToken);
        
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