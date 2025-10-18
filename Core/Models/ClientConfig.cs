namespace Core.Models
{
    public class ClientConfig
    {
        public List<Domain> Domains { get; set; } = new();
        public bool AutoRenew { get; set; } = true;
        public int RenewalIntervalDays { get; set; } = 30;
        public List<RenewalLog> RenewalLogs { get; set; } = new();
    }
}
