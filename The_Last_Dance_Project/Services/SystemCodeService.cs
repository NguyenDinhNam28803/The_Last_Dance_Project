using Microsoft.EntityFrameworkCore;
using The_Last_Dance_Project.Data;
using The_Last_Dance_Project.Models;

namespace The_Last_Dance_Project.Services
{
    public interface ISystemCodeService
    {
        Task<List<SystemCode>> GetSystemCodesAsync();
        Task AddSystemCodeAsync(SystemCode systemCode);
    }

    public class SystemCodeService : ISystemCodeService
    {
        private readonly ApplicationDbContext _context;

        public SystemCodeService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<SystemCode>> GetSystemCodesAsync()
        {
            return await _context.SystemCodes.Include(s => s.Values).ToListAsync();
        }

        public async Task AddSystemCodeAsync(SystemCode systemCode)
        {
            _context.SystemCodes.Add(systemCode);
            await _context.SaveChangesAsync();
        }
    }
}
