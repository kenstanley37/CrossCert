using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrossCert.Models
{
    /// <summary>
    /// Defines the type of ACME client configuration used for a domain.
    /// </summary>
    public enum ClientType
    {
        Unknown = 0,
        LetsEncrypt = 1, // Production
        LetsEncryptStaging = 2, // Testing environment
        Custom = 3 // For custom ACME providers
    }

    /// <summary>
    /// Represents a single managed domain.
    /// </summary>
    public class Domain
    {
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// The primary domain name (e.g., example.com).
        /// </summary>
        [Required]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Comma-separated list of Subject Alternative Names (SANs) including wildcards.
        /// </summary>
        public string SubjectAlternativeNames { get; set; } = string.Empty;

        /// <summary>
        /// Date of the next scheduled attempt to renew the certificate.
        /// </summary>
        public DateTime NextRenewalAttempt { get; set; } = DateTime.Now;

        /// <summary>
        /// Foreign key for the current certificate associated with this domain.
        /// </summary>
        public int? CertificateId { get; set; }

        /// <summary>
        /// Navigation property for the current certificate. Null if no certificate is issued yet.
        /// </summary>
        [ForeignKey(nameof(CertificateId))]
        public virtual Certificate? Certificate { get; set; }

        /// <summary>
        /// Foreign key for the client configuration settings.
        /// </summary>
        [Required]
        public int ClientConfigId { get; set; }

        /// <summary>
        /// Navigation property for the client configuration settings.
        /// </summary>
        [ForeignKey(nameof(ClientConfigId))]
        public virtual ClientConfig ClientConfig { get; set; } = new ClientConfig();

        /// <summary>
        /// Foreign key for the DNS credential settings.
        /// </summary>
        public int? DnsCredentialId { get; set; }

        /// <summary>
        /// Navigation property for the DNS credential settings. Null if using HTTP-01 challenge.
        /// </summary>
        [ForeignKey(nameof(DnsCredentialId))]
        public virtual DnsCredentials? DnsCredentials { get; set; }

        // --- Calculated Display Properties for UI ---

        /// <summary>
        /// Provides a human-readable status text for the certificate health.
        /// </summary>
        [NotMapped] // Tell EF Core not to map this property to a database column
        public string StatusText
        {
            get
            {
                if (Certificate == null)
                {
                    // No certificate issued yet.
                    return "Pending";
                }

                var daysUntilExpiration = (Certificate.ExpiryDate - DateTime.Today).Days;

                if (daysUntilExpiration < 0)
                {
                    return "EXPIRED";
                }
                else if (daysUntilExpiration <= 30) // Standard warning period
                {
                    return "Expiring Soon";
                }
                else
                {
                    return "Active";
                }
            }
        }

        /// <summary>
        /// Provides a color representation for the status for UI binding.
        /// </summary>
        [NotMapped]
        public Color StatusColor
        {
            get
            {
                if (Certificate == null)
                {
                    return Colors.DarkGray; // Pending
                }

                var daysUntilExpiration = (Certificate.ExpiryDate - DateTime.Today).Days;

                if (daysUntilExpiration < 0)
                {
                    return Colors.Red; // Critical: Expired
                }
                else if (daysUntilExpiration <= 30)
                {
                    return Colors.Orange; // Warning: Expiring soon
                }
                else
                {
                    return Colors.Green; // Healthy
                }
            }
        }
    }

    /// <summary>
    /// Stores certificate data after successful issuance.
    /// </summary>
    public class Certificate
    {
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// The PEM-encoded certificate chain (full chain).
        /// </summary>
        [Required]
        public string PemChain { get; set; } = string.Empty;

        /// <summary>
        /// The date the certificate was issued.
        /// </summary>
        public DateTime IssueDate { get; set; } = DateTime.Now;

        /// <summary>
        /// The date the certificate will expire.
        /// </summary>
        public DateTime ExpiryDate { get; set; }

        /// <summary>
        /// Optional: The date the certificate was revoked, if applicable.
        /// </summary>
        public DateTime? RevocationDate { get; set; }

        /// <summary>
        /// The date the record was last updated in the database.
        /// </summary>
        public DateTime LastUpdated { get; set; } = DateTime.Now;

        // Navigation property back to Domain
        public virtual Domain? Domain { get; set; }
    }

    /// <summary>
    /// Stores configuration for the ACME client connection.
    /// </summary>
    public class ClientConfig
    {
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// The ACME server URL (e.g., Let's Encrypt production or staging).
        /// </summary>
        [Required]
        public string AcmeServerUrl { get; set; } = string.Empty;

        /// <summary>
        /// The email address used for ACME account registration.
        /// </summary>
        [Required]
        public string RegistrationEmail { get; set; } = string.Empty;

        /// <summary>
        /// The type of ACME client being used (e.g., LetsEncrypt).
        /// </summary>
        public ClientType ClientType { get; set; } = ClientType.Unknown;
    }

    /// <summary>
    /// Stores credentials used for DNS-01 challenges.
    /// </summary>
    public class DnsCredentials
    {
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// The API key or token for the DNS provider.
        /// </summary>
        [Required]
        public string ApiKey { get; set; } = string.Empty;

        /// <summary>
        /// Optional: The user or email associated with the DNS API key.
        /// </summary>
        public string? UserEmail { get; set; }

        /// <summary>
        /// Identifier for the DNS provider (e.g., Cloudflare, Azure DNS).
        /// </summary>
        [Required]
        public string ProviderName { get; set; } = string.Empty;

        /// <summary>
        /// Foreign key back to the domain this credential is for.
        /// </summary>
        public int DomainId { get; set; }
        [ForeignKey(nameof(DomainId))]
        public virtual Domain Domain { get; set; } = null!;
    }
}
