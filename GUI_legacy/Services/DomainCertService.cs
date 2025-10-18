using CrossCert.Data;
using CrossCert.Models;
using Microsoft.EntityFrameworkCore;

namespace CrossCert.Services
{
    public class DomainCertService
    {
        private readonly CertDbContext _context;

        public DomainCertService(CertDbContext context)
        {
            _context = context;
        }

        // Domain CRUD
        public async Task<List<Domain>> GetDomainsAsync()
        {
            return await _context.Domains.Include(d => d.Certificate).ToListAsync();
        }

        public async Task AddDomainAsync(Domain domain)
        {
            _context.Domains.Add(domain);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateDomainAsync(Domain domain)
        {
            _context.Domains.Update(domain);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteDomainAsync(int domainId)
        {
            var domain = await _context.Domains.FindAsync(domainId);
            if (domain != null)
            {
                _context.Domains.Remove(domain);
                await _context.SaveChangesAsync();
            }
        }

        // Certificate CRUD
        public async Task SaveCertificateAsync(Certificate cert)
        {
            _context.Certificates.Add(cert);
            await _context.SaveChangesAsync();
        }

        public async Task<Certificate?> GetCertificateForDomainAsync(int domainId)
        {
            return await _context.Certificates.FirstOrDefaultAsync(c => c.DomainId == domainId);
        }

        public async Task<List<Certificate>> GetExpiringCertificatesAsync(DateTime threshold)
        {
            return await _context.Certificates
                .Where(c => c.ExpiryDate <= threshold)
                .Include(c => c.Domain)
                .ToListAsync();
        }
    }
}