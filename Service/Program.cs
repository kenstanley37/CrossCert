using Core.Interfaces;
using Core.Services;
using Service;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddLogging(config =>
{
    config.AddConsole();
    config.SetMinimumLevel(LogLevel.Information);
});

builder.Services.AddSingleton<ICertificateManager, CertificateManager>();
builder.Services.AddSingleton<IRenewalScheduler, RenewalScheduler>();
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
