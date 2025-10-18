using Core.Models;

namespace Core.Interfaces
{
    public interface ICertificateManager
    {
        Task<CertificateInfo> GenerateCertificateAsync(Domain domain);
        Task ExportCertificateAsync(CertificateInfo cert, string outputPath);
        Task<bool> InstallCertificateAsync(CertificateInfo cert);

        // 🆕 Add this for the `show` command
        Task<CertificateInfo?> GetCertificateAsync(string domain);
        void AddDomain(Domain domain);
        IEnumerable<Domain> GetDomains();
        bool RemoveDomain(string domainName);

    }

}
