using CLI.Commands;
using System.CommandLine;

var rootCommand = new RootCommand("CrossCert CLI");

// Register subcommands
rootCommand.AddCommand(CertificateCommands.Build());
rootCommand.AddCommand(DomainCommands.Build());
rootCommand.AddCommand(ConfigCommands.Build());
rootCommand.AddCommand(MetaCommands.Build());

try
{
    await rootCommand.InvokeAsync(args);
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Unexpected error: {ex.Message}");
}

