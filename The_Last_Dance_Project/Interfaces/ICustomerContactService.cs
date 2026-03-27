using System.Collections.Generic;
using System.Threading.Tasks;
using The_Last_Dance_Project.Dtos;

namespace The_Last_Dance_Project.Interfaces
{
    public interface ICustomerContactService
    {
        Task<IEnumerable<CustomerContactDto>> GetAllAsync();
        Task<CustomerContactDto?> GetByIdAsync(string id);
        Task<IEnumerable<CustomerContactDto>> GetByCustomerIdAsync(string custId);
        Task<CustomerContactDto> CreateAsync(CustomerContactCreateDto dto);
        Task<CustomerContactDto?> UpdateAsync(string id, CustomerContactUpdateDto dto);
        Task<bool> DeleteAsync(string id);
        Task<bool> SetDefaultAsync(string id);
        Task<bool> ApplyMakerCheckerAsync(string action, string details);
    }
}
