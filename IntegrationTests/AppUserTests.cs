using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Web.Controllers;
using Web.DTOs;
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
    
    [Fact]
    public async Task User_Signup_Bad_Request_AlreadySignedUp()
    {
        var signupUser = new SignupUserDto
        {
            Username = "testanno",
            Password = "12345678"
        };
        
        var response = await _httpClient.PostAsJsonAsync("/api/auth/users", signupUser, TestContext.Current.CancellationToken);
        
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    
    [Fact]
    public async Task User_login()
    {
        var user = new LoginUserDto
        {
            Username = "testanno",
            Password = "12345678"
        };
        
        var response = await _httpClient.PostAsJsonAsync("/api/auth/tokens", user, TestContext.Current.CancellationToken);
        
        response.EnsureSuccessStatusCode();

        var authenticatedUser = await response.Content.ReadFromJsonAsync<AuthenticatedUser>(TestContext.Current.CancellationToken);
        
        Assert.NotNull(authenticatedUser);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(user.Username, authenticatedUser.Username);
        Assert.NotNull(authenticatedUser.Token);

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