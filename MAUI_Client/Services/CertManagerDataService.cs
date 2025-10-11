using CrossCert.Data;
using CrossCert.Models;
using Microsoft.EntityFrameworkCore; // <-- THIS LINE IS THE FIX

namespace CrossCert.Services
{
    public class CertManagerDataService
    {
        private readonly CrossCertDbContext _dbContext;

        public CertManagerDataService(CrossCertDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Retrieves all managed domains from the database.
        /// </summary>
        public async Task<List<Domain>> GetAllDomainsAsync()
        {
            try
            {
                // Include the related certificate data (requires 'using Microsoft.EntityFrameworkCore;')
                return await _dbContext.Domains
                    .Include(d => d.Certificate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DB ERROR] Failed to retrieve domains: {ex.Message}");
                return new List<Domain>();
            }
        }

        /// <summary>
        /// Adds a new domain to the database.
        /// </summary>
        public async Task AddDomainAsync(Domain domain)
        {
            try
            {
                await _dbContext.Domains.AddAsync(domain);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DB ERROR] Failed to add domain: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Deletes an existing domain from the database based on its ID.
        /// Note: The associated certificate will be deleted automatically due to EF Core cascade delete conventions.
        /// </summary>
        public async Task DeleteDomainAsync(int domainId)
        {
            try
            {
                var domainToDelete = await _dbContext.Domains.FindAsync(domainId);
                if (domainToDelete != null)
                {
                    _dbContext.Domains.Remove(domainToDelete);
                    await _dbContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DB ERROR] Failed to delete domain ID {domainId}: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Placeholder method for future certificate renewal and client configuration.
        /// </summary>
        public Task RenewCertificatesAsync()
        {
            // Implementation pending: Will contain Certes logic
            return Task.CompletedTask;
        }
    }
}
