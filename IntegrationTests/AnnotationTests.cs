using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Domain.DTO;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc.Testing;
using Web.Controllers;
using Xunit;

namespace IntegrationTests;

public class AnnotationTests : IClassFixture<WebApplicationFactory<AnnotationsController>>, IAsyncLifetime
{
    private readonly WebApplicationFactory<AnnotationsController> _factory;
    private HttpClient _httpClient = null!;
    
    public AnnotationTests(WebApplicationFactory<AnnotationsController> factory)
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
    public async Task Get_ReturnsOK_GetAllUserAnnotationsMadeOnPostId()
    {
        // Arrange
        const int postId = 19;
        
        var annotation = new AnnotationsDto()
        {
            PostId = postId,
            Body = "Test annotation",
        };
        
        var arrangeResponse = await _httpClient.PostAsJsonAsync("/api/annotations", annotation, TestContext.Current.CancellationToken);
        
        arrangeResponse.EnsureSuccessStatusCode();
        
        // Act
        var response = await _httpClient.GetAsync($"/api/annotations/post/{postId}", TestContext.Current.CancellationToken);
        
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
    
    [Fact]
    public async Task Get_ReturnsOK_GetAllAnnotationsOfUser()
    {
        // Arrange
        
        // Act
        var response = await _httpClient.GetAsync($"/api/annotations/user", TestContext.Current.CancellationToken);
        
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
    
    [Fact]
    public async Task Get_ReturnsOK_GetAnyAnnotationById()
    {
        // Arrange
        const int postId = 19;
        
        var annotation = new AnnotationsDto()
        {
            PostId = postId,
            Body = "Test annotation",
        };
        
        var arrangeResponse = await _httpClient.PostAsJsonAsync("/api/annotations", annotation, TestContext.Current.CancellationToken);
        arrangeResponse.EnsureSuccessStatusCode();
        
        var annotationsDto = await arrangeResponse.Content.ReadFromJsonAsync<AnnotationsDto>(cancellationToken: TestContext.Current.CancellationToken); 
        var annotationId = annotationsDto?.AnnotationId;
        
        // Act
        var response = await _httpClient.GetAsync($"/api/annotations/{annotationId}", TestContext.Current.CancellationToken);
        
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
    
    [Fact]
    public async Task Put_ReturnsOK_UpdateAnnotation()
    {
        // Arrange
        const int postId = 19;
        
        var annotation = new AnnotationsDto()
        {
            PostId = postId,
            Body = "Test annotation",
        };
        
        var arrangeResponse = await _httpClient.PostAsJsonAsync("/api/annotations", annotation, TestContext.Current.CancellationToken);
        arrangeResponse.EnsureSuccessStatusCode();
        
        var annotationsDto = await arrangeResponse.Content.ReadFromJsonAsync<AnnotationsDto>(cancellationToken: TestContext.Current.CancellationToken);
        Assert.NotNull(annotationsDto);
        
        annotationsDto.Body = "Updated test annotation";
        
        // Act
        var response = await _httpClient.PutAsJsonAsync($"/api/annotations/{annotationsDto.AnnotationId}", annotationsDto, TestContext.Current.CancellationToken);
        
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
    
    [Fact]
    public async Task Get_ReturnOk_ForExistingAnnotations()
    {
        // Arrange
        var annotation = new AnnotationsDto()
        {
            PostId = 19,
            Body = "Second test annotation",
        };

        var arrangeResponse = await _httpClient.PostAsJsonAsync("/api/annotations", annotation, TestContext.Current.CancellationToken);
        
        arrangeResponse.EnsureSuccessStatusCode();
        
        var createdAnnotation = await arrangeResponse.Content.ReadFromJsonAsync<AnnotationsDto>(TestContext.Current.CancellationToken);
        
        // Act
        var response = await _httpClient.GetAsync($"/api/annotations/{createdAnnotation!.AnnotationId}", TestContext.Current.CancellationToken);
        
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
    
    [Fact]
    public async Task Delete_ReturnsOK_DeleteAnnotation()
    {
        // Arrange
        const int postId = 19;
        
        var annotation = new AnnotationsDto()
        {
            PostId = postId,
            Body = "Test annotation",
        };
        
        var arrangeResponse = await _httpClient.PostAsJsonAsync("/api/annotations", annotation, TestContext.Current.CancellationToken);
        arrangeResponse.EnsureSuccessStatusCode();
        
        var annotationsDto = await arrangeResponse.Content.ReadFromJsonAsync<AnnotationsDto>(cancellationToken: TestContext.Current.CancellationToken); 
        var annotationId = annotationsDto?.AnnotationId;
        
        // Act
        var response = await _httpClient.DeleteAsync($"/api/annotations/{annotationId}", TestContext.Current.CancellationToken);
        
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
    
    [Fact]
    public async Task Get_ReturnsOK_ForExistingAnnotation()
    {
        // Arrange
        const string validId = "42";
        
        // Act
        var response = await _httpClient.GetAsync($"/api/annotations/{validId}", TestContext.Current.CancellationToken);
        
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Get_ReturnsNotFound_ForMissingAnnotation()
    {
        // Arrange
        const string invalidId = "422";
        
        // Act
        var response = await _httpClient.GetAsync($"/api/annotations/{invalidId}", TestContext.Current.CancellationToken);
        
        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
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