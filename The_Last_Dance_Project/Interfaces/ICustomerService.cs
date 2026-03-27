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
    }
}
