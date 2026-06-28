using System.Collections.Generic;
using System.Threading.Tasks;
using The_Last_Dance_Project.Dtos;

namespace The_Last_Dance_Project.Interfaces
{
    public interface ICustomerService
    {
        // General CRUD using DTOs
        Task<IEnumerable<UserResponseDto>> GetAllAsync();
        Task<UserResponseDto?> GetByIdAsync(string id);
        Task<UserResponseDto> CreateAsync(UserCreateDto dto, string adminUserId);
        Task<UserResponseDto?> UpdateAsync(string id, UserUpdateDto dto, string adminUserId);
        Task<bool> DeleteAsync(string id);

        // Management specific
        Task<IEnumerable<UserResponseDto>> GetUserManagementListAsync();
        Task<UserResponseDto?> GetUserDetailByIdAsync(string id);
        Task<bool> AdminCreateUserAsync(UserCreateDto dto, string adminUserId);
        Task<bool> AdminUpdateUserAsync(string id, UserUpdateDto dto, string adminUserId);
        Task<bool> ChangeUserRoleAsync(string id, string newRoleId, string adminUserId);
        Task<bool> ToggleUserStatusAsync(string id, string adminUserId);
        // Get customers whose RoleId is "USER"
        Task<IEnumerable<UserResponseDto>> GetUsersWithRoleUserAsync();

        // Client (Khách hàng chứng khoán) - phân hệ theo URD
        Task<string> GenerateNextClientIdAsync();
        Task<UserResponseDto> CreateClientAsync(ClientCreateDto dto, string makerId);

        // Maker-Checker trên bản ghi Client
        Task<bool> ApproveClientAsync(string id, string checkerId);
        Task<bool> RejectClientAsync(string id, string checkerId, string reason);
        Task<bool> RequestDeleteClientAsync(string id, string makerId);
        Task<IEnumerable<Models.AuditEntity>> GetClientAuditAsync(string id);
    }
}
