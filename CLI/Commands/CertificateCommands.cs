namespace CLI.Commands;

using CLI;
using Core.Interfaces;
using Core.Models;
using Core.Services;
using Microsoft.Extensions.DependencyInjection;
using System.CommandLine;

public static class CertificateCommands
{
    public static Command Build()
    {
        var group = new Command("cert", "Certificate operations");

        // 🔁 Renew
        var renewDomainArg = new Argument<string>("domain", "The domain to renew");
        var renew = new Command("renew", "Renew certificate") { renewDomainArg };

        renew.SetHandler(async (string domain) =>
        {
            using var services = ServiceBuilder.Build();
            var scheduler = services.GetRequiredService<IRenewalScheduler>();

            var domainModel = new Domain
            {
                DomainName = domain,
                IsActive = true,
                LastRenewed = DateTime.UtcNow
            };

            await scheduler.ScheduleRenewalAsync(domainModel);
            await scheduler.RunManualRenewalAsync();

            // Load config and update domain timestamp
            var config = ConfigStore.Load();
            var match = config.Domains.FirstOrDefault(d => d.DomainName == domain);
            if (match != null)
            {
                match.LastRenewed = domainModel.LastRenewed;
            }

            // Add renewal log entry
            config.RenewalLogs.Add(new RenewalLog
            {
                DomainName = domain,
                Timestamp = DateTime.UtcNow,
                Result = "Success"
            });

            ConfigStore.Save(config);

            Console.WriteLine($"✅ Renewal triggered for {domain}");
        }, renewDomainArg);


        // 📦 Export
        var exportDomainArg = new Argument<string>("domain", "The domain to export");
        var exportPathArg = new Argument<string>("path", "The file path to save the certificate");
        var export = new Command("export", "Export certificate to a file")
        {
            exportDomainArg,
            exportPathArg
        };

        export.SetHandler(async (string domain, string path) =>
        {
            using var services = ServiceBuilder.Build();
            var certManager = services.GetRequiredService<ICertificateManager>();

            var cert = await certManager.GenerateCertificateAsync(new Domain { DomainName = domain });
            await certManager.ExportCertificateAsync(cert, path);

            Console.WriteLine($"✅ Certificate for {domain} exported to {path}");
        }, exportDomainArg, exportPathArg);

        // 🛠️ Install
        var installDomainArg = new Argument<string>("domain", "The domain to install");
        var install = new Command("install", "Install certificate for a domain") { installDomainArg };

        install.SetHandler(async (string domain) =>
        {
            using var services = ServiceBuilder.Build();
            var certManager = services.GetRequiredService<ICertificateManager>();

            var cert = await certManager.GenerateCertificateAsync(new Domain { DomainName = domain });
            var success = await certManager.InstallCertificateAsync(cert);

            Console.WriteLine(success
                ? $"✅ Certificate for {domain} installed successfully."
                : $"❌ Failed to install certificate for {domain}.");
        }, installDomainArg);

        // 🔍 Show
        var showDomainArg = new Argument<string>("domain", "The domain to inspect");
        var show = new Command("show", "Display certificate metadata") { showDomainArg };

        show.SetHandler(async (string domain) =>
        {
            using var services = ServiceBuilder.Build();
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
        }, showDomainArg);

        // Register all subcommands
        group.AddCommand(renew);
        group.AddCommand(export);
        group.AddCommand(install);
        group.AddCommand(show);

        return group;
    }
}