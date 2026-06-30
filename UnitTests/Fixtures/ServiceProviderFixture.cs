using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Web.DependencyRegistration;

namespace UnitTests.Fixtures;

public class ServiceProviderFixture
{
    public IServiceProvider ServiceProvider { get; }

    public ServiceProviderFixture()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Auth:PwdSize"] = "256",
                ["Auth:Key"] = "test-key",
                ["ConnectionStrings:DefaultConnection"] = "host=localhost;port=5432;db=stackoverflow;uid=postgres;pwd=Password123"
            })
            .Build();
        
        var services = new ServiceCollection();

        ServiceConfigurator.ConfigureServices(services, configuration);

        ServiceProvider = services.BuildServiceProvider();
    }
}