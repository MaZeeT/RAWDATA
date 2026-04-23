using Application.Interfaces.Services;
using Domain.DTO;
using Microsoft.Extensions.DependencyInjection;
using UnitTests.Fixtures;
using Xunit;

namespace UnitTests.ServiceTests;

public class AnnotationServiceTests : IClassFixture<ServiceProviderFixture>
{
    private readonly IAnnotationService _sut;
    private const int TestUserId = 12;
    
    public AnnotationServiceTests(ServiceProviderFixture fixture)
    {
        _sut = fixture.ServiceProvider.GetRequiredService<IAnnotationService>();
    }
    
    [Fact]
    public void InitialTest()
    {
        var s = new AnnotationsDto();
        _sut.CreateAnnotation(s, out var newId);
        
        Assert.Equal(-1, newId);
    }
    
    [Fact]
    public void AddAnnotation_Successfully()
    {
        var annotation = new AnnotationsDto
        {
            UserId = TestUserId,
            PostId = 19,
            Body = "AddAnnotation_Successfully test",
            Date = new DateTime(2025, 11, 11, 11,11,11,11,DateTimeKind.Utc)
        };
        
        _sut.CreateAnnotation(annotation,  out var result);
        
        Assert.True(result > 0);
    }
}