using Application.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;
using UnitTests.Fixtures;
using Xunit;

namespace UnitTests.ServiceTests;

public class BookmarkServiceTests : IClassFixture<ServiceProviderFixture>
{
    private readonly IBookmarkService _sut;
    
    public BookmarkServiceTests(ServiceProviderFixture fixture)
    {
        _sut = fixture.ServiceProvider.GetRequiredService<IBookmarkService>();
    }
    
    [Fact]
    public void InitialTest()
    {
        var count = _sut.GetCount(22);
        
        Assert.Equal(18, count);
    }
}