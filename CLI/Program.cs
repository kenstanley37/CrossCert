using Core.Interfaces;
using Core.Models;
using Core.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.CommandLine;

var rootCommand = new RootCommand("CrossCert CLI");

// 🔁 Renew Command
var renewCommand = new Command("renew", "Renew certificate for a domain")
{
    new Argument<string>("domain", "The domain to renew")
};

renewCommand.SetHandler(
    async (domain) =>
    {
        using var services = BuildServices();
        var certManager = services.GetRequiredService<ICertificateManager>();
        var renewalScheduler = services.GetRequiredService<IRenewalScheduler>();

        var domainModel = new Domain { DomainName = domain, IsActive = true };
        await renewalScheduler.ScheduleRenewalAsync(domainModel);
        await renewalScheduler.RunManualRenewalAsync();

        Console.WriteLine($"Renewal triggered for {domain}");
    },
    new Argument<string>("domain", "The domain to renew")
);

rootCommand.AddCommand(renewCommand);


// 🚀 Run CLI
await rootCommand.InvokeAsync(args);

// 🧾 Define arguments
var domainArg = new Argument<string>("domain", "The domain to export");
var pathArg = new Argument<string>("path", "The file path to save the certificate");

// 📦 Create command and add arguments
var exportCommand = new Command("export", "Export certificate to a file");
exportCommand.AddArgument(domainArg);
exportCommand.AddArgument(pathArg);

// 🔧 Bind handler with explicit descriptors
exportCommand.SetHandler<string, string>(
    async (domain, path) =>
    {
        using var services = BuildServices();
        var certManager = services.GetRequiredService<ICertificateManager>();

        var cert = await certManager.GenerateCertificateAsync(new Domain { DomainName = domain });
        await certManager.ExportCertificateAsync(cert, path);

        Console.WriteLine($"Certificate for {domain} exported to {path}");
    },
    domainArg,
    pathArg
);

rootCommand.AddCommand(exportCommand);

var installDomainArg = new Argument<string>("domain", "The domain to install the certificate for");

var installCommand = new Command("install", "Install certificate for a domain");
installCommand.AddArgument(installDomainArg);

installCommand.SetHandler<string>(
    async (domain) =>
    {
        using var services = BuildServices();
        var certManager = services.GetRequiredService<ICertificateManager>();

        var cert = await certManager.GenerateCertificateAsync(new Domain { DomainName = domain });
        var success = await certManager.InstallCertificateAsync(cert);

        Console.WriteLine(success
            ? $"✅ Certificate for {domain} installed successfully."
            : $"❌ Failed to install certificate for {domain}.");
    },
    installDomainArg
);

rootCommand.AddCommand(installCommand);

var showDomainArg = new Argument<string>("domain", "The domain to inspect");

var showCommand = new Command("show", "Display certificate metadata for a domain");
showCommand.AddArgument(showDomainArg);

showCommand.SetHandler<string>(
    async (domain) =>
    {
        using var services = BuildServices();
        var certManager = services.GetRequiredService<ICertificateManager>();

        var cert = await certManager.GetCertificateAsync(domain);

        if (cert == null)
        {
            Console.WriteLine($"❌ No certificate found for {domain}");
            return;
        }

        Console.WriteLine($"📄 Certificate for {domain}");
        Console.WriteLine($"  Issuer:     {cert.Issuer}");
        Console.WriteLine($"  Subject:    {cert.Subject}");
        Console.WriteLine($"  Expiry:     {cert.NotAfter}");
        Console.WriteLine($"  Thumbprint: {cert.Thumbprint}");
    },
    showDomainArg
);

rootCommand.AddCommand(showCommand);

// 🧱 DI Setup
static ServiceProvider BuildServices()
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