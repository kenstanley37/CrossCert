using CrossCert.Models;
using Microsoft.EntityFrameworkCore;

namespace CrossCert.Data
{
    public class CertDbContext : DbContext
    {
        public CertDbContext(DbContextOptions<CertDbContext> options)
            : base(options)
        {
        }

        public DbSet<Domain> Domains { get; set; }
        public DbSet<Certificate> Certificates { get; set; }
        public DbSet<ClientConfig> ClientConfigs { get; set; }
        public DbSet<DnsCredentials> DnsCredentials { get; set; }
        public DbSet<RenewalLog> RenewalLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Optional: configure relationships explicitly
            modelBuilder.Entity<Domain>()
                .HasOne(d => d.Certificate)
                .WithOne(c => c.Domain)
                .HasForeignKey<Certificate>(c => c.DomainId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Domain>()
                .HasOne(d => d.DnsCredentials)
                .WithOne(dc => dc.Domain)
                .HasForeignKey<DnsCredentials>(dc => dc.DomainId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}