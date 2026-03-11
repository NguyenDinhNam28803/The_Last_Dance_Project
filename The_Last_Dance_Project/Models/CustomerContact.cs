using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace The_Last_Dance_Project.Models
{
    [Table("CUSTOMER_CONTACT")]
    public class CustomerContact
    {
        [Key]
        [StringLength(50)]
        public string ContactId { get; set; } // ID gốc của bản ghi

        [Required]
        [StringLength(20)]
        public string CustId { get; set; } // Khóa ngoại liên kết với Customer

        [Required]
        [StringLength(100)]
        public string CountryId { get; set; }

        [Required]
        [StringLength(3)]
        public string AddType { get; set; } // A: Địa chỉ, S: SMS, E: Email, F: FAX

        [Required]
        [StringLength(3)]
        public string InfoType { get; set; } // TEP, ARB, HOM, OFC, HTL, OFT, MTL, EML...

        [Required]
        [StringLength(100)]
        public string Contact { get; set; } // Nội dung địa chỉ/SĐT/Email

        [StringLength(50)]
        public string? FaxAttention { get; set; } // Hiển thị khi AddType = 'Fax'

        [Required]
        [StringLength(1)]
        public string IsDefault { get; set; } = "N"; // N: Thông thường, Y: Mặc định

        [StringLength(100)]
        public string? Description { get; set; } // Mô tả thêm

        // Thiết lập quan hệ (Navigation Property) - Tùy chọn
        [ForeignKey("CustId")]
        public virtual Customer? Customer { get; set; }
    }
}