namespace Core.Models
{
    public class RenewalLog
    {
        public string DomainName { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public string Result { get; set; } = string.Empty;
        public string? ErrorMessage { get; set; }
    }
}
