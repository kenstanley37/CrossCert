using CrossCert.Models;
using Microsoft.EntityFrameworkCore;

namespace CrossCert.Data
{
    // FIX: Must be PUBLIC to be used in the dependency injection chain
    public class CrossCertDbContext : DbContext
    {
        // Define your DbSet properties for each model
        public DbSet<Domain> Domains { get; set; }
        public DbSet<Certificate> Certificates { get; set; }
        public DbSet<ClientConfig> ClientConfigs { get; set; }
        public DbSet<DnsCredentials> DnsCredentials { get; set; }

        public CrossCertDbContext()
        {
            // Empty constructor needed for Design Time Factory
        }

        public CrossCertDbContext(DbContextOptions<CrossCertDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure composite keys, indexes, or special relationships here if needed.
            // Currently using default conventions, so this remains minimal.
            base.OnModelCreating(modelBuilder);
        }

        /// <summary>
        /// Gets the platform-specific path where the SQLite database file should reside.
        /// </summary>
        public static string GetDatabasePath()
        {
            // Check if running on Android/iOS/MacCatalyst/Windows
            var path = FileSystem.AppDataDirectory;
            return Path.Combine(path, "CrossCert.db");
        }
    }
}