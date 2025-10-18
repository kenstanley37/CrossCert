using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrossCert.Models
{
    public class RenewalLog
    {
        [Key]
        public int Id { get; set; }

        public int DomainId { get; set; }

        [ForeignKey(nameof(DomainId))]
        public virtual Domain Domain { get; set; } = null!;

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        public string Result { get; set; } = "Success"; // or "Failed"

        public string? ErrorMessage { get; set; }
    }
}