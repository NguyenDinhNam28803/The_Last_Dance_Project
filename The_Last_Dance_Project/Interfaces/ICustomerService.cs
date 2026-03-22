using System.Collections.Generic;
using System.Threading.Tasks;
using The_Last_Dance_Project.Dtos;
using The_Last_Dance_Project.Models;

namespace The_Last_Dance_Project.Interfaces
{
    public interface ICustomerService
    {
        Task<IEnumerable<Customer>> GetAllAsync();
        Task<Customer?> GetByIdAsync(string id);
        Task<Customer> CreateAsync(Customer customer);
        Task<Customer?> UpdateAsync(string id, Customer customer);
        Task<bool> DeleteAsync(string id);

        Task<IEnumerable<UserResponseDto>> GetUserManagementListAsync();
        Task<UserResponseDto?> GetUserDetailByIdAsync(string id);
        Task<bool> AdminCreateUserAsync(UserCreateDto dto, string adminUserId);
        Task<bool> AdminUpdateUserAsync(string id, UserUpdateDto dto, string adminUserId);
        Task<bool> ChangeUserRoleAsync(string id, string newRoleId, string adminUserId);
        Task<bool> ToggleUserStatusAsync(string id, string adminUserId);
    }
}
