using Application.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;
using UnitTests.Fixtures;
using Xunit;

namespace UnitTests.ServiceTests;

public class UserServiceTests : IClassFixture<ServiceProviderFixture>
{
    private readonly IUserService _sut;
    
    public UserServiceTests(ServiceProviderFixture fixture)
    {
        _sut = fixture.ServiceProvider.GetRequiredService<IUserService>();
    }
    
    [Fact]
    public void InitialTest()
    {
        var result = _sut.GetUserName(22);
        
        Assert.Equal("mazeet", result);
    }
}