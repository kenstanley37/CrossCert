using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrossCert.Models
{
    public enum ClientType
    {
        Unknown = 0,
        LetsEncrypt = 1,
        LetsEncryptStaging = 2,
        Custom = 3
    }

    public class Domain
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string DomainName { get; set; } = string.Empty;

        public string SubjectAlternativeNames { get; set; } = string.Empty;

        public DateTime NextRenewalAttempt { get; set; } = DateTime.Now;

        public int? CertificateId { get; set; }

        [ForeignKey(nameof(CertificateId))]
        [InverseProperty("Domain")]
        public virtual Certificate? Certificate { get; set; }

        [Required]
        public int ClientConfigId { get; set; }

        [ForeignKey(nameof(ClientConfigId))]
        public virtual ClientConfig ClientConfig { get; set; } = new ClientConfig();

        public int? DnsCredentialId { get; set; }

        [ForeignKey(nameof(DnsCredentialId))]
        public virtual DnsCredentials? DnsCredentials { get; set; }

        [NotMapped]
        public string StatusText => Certificate == null
            ? "Pending"
            : (Certificate.ExpiryDate < DateTime.Today ? "EXPIRED"
              : (Certificate.ExpiryDate - DateTime.Today).Days <= 30 ? "Expiring Soon"
              : "Active");

        [NotMapped]
        public Color StatusColor => Certificate == null
            ? Colors.DarkGray
            : (Certificate.ExpiryDate < DateTime.Today ? Colors.Red
              : (Certificate.ExpiryDate - DateTime.Today).Days <= 30 ? Colors.Orange
              : Colors.Green);
    }

    public class Certificate
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string PemChain { get; set; } = string.Empty;

        public DateTime IssueDate { get; set; } = DateTime.Now;

        public DateTime ExpiryDate { get; set; }

        public DateTime? RevocationDate { get; set; }

        public DateTime LastUpdated { get; set; } = DateTime.Now;

        public string? RenewalStatus { get; set; }

        public int DomainId { get; set; }

        [ForeignKey(nameof(DomainId))]
        [InverseProperty("Certificate")]
        public virtual Domain Domain { get; set; } = null!;
    }

    public class ClientConfig
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string AcmeServerUrl { get; set; } = string.Empty;

        [Required]
        public string RegistrationEmail { get; set; } = string.Empty;

        public ClientType ClientType { get; set; } = ClientType.Unknown;
    }

    public class DnsCredentials
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string ApiKey { get; set; } = string.Empty;

        public string? UserEmail { get; set; }

        [Required]
        public string ProviderName { get; set; } = string.Empty;

        public int DomainId { get; set; }

        [ForeignKey(nameof(DomainId))]
        public virtual Domain Domain { get; set; } = null!;
    }
}