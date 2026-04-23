using Microsoft.Extensions.DependencyInjection;
using Web;

namespace UnitTests.Fixtures;

public class ServiceProviderFixture
{
    public IServiceProvider ServiceProvider { get; }

    public ServiceProviderFixture()
    {
        var services = new ServiceCollection();

        ServiceConfigurator.ConfigureServices(services);

        ServiceProvider = services.BuildServiceProvider();
    }
}