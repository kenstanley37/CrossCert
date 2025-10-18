namespace Core.Models
{
    public class CertificateInfo
    {
        public string DomainName { get; set; } = string.Empty;
        public byte[] PfxData { get; set; } = Array.Empty<byte>();
        public DateTime ExpiryDate { get; set; }

        public string Issuer { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public DateTime NotAfter { get; set; }
        public string Thumbprint { get; set; } = string.Empty;
    }
}
