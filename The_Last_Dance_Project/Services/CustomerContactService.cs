using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using The_Last_Dance_Project.Data;
using The_Last_Dance_Project.Dtos;
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

        public async Task<IEnumerable<CustomerContactDto>> GetAllAsync()
        {
            return await _db.CustomerContacts
                .AsNoTracking()
                .Select(cc => new CustomerContactDto
                {
                    ContactId = cc.ContactId,
                    CustId = cc.CustId,
                    CountryId = cc.CountryId,
                    AddType = cc.AddType,
                    InfoType = cc.InfoType,
                    Contact = cc.Contact,
                    FaxAttention = cc.FaxAttention,
                    IsDefault = cc.IsDefault,
                    Description = cc.Description
                })
                .ToListAsync();
        }

        public async Task<CustomerContactDto?> GetByIdAsync(string contactId)
        {
            var cc = await _db.CustomerContacts.FindAsync(contactId);
            if (cc == null) return null;

            return new CustomerContactDto
            {
                ContactId = cc.ContactId,
                CustId = cc.CustId,
                CountryId = cc.CountryId,
                AddType = cc.AddType,
                InfoType = cc.InfoType,
                Contact = cc.Contact,
                FaxAttention = cc.FaxAttention,
                IsDefault = cc.IsDefault,
                Description = cc.Description
            };
        }

        public async Task<IEnumerable<CustomerContactDto>> GetByCustomerIdAsync(string custId)
        {
            return await _db.CustomerContacts
                .AsNoTracking()
                .Where(cc => cc.CustId == custId)
                .Select(cc => new CustomerContactDto
                {
                    ContactId = cc.ContactId,
                    CustId = cc.CustId,
                    CountryId = cc.CountryId,
                    AddType = cc.AddType,
                    InfoType = cc.InfoType,
                    Contact = cc.Contact,
                    FaxAttention = cc.FaxAttention,
                    IsDefault = cc.IsDefault,
                    Description = cc.Description
                })
                .ToListAsync();
        }

        public async Task<bool> CreateAsync(CustomerContactCreateDto dto)
        {
            // Kiểm tra khách hàng có tồn tại không
            var customerExists = await _db.Customers.AnyAsync(c => c.CustId == dto.CustId);
            if (!customerExists) return false;

            // Kiểm tra xem ContactId đã tồn tại chưa
            var contactExists = await _db.CustomerContacts.AnyAsync(cc => cc.ContactId == dto.ContactId);
            if (contactExists) return false;

            var newContact = new CustomerContact
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

            // Nếu đặt là mặc định, reset các liên hệ khác cùng loại của khách hàng này
            if (newContact.IsDefault == "Y")
            {
                await ResetDefaultContacts(newContact.CustId, newContact.AddType);
            }

            _db.CustomerContacts.Add(newContact);
            return await _db.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateAsync(string contactId, CustomerContactUpdateDto dto)
        {
            var existing = await _db.CustomerContacts.FindAsync(contactId);
            if (existing == null) return false;

            // Nếu thay đổi sang mặc định, reset các liên hệ khác
            if (dto.IsDefault == "Y" && existing.IsDefault == "N")
            {
                await ResetDefaultContacts(existing.CustId, dto.AddType);
            }

            existing.CountryId = dto.CountryId;
            existing.AddType = dto.AddType;
            existing.InfoType = dto.InfoType;
            existing.Contact = dto.Contact;
            existing.FaxAttention = dto.FaxAttention;
            existing.IsDefault = dto.IsDefault;
            existing.Description = dto.Description;

            return await _db.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(string contactId)
        {
            var existing = await _db.CustomerContacts.FindAsync(contactId);
            if (existing == null) return false;

            _db.CustomerContacts.Remove(existing);
            return await _db.SaveChangesAsync() > 0;
        }

        public async Task<bool> SetDefaultContactAsync(string contactId)
        {
            var contact = await _db.CustomerContacts.FindAsync(contactId);
            if (contact == null) return false;

            await ResetDefaultContacts(contact.CustId, contact.AddType);
            contact.IsDefault = "Y";

            return await _db.SaveChangesAsync() > 0;
        }

        private async Task ResetDefaultContacts(string custId, string addType)
        {
            var defaults = await _db.CustomerContacts
                .Where(cc => cc.CustId == custId && cc.AddType == addType && cc.IsDefault == "Y")
                .ToListAsync();

            foreach (var d in defaults)
            {
                d.IsDefault = "N";
            }
        }
    }
}
