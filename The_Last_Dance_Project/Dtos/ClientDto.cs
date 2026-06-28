using System.ComponentModel.DataAnnotations;

namespace The_Last_Dance_Project.Dtos
{
    /// <summary>
    /// DTO tạo mới Khách hàng (Client) theo URD mục 3.2.
    /// Chỉ chứa các trường hiện đã có cột trong model Customer (các trường giấy tờ
    /// định danh sẽ bổ sung ở giai đoạn mở rộng schema sau).
    /// </summary>
    public class ClientCreateDto
    {
        /// <summary>Mã khách hàng 6 chữ số. Để trống hoặc "#" để hệ thống tự sinh.</summary>
        public string? ClientId { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;

        public string? NameOther { get; set; }
        public string? ShortName { get; set; }

        /// <summary>LOCAL_RETAIL / FOREIGN_RETAIL / LOCAL_INSTITUTION / FOREIGN_INSTITUTION.</summary>
        [Required]
        public string RegistrationType { get; set; } = string.Empty;

        public string? Nationality { get; set; }

        /// <summary>Loại hình tổ chức (bắt buộc với KH tổ chức).</summary>
        public string? InstitutionType { get; set; }

        /// <summary>Mã nhà đầu tư VSD (bắt buộc với KH nước ngoài).</summary>
        public string? InvestorCode { get; set; }

        public string? Gender { get; set; }

        /// <summary>Ngày sinh (dd/MM/yyyy hoặc ISO). Bắt buộc với KH cá nhân, đủ 18 tuổi.</summary>
        public string? DateOfBirth { get; set; }

        public string? PlaceOfBirth { get; set; }
        public string? ResidentCountryId { get; set; }

        /// <summary>Nhân viên công ty (Y/N).</summary>
        public string? IsStaff { get; set; }

        /// <summary>Kênh mở TK (Tại quầy/EKYC/Qua môi giới) -> lưu vào OpenVia.</summary>
        public string? CreationMethod { get; set; }

        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
    }

    /// <summary>Lý do từ chối khi Checker từ chối bản ghi Client.</summary>
    public class ClientRejectDto
    {
        public string Reason { get; set; } = string.Empty;
    }
}
