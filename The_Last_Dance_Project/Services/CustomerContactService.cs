using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using The_Last_Dance_Project.Data;
using The_Last_Dance_Project.Dtos;
using The_Last_Dance_Project.Interfaces;
using The_Last_Dance_Project.Models;

namespace The_Last_Dance_Project.Services
{
    public class CustomerContactService : ICustomerContactService
    {
        private readonly ApplicationDbContext _db;

        public CustomerContactService(ApplicationDbContext db)
        {
            _db = db;
        }

        private CustomerContactDto MapToDto(CustomerContact c)
        {
            return new CustomerContactDto
            {
                ContactId = c.ContactId,
                CustId = c.CustId,
                CountryId = c.CountryId,
                AddType = c.AddType,
                InfoType = c.InfoType,
                Contact = c.Contact,
                FaxAttention = c.FaxAttention,
                IsDefault = c.IsDefault,
                Description = c.Description
            };
        }

        public async Task<IEnumerable<CustomerContactDto>> GetAllAsync()
        {
            var contacts = await _db.CustomerContacts.AsNoTracking().ToListAsync();
            return contacts.Select(MapToDto);
        }

        public async Task<CustomerContactDto?> GetByIdAsync(string id)
        {
            var contact = await _db.CustomerContacts.FindAsync(id);
            return contact != null ? MapToDto(contact) : null;
        }

        public async Task<IEnumerable<CustomerContactDto>> GetByCustomerIdAsync(string custId)
        {
            var contacts = await _db.CustomerContacts
                .Where(c => c.CustId == custId)
                .AsNoTracking()
                .ToListAsync();
            return contacts.Select(MapToDto);
        }

        public async Task<CustomerContactDto> CreateAsync(CustomerContactCreateDto dto)
        {
            var contact = new CustomerContact
            {
                ContactId = dto.ContactId,
                CustId = dto.CustId,
                CountryId = dto.CountryId,
                AddType = dto.AddType,
                InfoType = dto.InfoType,
                Contact = dto.Contact,
                FaxAttention = dto.FaxAttention,
                IsDefault = dto.IsDefault,
                Description = dto.Description
            };

            _db.CustomerContacts.Add(contact);
            await _db.SaveChangesAsync();
            return MapToDto(contact);
        }

        public async Task<CustomerContactDto?> UpdateAsync(string id, CustomerContactUpdateDto dto)
        {
            var existing = await _db.CustomerContacts.FindAsync(id);
            if (existing == null) return null;

            existing.CountryId = dto.CountryId;
            existing.AddType = dto.AddType;
            existing.InfoType = dto.InfoType;
            existing.Contact = dto.Contact;
            existing.FaxAttention = dto.FaxAttention;
            existing.IsDefault = dto.IsDefault;
            existing.Description = dto.Description;

            await _db.SaveChangesAsync();
            return MapToDto(existing);
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var existing = await _db.CustomerContacts.FindAsync(id);
            if (existing == null) return false;

            _db.CustomerContacts.Remove(existing);
            return await _db.SaveChangesAsync() > 0;
        }

        public async Task<bool> SetDefaultAsync(string id)
        {
            var target = await _db.CustomerContacts.FindAsync(id);
            if (target == null) return false;

            // Reset other defaults for the same customer and same AddType
            var otherDefaults = await _db.CustomerContacts
                .Where(c => c.CustId == target.CustId && c.AddType == target.AddType && c.IsDefault == "Y")
                .ToListAsync();

            foreach (var item in otherDefaults)
            {
                item.IsDefault = "N";
            }

            target.IsDefault = "Y";
            return await _db.SaveChangesAsync() > 0;
        }
    }
}
