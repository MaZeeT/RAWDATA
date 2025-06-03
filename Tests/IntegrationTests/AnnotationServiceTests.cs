using Domain.AnnotationsDTOs;
using DomainServices.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using WebService;

namespace Tests.IntegrationTests;

public class AnnotationServiceTests
{
    private IAnnotationService _sut;
    private const int UserId = 12;

    [SetUp]
    public void SetUp()
    {
        var services = new ServiceCollection();
        ServiceConfigurator.ConfigureServices(services);
        var serviceProvider = services.BuildServiceProvider();
        _sut = serviceProvider.GetRequiredService<IAnnotationService>();
        
    }
    
    [Test]
    public void AddAnnotation_Successfully()
    {
        var annotation = new AnnotationsDto
        {
            UserId = UserId,
            PostId = 14,
            Body = "AddAnnotation_Successfully test"
        };
        
        _sut.CreateAnnotation(annotation,  out var result);
        
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.GreaterThan(0));
    }
}