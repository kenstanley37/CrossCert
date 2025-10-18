namespace CLI.Commands;

using Core.Models;
using Core.Services;
using System.CommandLine;
using System.Diagnostics;
using System.Text.Json;

public static class ConfigCommands
{
    public static Command Build()
    {
        var group = new Command("config", "Manage configuration");

        // 🛠️ Show current config
        var show = new Command("show", "Display current configuration");
        show.SetHandler(() =>
        {
            var config = ConfigStore.Load();
            var json = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
            Console.WriteLine("🛠️ Current Configuration:");
            Console.WriteLine(json);
        });

        // 📝 Edit config file
        var edit = new Command("edit", "Open config file in default editor");
        edit.SetHandler(() =>
        {
            var path = ConfigStore.GetConfigPath();
            Console.WriteLine($"📝 Opening config file: {path}");

            Process.Start(new ProcessStartInfo
            {
                FileName = path,
                UseShellExecute = true
            });
        });

        // 🔧 Set config value
        var keyArg = new Argument<string>("key", "Config key to update");
        var valueArg = new Argument<string>("value", "New value");

        var set = new Command("set", "Update a config value")
        {
            keyArg,
            valueArg
        };

        set.SetHandler(async (string key, string value) =>
        {
            var config = ConfigStore.Load();

            switch (key.ToLowerInvariant())
            {
                case "autorenew":
                    config.AutoRenew = bool.TryParse(value, out var b) && b;
                    break;
                case "renewalintervaldays":
                    config.RenewalIntervalDays = int.TryParse(value, out var i) ? i : config.RenewalIntervalDays;
                    break;
                default:
                    Console.WriteLine($"❌ Unknown config key: {key}");
                    await Task.CompletedTask;
                    return;
            }

            ConfigStore.Save(config);
            Console.WriteLine($"✅ Updated '{key}' to '{value}'");

            await Task.CompletedTask;
        }, keyArg, valueArg);

        var reset = new Command("reset", "Restore default configuration");

        reset.SetHandler(() =>
        {
            var config = new ClientConfig
            {
                Domains = new List<Domain>
        {
            new Domain { DomainName = "example.com", IsActive = true }
        },
                AutoRenew = true,
                RenewalIntervalDays = 30,
                RenewalLogs = new List<RenewalLog>()
            };

            ConfigStore.Save(config);
            Console.WriteLine("🔄 Configuration reset to default.");
            Console.WriteLine("📁 You can now run 'cert renew example.com' to begin.");
        });

        // 📁 Show config file path
        var path = new Command("path", "Show config file path");
        path.SetHandler(() =>
        {
            var configPath = ConfigStore.GetConfigPath();
            Console.WriteLine($"📁 Config file path: {configPath}");
        });

        // 🧰 Init config
        var init = new Command("init-config", "Create a starter config file");
        init.SetHandler(() =>
        {
            var config = new ClientConfig
            {
                Domains = new List<Domain>
                {
                    new Domain { DomainName = "example.com", IsActive = true }
                },
                AutoRenew = true,
                RenewalIntervalDays = 30,
                RenewalLogs = new List<RenewalLog>()
            };

            ConfigStore.Save(config);
            Console.WriteLine("✅ Starter config file created.");
        });

        // Register subcommands
        group.AddCommand(show);
        group.AddCommand(edit);
        group.AddCommand(set);
        group.AddCommand(reset);
        group.AddCommand(path);
        group.AddCommand(init);

        return group;
    }
}