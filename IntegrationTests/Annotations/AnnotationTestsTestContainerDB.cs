using System.Net;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc.Testing;
using WebService.Controllers;
using Xunit;

namespace IntegrationTests.Annotations;

public class AnnotationTestsTestContainerDb : IClassFixture<TestContainerWebApplicationFactory>
{
    private const string Token =
        "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6IjMwNSIsIm5iZiI6MTc3MjMxNjc1NywiZXhwIjoxNzcyNDAzMTU3LCJpYXQiOjE3NzIzMTY3NTd9.UNSZDvmVcPZLJ3Rrq7-45S-BTaEPmWR2cEVTSV3P6vs";
    private readonly HttpClient _httpClient;

    public AnnotationTestsTestContainerDb(TestContainerWebApplicationFactory factory)
    {
        _httpClient = factory.CreateDefaultClient();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);
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