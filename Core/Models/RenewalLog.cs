namespace Core.Models
{
    public class RenewalLog
    {
        public string DomainName { get; set; } = string.Empty;

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Result of the renewal attempt: "Success", "Failed", "Skipped", etc.
        /// </summary>
        public string Result { get; set; } = "Pending";

        /// <summary>
        /// Optional error message if the renewal failed
        /// </summary>
        public string? ErrorMessage { get; set; }

        /// <summary>
        /// Optional certificate thumbprint for tracking
        /// </summary>
        public string? Thumbprint { get; set; }

        /// <summary>
        /// Optional issuer info for display
        /// </summary>
        public string? Issuer { get; set; }

        /// <summary>
        /// Optional expiry date for the renewed certificate
        /// </summary>
        public DateTime? NotAfter { get; set; }
    }
}