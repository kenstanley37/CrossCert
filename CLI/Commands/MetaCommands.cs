namespace CLI.Commands;

using Core.Services;
using System.CommandLine;
using System.Text.Json;

public static class MetaCommands
{
    public static Command Build()
    {
        var group = new Command("meta", "CLI info and logs");

        // 📦 Version
        var version = new Command("version", "Show CLI version");
        version.SetHandler(() =>
        {
            Console.WriteLine("CrossCert CLI v1.0.0");
            Console.WriteLine($"Build Date: {DateTime.UtcNow:yyyy-MM-dd}");
        });

        // 🧑 About
        var about = new Command("about", "Show author and purpose");
        about.SetHandler(() =>
        {
            Console.WriteLine("CrossCert CLI");
            Console.WriteLine("Created by Kenneth Stanley");
            Console.WriteLine("Secure certificate management for all platforms.");
        });

        // 📜 Logs (from config)
        var logs = new Command("logs", "Show recent renewal logs");
        logs.SetHandler(() =>
        {
            var config = ConfigStore.Load();
            var entries = config.RenewalLogs;

            if (entries == null || !entries.Any())
            {
                Console.WriteLine("📭 No renewal logs found.");
                return;
            }

            Console.WriteLine("📜 Renewal Logs:");
            foreach (var log in entries.OrderByDescending(l => l.Timestamp))
                Console.WriteLine($"  - {log.DomainName} @ {log.Timestamp:g} → {log.Result}");
        });

        var checkUpdates = new Command("check-updates", "Check for newer versions on GitHub");

        checkUpdates.SetHandler(async () =>
        {
            const string repoUrl = "https://github.com/kenstanley37/CrossCert";
            const string currentVersion = "v1.0.0";

            Console.WriteLine("🔍 Checking for updates...");

            try
            {
                using var client = new HttpClient();
                client.DefaultRequestHeaders.UserAgent.ParseAdd("CrossCertCLI/1.0");

                var apiUrl = "https://api.github.com/repos/kenstanley37/CrossCert/releases/latest";
                var response = await client.GetAsync(apiUrl);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);
                var latestTag = doc.RootElement.GetProperty("tag_name").GetString();

                if (latestTag != null && latestTag != currentVersion)
                {
                    Console.WriteLine($"🚀 New version available: {latestTag}");
                    Console.WriteLine($"🔗 Visit: {repoUrl}/releases");
                }
                else
                {
                    Console.WriteLine("✅ You’re using the latest version.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Could not check for updates: {ex.Message}");
            }
        });

        // Register subcommands
        group.AddCommand(version);
        group.AddCommand(about);
        group.AddCommand(logs);
        group.AddCommand(checkUpdates);

        return group;
    }
}