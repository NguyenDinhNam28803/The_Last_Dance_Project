using Microsoft.EntityFrameworkCore;
using The_Last_Dance_Project.Data;
using The_Last_Dance_Project.Interfaces;
using The_Last_Dance_Project.Models;

namespace The_Last_Dance_Project.Services
{
    public class AuditService : IAuditService
    {
        private readonly ApplicationDbContext _context;

        public AuditService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Auditentity>> GetAllLogsAsync()
        {
            return await _context.Auditentities.Include(a => a.MtTranId).ToListAsync();
        }
    }

    public interface IAuditService
    {
        Task<List<Auditentity>> GetAllLogsAsync();
    }
}
