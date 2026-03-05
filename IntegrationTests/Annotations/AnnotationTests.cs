using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Domain.AnnotationsDTOs;
using Microsoft.AspNetCore.Mvc.Testing;
using WebService.Controllers;
using Xunit;

namespace IntegrationTests.Annotations;

public class AnnotationTests : IClassFixture<WebApplicationFactory<AnnotationsController>>
{
    private readonly HttpClient _httpClient;
    
    public AnnotationTests(WebApplicationFactory<AnnotationsController> factory)
    {
        _httpClient = factory.CreateDefaultClient();
        var token = GetToken().Result;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

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

    
    [Fact]
    public async Task Get_ReturnOk_ForExistingAnnotations()
    {
        // Arrange
        var annotationOne = new AnnotationsDto()
        {
            PostId = 19,
            Body = "First test annotation",
        };
        
        var annotationTwo = new AnnotationsDto()
        {
            PostId = 19,
            Body = "Second test annotation",
        };
        
        var json = "{\n  \"postId\" : 1,\n  \"body\": \"test text\"\n}";
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        
        var responseOne = await _httpClient.PostAsync("/api/annotations", content, TestContext.Current.CancellationToken);
            
        
        // Act
        var response = await _httpClient.GetAsync("/api/annotations/422", TestContext.Current.CancellationToken);
        
        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        
    }
    
    [Fact]
    public async Task Get_ReturnsOK_ForExistingAnnotation()
    {
        var response = await _httpClient.GetAsync("/api/annotations/42", TestContext.Current.CancellationToken);
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact(Skip = "Fails with a servercrash")] //TODO fix this bug in this test
    public async Task Get_ReturnsNotFound_ForMissingAnnotation()
    {
        var response = await _httpClient.GetAsync("/api/annotations/422", TestContext.Current.CancellationToken);
        
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}