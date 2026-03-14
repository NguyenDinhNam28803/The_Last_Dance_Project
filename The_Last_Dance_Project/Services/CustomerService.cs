using Microsoft.EntityFrameworkCore;
using The_Last_Dance_Project.Data;
using The_Last_Dance_Project.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace The_Last_Dance_Project.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ApplicationDbContext _db;

        public CustomerService(ApplicationDbContext db)
        {
            _db = db;
        }

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

            // Copy updatable fields - keep it simple: replace scalar props except key
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
    }
}
