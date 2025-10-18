using Core.Interfaces;
using Core.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CLI;

public static class ServiceBuilder
{
    public static ServiceProvider Build()
    {
        var services = new ServiceCollection();

        services.AddLogging(config =>
        {
            config.AddConsole();
            config.SetMinimumLevel(LogLevel.Information);
        });

        services.AddSingleton<ICertificateManager, CertificateManager>();
        services.AddSingleton<IRenewalScheduler, RenewalScheduler>();

        return services.BuildServiceProvider();
    }
}