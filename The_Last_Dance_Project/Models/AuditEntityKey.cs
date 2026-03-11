using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace The_Last_Dance_Project.Models
{
    [Table("AUDITENTITY_KEY")]
    public class AuditEntityKey
    {
        [Key]
        [Column("MTTRANFLDID")]
        public long MtTranFldId { get; set; } // Số tự tăng

        [Required]
        [Column("MTTRANID")]
        public long MtTranId { get; set; } // Join với MTTRAN

        [StringLength(50)]
        [Column("COLUMNNAME")]
        public string? ColumnName { get; set; } // Tên cột dưới database

        [StringLength(200)]
        [Column("DISPLAYNAME")]
        public string? DisplayName { get; set; } // Tên tiếng Việt dùng để hiển thị

        [StringLength(200)]
        [Column("DISPLAYNAMEEN")]
        public string? DisplayNameEn { get; set; } // Tên ngôn ngữ khác dùng để hiển thị

        [StringLength(4000)]
        [Column("OLDVAL")]
        public string? OldVal { get; set; } // Giá trị cũ

        [StringLength(4000)]
        [Column("NEWVAL")]
        public string? NewVal { get; set; } // Giá trị mới sau khi sửa

        [StringLength(4000)]
        [Column("DISPLAYVALOLD")]
        public string? DisplayValOld { get; set; } // Giá trị hiển thị cũ trên màn hình

        [StringLength(4000)]
        [Column("DISPLAYVALNEW")]
        public string? DisplayValNew { get; set; } // Giá trị hiển thị mới trên màn hình

        [StringLength(2000)]
        [Column("DESCRIPTION")]
        public string? Description { get; set; } // Mô tả thêm

        [StringLength(200)]
        [Column("KEYMAP")]
        public string? KeyMap { get; set; } // Map quan hệ các đối tượng bị thay đổi

        [StringLength(200)]
        [Column("KEYMAPEN")]
        public string? KeyMapEn { get; set; } // Map quan hệ (ngôn ngữ khác)

        [StringLength(100)]
        [Column("TABLENAME")]
        public string? TableName { get; set; } // Tên bảng DB bị tác động

        [StringLength(200)]
        [Column("TABNAME")]
        public string? TabName { get; set; } // Tên tab thông tin trên giao diện bị tác động

        [StringLength(200)]
        [Column("TABNAMEEN")]
        public string? TabNameEn { get; set; } // Tên tab (ngôn ngữ khác)

        [StringLength(4000)]
        [Column("OLDVALEN")]
        public string? OldValEn { get; set; } // Giá trị cũ ngôn ngữ khác

        [StringLength(4000)]
        [Column("NEWVALEN")]
        public string? NewValEn { get; set; } // Giá trị mới ngôn ngữ khác

        // Quan hệ Navigation
        [ForeignKey("MtTranId")]
        public virtual Auditentity? Auditentity { get; set; }
    }
}