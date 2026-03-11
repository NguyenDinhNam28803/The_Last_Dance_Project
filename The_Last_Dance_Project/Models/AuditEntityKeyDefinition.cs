using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace The_Last_Dance_Project.Models
{
    [Table("AUDITENTITY_KEY_DEFINITION")]
    public class AuditEntityKeyDefinition
    {
        [Key]
        [Column("DEFID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DefId { get; set; } // ID tự tăng để quản lý định nghĩa

        [Required]
        [StringLength(50)]
        [Column("TABLENAME")]
        public string TableName { get; set; } // Tên bảng DB cần theo dõi

        [Required]
        [StringLength(50)]
        [Column("COLUMNNAME")]
        public string ColumnName { get; set; } // Tên cột cần theo dõi

        [StringLength(200)]
        [Column("DISPLAYNAME")]
        public string? DisplayName { get; set; } // Tên hiển thị (Tiếng Việt)

        [StringLength(200)]
        [Column("DISPLAYNAMEEN")]
        public string? DisplayNameEn { get; set; } // Tên hiển thị (Ngôn ngữ khác)

        [StringLength(200)]
        [Column("TABNAME")]
        public string? TabName { get; set; } // Tên Tab trên giao diện chứa trường này

        [StringLength(200)]
        [Column("TABNAMEEN")]
        public string? TabNameEn { get; set; } // Tên Tab (Ngôn ngữ khác)

        [StringLength(1)]
        [Column("ISCOMPARE")]
        public string? IsCompare { get; set; } // Y/N: Có so sánh để lưu log hay không

        [StringLength(10)]
        [Column("DATATYPE")]
        public string? DataType { get; set; } // Kiểu dữ liệu (VARCHAR2, DATE, NUMBER...)

        [StringLength(100)]
        [Column("LOOKUPTABLE")]
        public string? LookupTable { get; set; } // Bảng để map code thành tên hiển thị (nếu có)

        [StringLength(1000)]
        [Column("DESCRIPTION")]
        public string? Description { get; set; } // Mô tả thêm về định nghĩa này
    }
}