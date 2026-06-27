using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using The_Last_Dance_Project.Data;
using The_Last_Dance_Project.Interfaces;

namespace The_Last_Dance_Project.Services
{
    /// <summary>
    /// Hiện thực các quy tắc sinh mã &amp; nghiệp vụ cho Khách hàng (Client) theo URD mục 3.2.
    /// </summary>
    public class ClientCodeService : IClientCodeService
    {
        private readonly ApplicationDbContext _db;

        public ClientCodeService(ApplicationDbContext db)
        {
            _db = db;
        }

        // URD: ClientID gồm 6 ký tự số; icon # tự sinh giá trị nhỏ nhất chưa tồn tại.
        public async Task<string> GenerateClientIdAsync()
        {
            var existingIds = await _db.Customers
                .Select(c => c.CustId)
                .ToListAsync();

            // Chỉ xét các CustId là số (bỏ qua tài khoản hệ thống dạng ACC_xxx)
            var usedNumbers = new HashSet<int>();
            foreach (var id in existingIds)
            {
                if (!string.IsNullOrEmpty(id) && id.All(char.IsDigit) && int.TryParse(id, out var n))
                {
                    usedNumbers.Add(n);
                }
            }

            int candidate = 1;
            while (usedNumbers.Contains(candidate))
            {
                candidate++;
            }

            return candidate.ToString("D6", CultureInfo.InvariantCulture);
        }

        // URD: 003C + ClientID (trong nước); 003F + ClientID (nước ngoài).
        public string GenerateCustodyId(string registrationType, string clientId)
        {
            var isForeign = IsForeign(registrationType);
            var prefix = isForeign ? "003F" : "003C";
            return prefix + (clientId ?? string.Empty);
        }

        // URD: tự động tick FATCA khi có dấu hiệu Hoa Kỳ ở quốc tịch hoặc địa chỉ.
        public bool DetectFatca(string? nationality, params string?[] addresses)
        {
            var nat = (nationality ?? string.Empty).Trim().ToUpperInvariant();
            if (nat == "US" || nat == "USA" || nat == "UNITED STATES" || nat == "UNITED STATES OF AMERICA")
                return true;

            string[] markers = { "USA", "UNITED STATE", "AMERICA" };
            foreach (var addr in addresses)
            {
                if (string.IsNullOrWhiteSpace(addr)) continue;
                var upper = addr.ToUpperInvariant();
                if (markers.Any(m => upper.Contains(m)))
                    return true;
            }
            return false;
        }

        // URD: KH cá nhân bắt buộc đủ 18 tuổi.
        public bool IsAtLeast18(string? dateOfBirth)
        {
            if (string.IsNullOrWhiteSpace(dateOfBirth)) return false;

            string[] formats = { "dd/MM/yyyy", "yyyy-MM-dd", "yyyy-MM-ddTHH:mm:ss", "MM/dd/yyyy" };
            DateTime dob;
            var parsed = DateTime.TryParseExact(dateOfBirth.Trim(), formats,
                             CultureInfo.InvariantCulture, DateTimeStyles.None, out dob)
                         || DateTime.TryParse(dateOfBirth.Trim(), CultureInfo.InvariantCulture,
                             DateTimeStyles.None, out dob);
            if (!parsed) return false;

            var today = DateTime.UtcNow.Date;
            var age = today.Year - dob.Year;
            if (dob.Date > today.AddYears(-age)) age--;
            return age >= 18;
        }

        private static bool IsForeign(string? registrationType)
        {
            var rt = (registrationType ?? string.Empty).Trim().ToUpperInvariant();
            return rt.Contains("FOREIGN");
        }
    }
}
