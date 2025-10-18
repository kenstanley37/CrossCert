using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

// Important: Add the using statement for the CrossCert.Data namespace if not already present

namespace CrossCert.Data
{
    /// <summary>
    /// Design-time factory required by the Entity Framework Core tooling 
    /// (dotnet ef migrations) to create a DbContext instance outside of the 
    /// MAUI application's runtime environment.
    /// </summary>
    // FIX: Must be PUBLIC to ensure full accessibility throughout the compilation process
    public class CrossCertDbContextFactory : IDesignTimeDbContextFactory<CrossCertDbContext>
    {
        public CrossCertDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<CrossCertDbContext>();

            // NOTE: The path used here is a mock path for design-time only.
            // The real path is determined at application runtime (in MauiProgram.cs).
            optionsBuilder.UseSqlite($"Filename=DesignOnly.db");

            return new CrossCertDbContext(optionsBuilder.Options);
        }
    }
}