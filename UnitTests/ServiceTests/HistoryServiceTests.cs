using Application.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;
using UnitTests.Fixtures;
using Xunit;

namespace UnitTests.ServiceTests;

public class HistoryServiceTests : IClassFixture<ServiceProviderFixture>
{
    private readonly IHistoryService _sut;
    
    public HistoryServiceTests(ServiceProviderFixture fixture)
    {
        _sut = fixture.ServiceProvider.GetRequiredService<IHistoryService>();
    }
    
    [Fact]
    public void InitialTest()
    {
        var count = _sut.GetCount(22);
        
        Assert.Equal(93, count);
    }
}