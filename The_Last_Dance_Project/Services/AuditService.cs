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

        public async Task<List<AuditEntity>> GetAllLogsAsync()
        {
            return await _context.AuditEntities.ToListAsync();
        }
    }

    public interface IAuditService
    {
        Task<List<AuditEntity>> GetAllLogsAsync();
    }
}
