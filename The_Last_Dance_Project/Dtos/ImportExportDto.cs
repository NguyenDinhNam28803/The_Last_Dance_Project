namespace The_Last_Dance_Project.Dtos
{
    /// <summary>
    /// DTO dùng để đọc dữ liệu từ file Excel khi Import khách hàng.
    /// Các cột khớp với template Import.Client.
    /// </summary>
    public class CustomerImportRowDto
    {
        public string? ClientId { get; set; }           // Client ID (optional: nếu để trống, sẽ tự sinh)
        public string? Name { get; set; }               // Name
        public string? DateOfBirth { get; set; }        // Date Of Birth (dd/MM/yyyy)
        public string? Sex { get; set; }                // M / F
        public string? RegistrationType { get; set; }  // Registration Type
        public string? IdType { get; set; }             // ID Type (CID, PP, ...)
        public string? IdNumber { get; set; }           // ID Number
        public string? IssuedDate { get; set; }         // Issued Date (dd/MM/yyyy)
        public string? IssuedPlace { get; set; }        // Issued Place
        public string? IdExpiryDate { get; set; }       // ID Expiry Date (dd/MM/yyyy)
        public string? Address { get; set; }            // Address / Home
        public string? Nationality { get; set; }        // Nationality
        public string? Email { get; set; }              // Email
        public string? Fax { get; set; }                // Fax
        public string? HomeTel { get; set; }            // Home Tel
        public string? OfficeTel { get; set; }          // Office Tel
        public string? MobileTel { get; set; }          // Mobile Tel
        public string? CreationMethod { get; set; }     // Creation Method
    }

    /// <summary>
    /// Kết quả trả về sau khi xử lý Import.
    /// </summary>
    public class ImportResultDto
    {
        public int TotalRows { get; set; }
        public int SuccessCount { get; set; }
        public int FailCount { get; set; }
        public List<ImportRowErrorDto> Errors { get; set; } = new();
    }

    /// <summary>
    /// Thông tin lỗi từng dòng khi Import thất bại.
    /// </summary>
    public class ImportRowErrorDto
    {
        public int RowNumber { get; set; }
        public string? ClientId { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}
