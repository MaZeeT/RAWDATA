using Application.Interfaces.Services;
using Domain.DTO;
using Microsoft.Extensions.DependencyInjection;
using UnitTests.Fixtures;
using Xunit;

namespace UnitTests.ServiceTests;

public class AnnotationServiceTests : IClassFixture<ServiceProviderFixture>
{
    private readonly IAnnotationService _sut;
    
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
}