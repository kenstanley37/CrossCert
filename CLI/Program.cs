using CLI.Commands;
using System.CommandLine;
using System.Text.Json;

var rootCommand = new RootCommand("CrossCert CLI");

// Register subcommands
rootCommand.AddCommand(CertificateCommands.Build());
rootCommand.AddCommand(DomainCommands.Build());
rootCommand.AddCommand(ConfigCommands.Build());
rootCommand.AddCommand(MetaCommands.Build());

try
{
    await CheckForUpdatesAsync();
    await rootCommand.InvokeAsync(args);
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Unexpected error: {ex.Message}");
}

static async Task CheckForUpdatesAsync()
{
    var currentVersion = System.Reflection.Assembly
    .GetExecutingAssembly()
    .GetName()
    .Version?
    .ToString() ?? "0.0.0";

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
            Console.WriteLine($"🚀 Update available: {latestTag} → Run 'meta check-updates' for details.");
        }
    }
    catch
    {
        // Silent fail — no need to alert user unless explicitly requested
    }
}

