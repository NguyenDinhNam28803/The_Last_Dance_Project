using OfficeOpenXml;
using OfficeOpenXml.Style;
using The_Last_Dance_Project.Data;
using The_Last_Dance_Project.Dtos;
using The_Last_Dance_Project.Interfaces;
using The_Last_Dance_Project.Models;
using Microsoft.EntityFrameworkCore;
using System.Drawing;

namespace The_Last_Dance_Project.Services
{
    public class ImportExportService : IImportExportService
    {
        private readonly ApplicationDbContext _db;

        public ImportExportService(ApplicationDbContext db)
        {
            _db = db;
            // Yêu cầu của thư viện EPPlus cho việc sử dụng phi thương mại
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        public byte[] GenerateTemplate()
        {
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Template");

            // Define Headers
            var headers = new[]
            {
                "Client ID", "Name", "Date Of Birth", "Sex", "Registration Type",
                "ID Type", "ID Number", "Issued Date", "Issued Place", "ID Expiry Date",
                "Address/Home", "Nationality", "Email", "Fax", "Home Tel", "Office Tel",
                "Mobile Tel", "Creation Method"
            };

            for (int i = 0; i < headers.Length; i++)
            {
                var cell = worksheet.Cells[1, i + 1];
                cell.Value = headers[i];
                cell.Style.Font.Bold = true;
                cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                cell.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                cell.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            }

            // Dữ liệu mẫu (Sample row)
            worksheet.Cells[2, 1].Value = "CUST001";
            worksheet.Cells[2, 2].Value = "Nguyen Van A";
            worksheet.Cells[2, 3].Value = "01/01/1990";
            worksheet.Cells[2, 4].Value = "M";
            worksheet.Cells[2, 5].Value = "C";
            worksheet.Cells[2, 6].Value = "CID";
            worksheet.Cells[2, 7].Value = "123456789";
            worksheet.Cells[2, 8].Value = "01/01/2010";
            worksheet.Cells[2, 9].Value = "Hanoi";
            worksheet.Cells[2, 10].Value = "01/01/2030";
            worksheet.Cells[2, 11].Value = "123 ABC Street";
            worksheet.Cells[2, 12].Value = "VN";
            worksheet.Cells[2, 13].Value = "nguyenvana@gmail.com";
            worksheet.Cells[2, 14].Value = "111-222";
            worksheet.Cells[2, 15].Value = "333-444";
            worksheet.Cells[2, 16].Value = "555-666";
            worksheet.Cells[2, 17].Value = "0987654321";
            worksheet.Cells[2, 18].Value = "1";

            worksheet.Cells.AutoFitColumns();
            return package.GetAsByteArray();
        }

        public async Task<byte[]> ExportCustomersAsync()
        {
            var customers = await _db.Customers
                .AsNoTracking()
                .ToListAsync();

            var customerContacts = await _db.CustomerContacts
                .AsNoTracking()
                .ToListAsync();

            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Customers");

            // Define Headers (Trùng với Template 18 cột)
            var headers = new[]
            {
                "Client ID", "Name", "Date Of Birth", "Sex", "Registration Type",
                "ID Type", "ID Number", "Issued Date", "Issued Place", "ID Expiry Date",
                "Address/Home", "Nationality", "Email", "Fax", "Home Tel", "Office Tel",
                "Mobile Tel", "Creation Method"
            };

            for (int i = 0; i < headers.Length; i++)
            {
                var cell = worksheet.Cells[1, i + 1];
                cell.Value = headers[i];
                cell.Style.Font.Bold = true;
                cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                cell.Style.Fill.BackgroundColor.SetColor(Color.LightBlue);
                cell.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            }

            // Đổ dữ liệu
            int rowNumber = 2;
            foreach (var c in customers)
            {
                var contacts = customerContacts.Where(x => x.CustId == c.CustId).ToList();
                var homeAddress = contacts.FirstOrDefault(x => x.AddType == "A" && x.InfoType == "HOM")?.Contact;
                var mobileTel = contacts.FirstOrDefault(x => (x.AddType == "S" || x.InfoType == "MTL") && x.CustId == c.CustId)?.Contact;

                worksheet.Cells[rowNumber, 1].Value = c.CustId;
                worksheet.Cells[rowNumber, 2].Value = c.Name;
                worksheet.Cells[rowNumber, 3].Value = c.DateOfBirth;
                worksheet.Cells[rowNumber, 4].Value = c.Gender;
                worksheet.Cells[rowNumber, 5].Value = c.RegistrationType;
                worksheet.Cells[rowNumber, 6].Value = c.InstitutionTypeId;
                worksheet.Cells[rowNumber, 7].Value = c.InvestorCode;
                worksheet.Cells[rowNumber, 8].Value = ""; // Issued Date (Chưa có trong Model)
                worksheet.Cells[rowNumber, 9].Value = ""; // Issued Place (Chưa có trong Model)
                worksheet.Cells[rowNumber, 10].Value = ""; // ID Expiry Date (Chưa có trong Model)
                worksheet.Cells[rowNumber, 11].Value = homeAddress ?? "";
                worksheet.Cells[rowNumber, 12].Value = c.Nationality;
                worksheet.Cells[rowNumber, 13].Value = c.Email;
                worksheet.Cells[rowNumber, 14].Value = ""; // Fax
                worksheet.Cells[rowNumber, 15].Value = ""; // Home Tel
                worksheet.Cells[rowNumber, 16].Value = ""; // Office Tel
                worksheet.Cells[rowNumber, 17].Value = mobileTel ?? c.PhoneNumber;
                worksheet.Cells[rowNumber, 18].Value = "1"; // Creation Method

                rowNumber++;
            }

            worksheet.Cells.AutoFitColumns();
            return package.GetAsByteArray();
        }

        public async Task<ImportResultDto> ImportCustomersAsync(Stream fileStream, string importedBy)
        {
            var result = new ImportResultDto();

            using var package = new ExcelPackage(fileStream);
            var worksheet = package.Workbook.Worksheets.FirstOrDefault();

            if (worksheet == null)
            {
                result.Errors.Add(new ImportRowErrorDto { RowNumber = 0, Reason = "File Excel không có dữ liệu." });
                return result;
            }

            int rowCount = worksheet.Dimension?.Rows ?? 0;
            if (rowCount <= 1)
            {
                result.Errors.Add(new ImportRowErrorDto { RowNumber = 0, Reason = "File dữ liệu rỗng (chỉ có header)." });
                return result;
            }

            result.TotalRows = rowCount - 1; // Số lượng row trừ header

            // Lưu danh sách ID để check trùng trong file
            var currentFileIds = new HashSet<string>();

            for (int row = 2; row <= rowCount; row++)
            {
                var clientId = worksheet.Cells[row, 1].Text?.Trim();
                var name = worksheet.Cells[row, 2].Text?.Trim();
                var email = worksheet.Cells[row, 13].Text?.Trim();

                // 1. Kiểm tra dòng trống
                if (string.IsNullOrEmpty(clientId) && string.IsNullOrEmpty(name))
                {
                    result.FailCount++;
                    result.Errors.Add(new ImportRowErrorDto { RowNumber = row, Reason = "Dòng trống (Thiếu Client ID và Name)." });
                    continue;
                }

                // 2. Validate Client ID bắt buộc (Dựa vào template)
                if (string.IsNullOrEmpty(clientId))
                {
                    result.FailCount++;
                    result.Errors.Add(new ImportRowErrorDto { RowNumber = row, Reason = "Bắt buộc phải có Client ID." });
                    continue;
                }

                // 3. Trùng ID trong cùng 1 file
                if (currentFileIds.Contains(clientId))
                {
                    result.FailCount++;
                    result.Errors.Add(new ImportRowErrorDto { RowNumber = row, ClientId = clientId, Reason = "Client ID bị trùng lặp trong file tải lên." });
                    continue;
                }
                currentFileIds.Add(clientId);

                // 4. Trùng ID trong CSDL
                bool existsInDb = await _db.Customers.AnyAsync(x => x.CustId == clientId);
                if (existsInDb)
                {
                    result.FailCount++;
                    result.Errors.Add(new ImportRowErrorDto { RowNumber = row, ClientId = clientId, Reason = "Client ID đã tồn tại trên hệ thống." });
                    continue;
                }

                // 5. Kiểm tra định dạng ngày tháng (DateOfBirth - Cột 3)
                var dobText = worksheet.Cells[row, 3].Text?.Trim();
                if (!string.IsNullOrEmpty(dobText))
                {
                    // Các định dạng phổ biến
                    string[] formats = { "dd/MM/yyyy", "MM/dd/yyyy", "yyyy-MM-dd", "dd-MM-yyyy" };
                    if (!DateTime.TryParseExact(dobText, formats, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out _))
                    {
                        result.FailCount++;
                        result.Errors.Add(new ImportRowErrorDto { RowNumber = row, ClientId = clientId, Reason = $"Định dạng ngày sinh không hợp lệ: '{dobText}'. Vui lòng sử dụng dd/MM/yyyy." });
                        continue;
                    }
                }

                try
                {
                    // Tạo bản ghi Customer
                    var newCust = new Customer
                    {
                        CustId = clientId,
                        Name = name ?? "Unknown",
                        DateOfBirth = worksheet.Cells[row, 3].Text?.Trim(),
                        Gender = worksheet.Cells[row, 4].Text?.Trim(),
                        RegistrationType = worksheet.Cells[row, 5].Text?.Trim(),
                        InstitutionTypeId = worksheet.Cells[row, 6].Text?.Trim(), // ID Type
                        InvestorCode = worksheet.Cells[row, 7].Text?.Trim(),      // ID Number
                        Nationality = worksheet.Cells[row, 12].Text?.Trim(),
                        Email = email,
                        PhoneNumber = worksheet.Cells[row, 17].Text?.Trim(),      // Mobile Tel
                        Status = "P", // Pending approval (Do MAKER đưa lên)
                        RecordStatus = "O", // Open
                        UserName = "TMP_" + clientId, // Fake username tạm để mapping
                        CreatedBy = importedBy,
                        CreatedDate = DateTime.Now
                    };

                    _db.Customers.Add(newCust);

                    // Xử lý Contact Address / Phone (Tạo 1 bản ghi Contact mặc định)
                    var address = worksheet.Cells[row, 11].Text?.Trim();
                    if (!string.IsNullOrEmpty(address))
                    {
                        var newContact = new CustomerContact
                        {
                            ContactId = Guid.NewGuid().ToString("N").Substring(0, 20),
                            CustId = clientId,
                            CountryId = newCust.Nationality ?? "VN",
                            AddType = "A",   // Address
                            InfoType = "HOM",// Home
                            Contact = address,
                            IsDefault = "Y"
                        };
                        _db.CustomerContacts.Add(newContact);
                    }

                    // Lưu thay đổi ngay lập tức (Để bắt lỗi DB nếu có)
                    // Nếu muốn Bulk Insert tối ưu thì dùng List<Customer> lưu sau vòng lặp
                    await _db.SaveChangesAsync();
                    result.SuccessCount++;
                }
                catch (Exception ex)
                {
                    result.FailCount++;
                    result.Errors.Add(new ImportRowErrorDto { RowNumber = row, ClientId = clientId, Reason = $"Lỗi hệ thống: {ex.Message}" });
                }
            }

            return result;
        }
    }
}
