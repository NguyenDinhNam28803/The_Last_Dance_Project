using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace The_Last_Dance_Project.Models
{
    [Table("SYSTEMCODE_VALUE")]
    public class SystemCodeValue
    {
        [Key]
        [Column("ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; } // ID tự tăng

        [Required]
        [StringLength(50)]
        [Column("SYSTEMCODEID")]
        public string SystemCodeId { get; set; } // Link tới bảng SYSTEMCODE

        [Required]
        [StringLength(50)]
        [Column("CODEVALUE")]
        public string CodeValue { get; set; } // Mã giá trị (Ví dụ: 'M', 'F' trong GENDER)

        [Required]
        [StringLength(200)]
        [Column("DISPLAYVALUE")]
        public string DisplayValue { get; set; } // Tên hiển thị tiếng Việt (Nam, Nữ)

        [StringLength(200)]
        [Column("DISPLAYVALUEEN")]
        public string? DisplayValueEn { get; set; } // Tên hiển thị tiếng Anh

        [Column("ORDERBY")]
        public int? OrderBy { get; set; } // Thứ tự sắp xếp hiển thị

        [StringLength(1)]
        [Column("ISDEFAULT")]
        public string IsDefault { get; set; } = "N"; // Y: Mặc định chọn

        [ForeignKey("SystemCodeId")]
        public virtual SystemCode? SystemCode { get; set; }
    }
}