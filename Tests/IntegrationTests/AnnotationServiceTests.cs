using System;
using Application.Interfaces.Services;
using Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Web;

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
            PostId = 19,
            Body = "AddAnnotation_Successfully test",
            Date = new DateTime(2025, 11, 11, 11,11,11,11,DateTimeKind.Utc)
        };
        
        _sut.CreateAnnotation(annotation,  out var result);
        
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.GreaterThan(0));
    }
}