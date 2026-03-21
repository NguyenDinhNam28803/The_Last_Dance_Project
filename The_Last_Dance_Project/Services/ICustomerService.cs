using System.Collections.Generic;
using System.Threading.Tasks;
using The_Last_Dance_Project.Dto;
using The_Last_Dance_Project.Models;

namespace The_Last_Dance_Project.Services
{
    public interface ICustomerService
    {
        // 1. Core CRUD (For back-end logic)
        Task<IEnumerable<Customer>> GetAllAsync();
        Task<Customer?> GetByIdAsync(string id);
        Task<Customer> CreateAsync(Customer customer);
        Task<Customer?> UpdateAsync(string id, Customer customer);
        Task<bool> DeleteAsync(string id);

        // 2. User Management (For API Layer)
        Task<IEnumerable<UserResponseDto>> GetUserManagementListAsync();
        Task<UserResponseDto?> GetUserDetailByIdAsync(string id);
        Task<bool> AdminCreateUserAsync(UserCreateDto dto, string adminUserId);
        Task<bool> AdminUpdateUserAsync(string id, UserUpdateDto dto, string adminUserId);
        Task<bool> ChangeUserRoleAsync(string id, string newRoleId, string adminUserId);
        Task<bool> ToggleUserStatusAsync(string id, string adminUserId);
    }
}
