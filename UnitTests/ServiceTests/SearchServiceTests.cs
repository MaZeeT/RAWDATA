using Application.Interfaces.Services;
using Domain.Entities;
using Domain.Enums;
using Microsoft.Extensions.DependencyInjection;
using UnitTests.Fixtures;
using Xunit;

namespace UnitTests.ServiceTests;

public class SearchServiceTests : IClassFixture<ServiceProviderFixture>
{
    private readonly ISearchService _sut;
    
    public SearchServiceTests(ServiceProviderFixture fixture)
    {
        _sut = fixture.ServiceProvider.GetRequiredService<ISearchService>();
    }
    
    [Fact]
    public void InitialTest()
    {
        var pagination = new PagingAttributes();
        var result = _sut.Search(22, "dotnet", SearchType.BestMatch, pagination);
        
        Assert.Equal(7, result.Count);
    }
}