using Microsoft.EntityFrameworkCore;
using The_Last_Dance_Project.Data;
using The_Last_Dance_Project.Dto;
using The_Last_Dance_Project.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using BCrypt.Net;

namespace The_Last_Dance_Project.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ApplicationDbContext _db;

        public CustomerService(ApplicationDbContext db)
        {
            _db = db;
        }

        #region Core CRUD
        public async Task<IEnumerable<Customer>> GetAllAsync()
        {
            return await _db.Customers.AsNoTracking().ToListAsync();
        }

        public async Task<Customer?> GetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id)) return null;
            return await _db.Customers.FindAsync(id);
        }

        public async Task<Customer> CreateAsync(Customer customer)
        {
            _db.Customers.Add(customer);
            await _db.SaveChangesAsync();
            return customer;
        }

        public async Task<Customer?> UpdateAsync(string id, Customer customer)
        {
            var existing = await _db.Customers.FindAsync(id);
            if (existing == null) return null;

            _db.Entry(existing).CurrentValues.SetValues(customer);
            await _db.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var existing = await _db.Customers.FindAsync(id);
            if (existing == null) return false;
            _db.Customers.Remove(existing);
            await _db.SaveChangesAsync();
            return true;
        }
        #endregion

        #region User Management
        public async Task<IEnumerable<UserResponseDto>> GetUserManagementListAsync()
        {
            return await _db.Customers
                .Include(c => c.Role)
                .AsNoTracking()
                .Select(c => new UserResponseDto
                {
                    CustId = c.CustId,
                    UserName = c.UserName,
                    Name = c.Name,
                    Email = c.Email,
                    PhoneNumber = c.PhoneNumber,
                    RoleId = c.RoleId,
                    RoleName = c.Role != null ? c.Role.Name : null,
                    Status = c.Status,
                    RecordStatus = c.RecordStatus,
                    CreatedDate = c.CreatedDate
                })
                .ToListAsync();
        }

        public async Task<UserResponseDto?> GetUserDetailByIdAsync(string id)
        {
            var c = await _db.Customers
                .Include(c => c.Role)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.CustId == id);

            if (c == null) return null;

            return new UserResponseDto
            {
                CustId = c.CustId,
                UserName = c.UserName,
                Name = c.Name,
                Email = c.Email,
                PhoneNumber = c.PhoneNumber,
                RoleId = c.RoleId,
                RoleName = c.Role != null ? c.Role.Name : null,
                Status = c.Status,
                RecordStatus = c.RecordStatus,
                CreatedDate = c.CreatedDate
            };
        }

        public async Task<bool> AdminCreateUserAsync(UserCreateDto dto, string adminUserId)
        {
            // Check if CustId or UserName already exists
            if (await _db.Customers.AnyAsync(x => x.CustId == dto.CustId || x.UserName == dto.UserName))
                return false;

            var newUser = new Customer
            {
                CustId = dto.CustId,
                UserName = dto.UserName,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Name = dto.Name,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                RoleId = dto.RoleId,
                Status = "A", // Active
                RecordStatus = "O", // Open
                CreatedBy = adminUserId,
                CreatedDate = DateTime.Now
            };

            _db.Customers.Add(newUser);
            return await _db.SaveChangesAsync() > 0;
        }

        public async Task<bool> AdminUpdateUserAsync(string id, UserUpdateDto dto, string adminUserId)
        {
            var existing = await _db.Customers.FindAsync(id);
            if (existing == null) return false;

            existing.Name = dto.Name;
            existing.Email = dto.Email;
            existing.PhoneNumber = dto.PhoneNumber;
            if (!string.IsNullOrEmpty(dto.Status)) existing.Status = dto.Status;
            
            existing.LastChangeBy = adminUserId;
            existing.LastChangeDate = DateTime.Now;

            return await _db.SaveChangesAsync() > 0;
        }

        public async Task<bool> ChangeUserRoleAsync(string id, string newRoleId, string adminUserId)
        {
            var existing = await _db.Customers.FindAsync(id);
            if (existing == null) return false;

            // Check if role exists
            var roleExists = await _db.Roles.AnyAsync(r => r.RoleId == newRoleId);
            if (!roleExists) return false;

            existing.RoleId = newRoleId;
            existing.LastChangeBy = adminUserId;
            existing.LastChangeDate = DateTime.Now;

            return await _db.SaveChangesAsync() > 0;
        }

        public async Task<bool> ToggleUserStatusAsync(string id, string adminUserId)
        {
            var existing = await _db.Customers.FindAsync(id);
            if (existing == null) return false;

            existing.Status = (existing.Status == "A") ? "I" : "A"; // Toggle Active/Inactive
            existing.LastChangeBy = adminUserId;
            existing.LastChangeDate = DateTime.Now;

            return await _db.SaveChangesAsync() > 0;
        }
        #endregion
    }
}
