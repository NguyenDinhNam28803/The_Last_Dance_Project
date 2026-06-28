using Microsoft.EntityFrameworkCore;
using The_Last_Dance_Project.Data;
using The_Last_Dance_Project.Dtos;
using The_Last_Dance_Project.Models;
using The_Last_Dance_Project.Interfaces;
using The_Last_Dance_Project.Constants;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using BCrypt.Net;

namespace The_Last_Dance_Project.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ApplicationDbContext _db;
        private readonly IClientCodeService _clientCodeService;

        public CustomerService(ApplicationDbContext db, IClientCodeService clientCodeService)
        {
            _db = db;
            _clientCodeService = clientCodeService;
        }

        #region Helpers
        private UserResponseDto MapToResponseDto(Customer c)
        {
            return new UserResponseDto
            {
                CustId = c.CustId,
                UserName = c.UserName,
                Name = c.Name,
                NameOther = c.NameOther,
                ShortName = c.ShortName,
                Email = c.Email,
                PhoneNumber = c.PhoneNumber,
                RoleId = c.RoleId,
                RoleName = c.Role != null ? c.Role.Name : null,
                Status = c.Status,
                RecordStatus = c.RecordStatus,
                Gender = c.Gender,
                DateOfBirth = c.DateOfBirth,
                Nationality = c.Nationality,
                ResidentCountryId = c.ResidentCountryId,
                CreatedDate = c.CreatedDate,
                CreatedBy = c.CreatedBy
            };
        }
        #endregion

        #region Core CRUD
        public async Task<IEnumerable<UserResponseDto>> GetAllAsync()
        {
            var customers = await _db.Customers.Include(c => c.Role).AsNoTracking().ToListAsync();
            return customers.Select(MapToResponseDto);
        }


        public async Task<UserResponseDto?> GetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id)) return null;
            var customer = await _db.Customers.Include(c => c.Role).AsNoTracking().FirstOrDefaultAsync(c => c.CustId == id);
            return customer != null ? MapToResponseDto(customer) : null;
        }

        public async Task<UserResponseDto> CreateAsync(UserCreateDto dto, string adminUserId)
        {
            var newUser = new Customer
            {
                CustId = dto.CustId,
                UserName = dto.UserName,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Name = dto.Name,
                NameOther = dto.NameOther,
                ShortName = dto.ShortName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                RoleId = dto.RoleId,
                Gender = dto.Gender,
                DateOfBirth = dto.DateOfBirth,
                Nationality = dto.Nationality,
                ResidentCountryId = dto.ResidentCountryId,
                Status = "A",
                RecordStatus = "O",
                CreatedBy = adminUserId,
                CreatedDate = DateTime.UtcNow
            };

            _db.Customers.Add(newUser);
            await _db.SaveChangesAsync();
            
            // Reload to get role info
            return await GetByIdAsync(newUser.CustId) ?? MapToResponseDto(newUser);
        }

        public async Task<UserResponseDto?> UpdateAsync(string id, UserUpdateDto dto, string adminUserId)
        {
            var existing = await _db.Customers.FindAsync(id);
            if (existing == null) return null;

            existing.Name = dto.Name;
            existing.NameOther = dto.NameOther;
            existing.ShortName = dto.ShortName;
            existing.Email = dto.Email;
            existing.PhoneNumber = dto.PhoneNumber;
            existing.Gender = dto.Gender;
            existing.DateOfBirth = dto.DateOfBirth;
            existing.Nationality = dto.Nationality;
            existing.ResidentCountryId = dto.ResidentCountryId;
            if (!string.IsNullOrEmpty(dto.Status)) existing.Status = dto.Status;
            
            existing.LastChangeBy = adminUserId;
            existing.LastChangeDate = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            return await GetByIdAsync(id);
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var existing = await _db.Customers.FindAsync(id);
            if (existing == null) return false;
            _db.Customers.Remove(existing);
            return await _db.SaveChangesAsync() > 0;
        }
        #endregion

        #region User Management (Redirecting to Core)
        public async Task<IEnumerable<UserResponseDto>> GetUserManagementListAsync() => await GetAllAsync();

        public async Task<UserResponseDto?> GetUserDetailByIdAsync(string id) => await GetByIdAsync(id);

        public async Task<bool> AdminCreateUserAsync(UserCreateDto dto, string adminUserId)
        {
            var result = await CreateAsync(dto, adminUserId);
            return result != null;
        }

        public async Task<bool> AdminUpdateUserAsync(string id, UserUpdateDto dto, string adminUserId)
        {
            var result = await UpdateAsync(id, dto, adminUserId);
            return result != null;
        }

        public async Task<bool> ChangeUserRoleAsync(string id, string newRoleId, string adminUserId)
        {
            var existing = await _db.Customers.FindAsync(id);
            if (existing == null) return false;

            var roleExists = await _db.Roles.AnyAsync(r => r.RoleId == newRoleId);
            if (!roleExists) return false;

            existing.RoleId = newRoleId;
            existing.LastChangeBy = adminUserId;
            existing.LastChangeDate = DateTime.UtcNow;

            return await _db.SaveChangesAsync() > 0;
        }

        public async Task<bool> ToggleUserStatusAsync(string id, string adminUserId)
        {
            var existing = await _db.Customers.FindAsync(id);
            if (existing == null) return false;

            existing.Status = (existing.Status == "A") ? "I" : "A";
            existing.LastChangeBy = adminUserId;
            existing.LastChangeDate = DateTime.UtcNow;

            return await _db.SaveChangesAsync() > 0;
        }
        #endregion

        // Lấy danh sách khách hàng có Role = "USER"
        public async Task<IEnumerable<UserResponseDto>> GetUsersWithRoleUserAsync()
        {
            var customers = await _db.Customers
                .Include(c => c.Role)
                .Where(c => c.RoleId == "USER")
                .AsNoTracking()
                .ToListAsync();

            return customers.Select(MapToResponseDto);
        }

        #region Client (Phân hệ Khách hàng chứng khoán - URD)

        // Sinh ClientID 6 số nhỏ nhất chưa tồn tại (icon #)
        public async Task<string> GenerateNextClientIdAsync()
        {
            return await _clientCodeService.GenerateClientIdAsync();
        }

        // Tạo mới Khách hàng: tự sinh ClientID/CustodyID/FATCA, đặt trạng thái Chờ duyệt thêm
        public async Task<UserResponseDto> CreateClientAsync(ClientCreateDto dto, string makerId)
        {
            // 1. Giải quyết ClientID: "#"/rỗng -> tự sinh
            var clientId = dto.ClientId;
            if (string.IsNullOrWhiteSpace(clientId) || clientId == "#")
            {
                clientId = await _clientCodeService.GenerateClientIdAsync();
            }

            // 2. Kiểm tra trùng mã
            if (await _db.Customers.AnyAsync(c => c.CustId == clientId))
            {
                throw new InvalidOperationException($"Mã khách hàng '{clientId}' đã tồn tại.");
            }

            // 3. KH cá nhân phải đủ 18 tuổi
            var isIndividual = (dto.RegistrationType ?? string.Empty).ToUpperInvariant().Contains("RETAIL");
            if (isIndividual && !_clientCodeService.IsAtLeast18(dto.DateOfBirth))
            {
                throw new InvalidOperationException("Khách hàng cá nhân phải đủ 18 tuổi.");
            }

            // 4. Tự sinh CustodyID và phát hiện FATCA
            var custodyId = _clientCodeService.GenerateCustodyId(dto.RegistrationType, clientId);
            var fatca = _clientCodeService.DetectFatca(dto.Nationality) ? "Y" : "N";

            var client = new Customer
            {
                CustId = clientId,
                UserName = clientId,           // KH chưa có tài khoản đăng nhập -> dùng mã làm định danh
                Name = dto.Name,
                NameOther = dto.NameOther,
                ShortName = dto.ShortName,
                RegistrationType = dto.RegistrationType,
                Nationality = dto.Nationality,
                InstitutionTypeId = dto.InstitutionType,
                InvestorCode = dto.InvestorCode,
                Gender = dto.Gender,
                DateOfBirth = dto.DateOfBirth,
                PlaceOfBirth = dto.PlaceOfBirth,
                ResidentCountryId = dto.ResidentCountryId,
                IsStaff = dto.IsStaff,
                OpenVia = dto.CreationMethod,
                CustodyCd = custodyId,
                FATCA = fatca,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                Status = "Active",
                RecordStatus = RecordStatus.PendingInsert, // PI: Chờ duyệt thêm
                RoleId = "USER",
                CreatedBy = makerId,
                CreatedDate = DateTime.UtcNow
            };

            _db.Customers.Add(client);
            await _db.SaveChangesAsync();

            return await GetByIdAsync(clientId) ?? MapToResponseDto(client);
        }

        #endregion
    }
}
