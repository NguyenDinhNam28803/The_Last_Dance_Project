using System.Collections.Generic;
using System.Threading.Tasks;
using The_Last_Dance_Project.Models;

namespace The_Last_Dance_Project.Services
{
    public interface ICustomerService
    {
        Task<IEnumerable<Customer>> GetAllAsync();
        Task<Customer?> GetByIdAsync(string id);
        Task<Customer> CreateAsync(Customer customer);
        Task<Customer?> UpdateAsync(string id, Customer customer);
        Task<bool> DeleteAsync(string id);
    }
}
