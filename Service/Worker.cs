using Core.Interfaces;

namespace Service;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IRenewalScheduler _scheduler;


    public Worker(ILogger<Worker> logger, IRenewalScheduler scheduler)
    {
        _logger = logger;
        _scheduler = scheduler;

    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("🔄 CrossCert Renewal Service started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _logger.LogInformation("Checking domains for renewal...");
                await _scheduler.RunAutoRenewalAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Auto-renewal failed.");
            }

            //await Task.Delay(TimeSpan.FromHours(12), stoppingToken); // configurable

            // TEST
            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
        }

        _logger.LogInformation("🛑 CrossCert Renewal Service stopped.");

    }
}
