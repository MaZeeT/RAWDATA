using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Domain.DTO;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
    public async Task Add_New_Annotation_To_Post_Question()
    {
        var url = "/api/annotations";
        
        var annotation = new AnnotationsDto
        {
            PostId = 7284,
            Body = "This is annotation 7284 made using unit test."
        };
        
        var response = await _httpClient.PostAsJsonAsync(url, annotation, TestContext.Current.CancellationToken);
        
        response.EnsureSuccessStatusCode();

        var testAnnotation = await response.Content.ReadFromJsonAsync<AnnotationsDto>(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains(url, testAnnotation.URL);
        Assert.Equal(annotation.Body, testAnnotation.Body);

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

    [Fact]
    public async Task Update_Annotation()
    {
        var annotToUpdate = new AnnotationsDto
        {
            Body = "This is new annotation body for the 2nd annotation made on post with id 7284"
        };
        
        var annotation = new AnnotationsDto
        {
            PostId = 7284,
            Body = "This is another annotation 7284 made using unit test."
        };

        var postResponse = await _httpClient.PostAsJsonAsync("/api/annotations", annotation, TestContext.Current.CancellationToken);
        
        postResponse.EnsureSuccessStatusCode();
        
        var testAnnotation = await postResponse.Content.ReadFromJsonAsync<AnnotationsDto>(TestContext.Current.CancellationToken);
        
        Assert.NotNull(testAnnotation);
        
        var putResponse = await _httpClient.PutAsJsonAsync($"/api/annotations/{testAnnotation.AnnotationId}", annotToUpdate, TestContext.Current.CancellationToken);
        
        Assert.Equal(HttpStatusCode.OK, postResponse.StatusCode);
        Assert.Equal(HttpStatusCode.OK, putResponse.StatusCode);
    }
    
    [Fact]
    public async Task Delete_Annotation_By_Id()
    {
        var annotation = new AnnotationsDto
        {
            PostId = 7284,
            Body = "This is another annotation 7284 made using unit test."
        };
        
        var postResponse = await _httpClient.PostAsJsonAsync("/api/annotations", annotation, TestContext.Current.CancellationToken);
        
        postResponse.EnsureSuccessStatusCode();
        
        var testAnnotation = await postResponse.Content.ReadFromJsonAsync<AnnotationsDto>(TestContext.Current.CancellationToken);
        
        Assert.NotNull(testAnnotation);
        
        var deleteResponse = await _httpClient.DeleteAsync($"/api/annotations/{testAnnotation.AnnotationId}", TestContext.Current.CancellationToken);
        
        Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);
    }
    
    [Fact (Skip = "Broken test data – needs isolation")]
    public async Task Get_All_Annotations_Of_User_By_PostId()
    {
        var response = await _httpClient.GetAsync("/api/annotations/post/39512", TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var data = await response.Content.ReadFromJsonAsync<List<SimpleAnnotationDto>>(cancellationToken: TestContext.Current.CancellationToken);
        
        Assert.NotNull(data);
        Assert.NotNull(data[0]);
        
        Assert.Equal("This is annotation 7284 made using unit test.", data[0].Body);
    }
    
    
    [Fact]
    public async Task Get_All_Annotations_Of_User()
    {
        var response = await _httpClient.GetAsync("/api/annotations/user", TestContext.Current.CancellationToken);
        
        response.EnsureSuccessStatusCode();

        var data = response.Content.ReadAsStringAsync().Result;
        var obj = JObject.Parse(data);

        var noOfPages = (string)obj["numberOfPages"];
        var previousPageUrl = (string)obj["prev"];
        var nextPageUrl = (string)obj["next"];
        var itemsList = (JArray)obj["items"];

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Null(previousPageUrl);
        Assert.NotNull(nextPageUrl);
        Assert.Equal("22", noOfPages);

        var firstItem = (JObject)itemsList[0];
        var body = (string)firstItem["body"];
        var postId = (string)firstItem["postId"];
        var questionId = (string)firstItem["questionId"];

        Assert.Equal("Updated test annotation", body);
        Assert.Equal("39512", postId);
        Assert.Equal("19", questionId);
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
}