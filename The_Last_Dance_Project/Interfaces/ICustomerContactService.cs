using System.Collections.Generic;
using System.Threading.Tasks;
using The_Last_Dance_Project.Dtos;
using The_Last_Dance_Project.Models;

namespace The_Last_Dance_Project.Services
{
    public interface ICustomerContactService
    {
        Task<IEnumerable<CustomerContactDto>> GetAllAsync();
        Task<CustomerContactDto?> GetByIdAsync(string contactId);
        Task<IEnumerable<CustomerContactDto>> GetByCustomerIdAsync(string custId);
        Task<bool> CreateAsync(CustomerContactCreateDto dto);
        Task<bool> UpdateAsync(string contactId, CustomerContactUpdateDto dto);
        Task<bool> DeleteAsync(string contactId);
        Task<bool> SetDefaultContactAsync(string contactId);
    }
}
