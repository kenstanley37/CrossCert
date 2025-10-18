using Core.Interfaces;
using Core.Models;
using Microsoft.Extensions.Logging;

namespace Core.Services
{
    public class RenewalScheduler : IRenewalScheduler
    {
        private readonly ILogger<RenewalScheduler> _logger;
        private readonly ICertificateManager _certManager;
        private readonly List<Domain> _scheduledDomains = [];

        public event EventHandler<RenewalLog>? RenewalCompleted;

        public RenewalScheduler(ICertificateManager certManager, ILogger<RenewalScheduler> logger)
        {
            _certManager = certManager;
            _logger = logger;
        }

        public Task ScheduleRenewalAsync(Domain domain)
        {
            if (!_scheduledDomains.Any(d => d.DomainName == domain.DomainName))
            {
                _scheduledDomains.Add(domain);
                _logger.LogInformation("Scheduled renewal for {Domain}", domain.DomainName);
            }

            return Task.CompletedTask;
        }

        public async Task RunManualRenewalAsync()
        {
            foreach (var domain in _scheduledDomains)
            {
                var log = new RenewalLog
                {
                    DomainName = domain.DomainName,
                    Timestamp = DateTime.UtcNow
                };

                try
                {
                    var cert = await _certManager.GenerateCertificateAsync(domain);
                    var installed = await _certManager.InstallCertificateAsync(cert);

                    log.Result = installed ? "Success" : "Install Failed";
                    _logger.LogInformation("Renewal completed for {Domain}", domain.DomainName);
                }
                catch (Exception ex)
                {
                    log.Result = "Error";
                    log.ErrorMessage = ex.Message;
                    _logger.LogError(ex, "Renewal failed for {Domain}", domain.DomainName);
                }

                RenewalCompleted?.Invoke(this, log);
            }
        }
    }
}
