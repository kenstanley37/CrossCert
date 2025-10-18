using Core.Interfaces;
using Core.Models;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography.X509Certificates;

namespace Core.Services
{
    public class CertificateManager : ICertificateManager
    {
        private readonly ILogger<CertificateManager> _logger;

        public CertificateManager(ILogger<CertificateManager> logger)
        {
            _logger = logger;
        }

        public async Task<CertificateInfo> GenerateCertificateAsync(Domain domain)
        {
            _logger.LogInformation("Generating certificate for {Domain}", domain.DomainName);

            // TODO: Replace with actual ACME logic
            await Task.Delay(500); // Simulate work

            return new CertificateInfo
            {
                DomainName = domain.DomainName,
                PfxData = Array.Empty<byte>(), // Replace with real cert data
                ExpiryDate = DateTime.UtcNow.AddMonths(3)
            };
        }

        public async Task ExportCertificateAsync(CertificateInfo cert, string outputPath)
        {
            _logger.LogInformation("Exporting certificate for {Domain} to {Path}", cert.DomainName, outputPath);

            // TODO: Write cert.PfxData to outputPath
            await File.WriteAllBytesAsync(outputPath, cert.PfxData);
        }

        public async Task<bool> InstallCertificateAsync(CertificateInfo cert)
        {
            _logger.LogInformation("Installing certificate for {Domain}", cert.DomainName);

            // TODO: Implement platform-specific install logic
            await Task.Delay(300); // Simulate install

            return true;
        }

        public async Task<CertificateInfo?> GetCertificateAsync(string domain)
        {
            var certPath = GetCertPathForDomain(domain);
            if (!File.Exists(certPath))
            {
                _logger.LogWarning("Certificate file not found for domain {Domain}", domain);
                return null;
            }

            var certInfo = await LoadCertificateInfoAsync(certPath);
            if (certInfo != null)
            {
                _logger.LogInformation("Loaded certificate for {Domain}", certInfo.DomainName);
            }

            return certInfo;
        }

        private string GetCertPathForDomain(string domain)
        {
            var certDirectory = Path.Combine(AppContext.BaseDirectory, "certs");
            var certFileName = $"{domain}.pfx"; // You can later make this format configurable
            return Path.Combine(certDirectory, certFileName);
        }

        private async Task<CertificateInfo?> LoadCertificateInfoAsync(string certPath)
        {
            try
            {
                var certBytes = await File.ReadAllBytesAsync(certPath);

#pragma warning disable SYSLIB0048
#pragma warning disable SYSLIB0057
                var cert = new X509Certificate2(certBytes);
#pragma warning restore SYSLIB0057
#pragma warning restore SYSLIB0048

                var dnsName = cert.GetNameInfo(X509NameType.DnsName, false);
                if (string.IsNullOrWhiteSpace(dnsName))
                {
                    dnsName = cert.Subject; // Fallback if DNS name is missing
                }

                return new CertificateInfo
                {
                    Issuer = cert.Issuer,
                    Subject = cert.Subject,
                    NotAfter = cert.NotAfter,
                    Thumbprint = cert.Thumbprint,
                    DomainName = dnsName,
                    PfxData = certBytes,
                    ExpiryDate = cert.NotAfter
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load certificate from {Path}", certPath);
                return null;
            }
        }
    }
}