using Application.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;
using UnitTests.Fixtures;
using Xunit;

namespace UnitTests.ServiceTests;

public class ThreadServiceTests : IClassFixture<ServiceProviderFixture>
{
    private readonly IThreadService _sut;
    
    public ThreadServiceTests(ServiceProviderFixture fixture)
    {
        _sut = fixture.ServiceProvider.GetRequiredService<IThreadService>();
    }
    
    [Fact]
    public void InitialTest()
    {
        var result = _sut.GetPost(19);
        
        Assert.Equal(19, result.Id);
    }
}