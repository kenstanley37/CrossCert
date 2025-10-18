namespace CLI.Commands;

using CLI;
using Core.Interfaces;
using Core.Models;
using Microsoft.Extensions.DependencyInjection;
using System.CommandLine;

public static class DomainCommands
{
    public static Command Build()
    {
        var group = new Command("domain", "Manage domain list");

        // ➕ Add domain
        var addArg = new Argument<string>("domain", "The domain to add");
        var add = new Command("add", "Add a new domain") { addArg };

        add.SetHandler(async (string domain) =>
        {
            using var services = ServiceBuilder.Build();
            var certManager = services.GetRequiredService<ICertificateManager>();

            certManager.AddDomain(new Domain { DomainName = domain, IsActive = true });
            Console.WriteLine($"✅ Domain '{domain}' added.");

            await Task.CompletedTask;
        }, addArg);

        // 🗑️ Remove domain
        var removeArg = new Argument<string>("domain", "The domain to remove");
        var remove = new Command("remove", "Remove a domain") { removeArg };

        remove.SetHandler(async (string domain) =>
        {
            using var services = ServiceBuilder.Build();
            var certManager = services.GetRequiredService<ICertificateManager>();

            var success = certManager.RemoveDomain(domain);
            Console.WriteLine(success
                ? $"🗑️ Domain '{domain}' removed."
                : $"❌ Domain '{domain}' not found.");

            await Task.CompletedTask;
        }, removeArg);

        // 📋 List domains
        var list = new Command("list", "List all managed domains");
        list.SetHandler(() =>
        {
            using var services = ServiceBuilder.Build();
            var certManager = services.GetRequiredService<ICertificateManager>();
            var domains = certManager.GetDomains();

            if (!domains.Any())
            {
                Console.WriteLine("📭 No domains found.");
                return;
            }

            Console.WriteLine("📋 Managed Domains:");
            foreach (var domain in domains)
                Console.WriteLine($"  - {domain.DomainName} {(domain.IsActive ? "(Active)" : "(Inactive)")}");
        });

        // Register subcommands
        group.AddCommand(add);
        group.AddCommand(remove);
        group.AddCommand(list);

        return group;
    }
}