using CrossCert.Data;
using CrossCert.Models;
using Microsoft.EntityFrameworkCore;

namespace CrossCert.Services
{
    public class RenewalLogService
    {
        private readonly CertDbContext _context;

        public RenewalLogService(CertDbContext context)
        {
            _context = context;
        }

        public async Task<List<RenewalLog>> GetRecentLogsAsync(int maxCount = 50)
        {
            return await _context.RenewalLogs
                .Include(r => r.Domain)
                .OrderByDescending(r => r.Timestamp)
                .Take(maxCount)
                .ToListAsync();
        }
    }
}