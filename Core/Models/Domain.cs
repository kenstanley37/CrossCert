namespace Core.Models
{
    public class Domain
    {
        public string DomainName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime LastRenewed { get; set; }
    }

}
