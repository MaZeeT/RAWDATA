using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Web.DependencyRegistration;

internal static class LoggerProviders
{
    internal static void RegisterSerilogLogger(IServiceCollection services)
    {
        var loggerConfiguration = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .WriteTo.Console()
            .CreateLogger();
        
        var loggerFactory = new LoggerFactory().AddSerilog(loggerConfiguration);
        
        services.AddSingleton(loggerFactory);
    }
}