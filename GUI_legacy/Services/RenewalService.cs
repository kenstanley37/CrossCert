using CrossCert.Data;
using CrossCert.Models;
using Microsoft.Extensions.Logging;

namespace CrossCert.Services
{
    public class RenewalService
    {
        private readonly DomainCertService _domainCertService;
        private readonly AcmeClientService _acmeClientService;
        private readonly CertDbContext _context;
        private readonly ILogger<RenewalService> _logger;

        public RenewalService(
            DomainCertService domainCertService,
            AcmeClientService acmeClientService,
            CertDbContext context,
            ILogger<RenewalService> logger)
        {
            _domainCertService = domainCertService;
            _acmeClientService = acmeClientService;
            _context = context;
            _logger = logger;
        }

        public async Task RunRenewalCheckAsync()
        {
            var threshold = DateTime.UtcNow.AddDays(14); // Renew if expiring within 14 days
            var expiringCerts = await _domainCertService.GetExpiringCertificatesAsync(threshold);

            foreach (var cert in expiringCerts)
            {
                try
                {
                    _logger.LogInformation($"Renewing cert for domain: {cert.Domain.DomainName}");

                    var newCert = await _acmeClientService.RenewCertificateAsync(cert.Domain);
                    await _domainCertService.SaveCertificateAsync(newCert);

                    _logger.LogInformation($"Renewal successful for {cert.Domain.DomainName}");

                    await _context.RenewalLogs.AddAsync(new RenewalLog
                    {
                        DomainId = cert.Domain.Id,
                        Timestamp = DateTime.UtcNow,
                        Result = "Success",
                        ErrorMessage = null
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Renewal failed for {cert.Domain.DomainName}");

                    await _context.RenewalLogs.AddAsync(new RenewalLog
                    {
                        DomainId = cert.Domain.Id,
                        Timestamp = DateTime.UtcNow,
                        Result = "Failed",
                        ErrorMessage = ex.Message
                    });
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}