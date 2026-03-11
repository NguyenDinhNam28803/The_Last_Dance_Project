using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace The_Last_Dance_Project.Models
{
    [Table("SYSTEMCODE")]
    public class SystemCode
    {
        [Key]
        [StringLength(50)]
        [Column("SYSTEMCODEID")]
        public string SystemCodeId { get; set; } // Ví Kong: GENDER, NATION, STATUS...

        [Required]
        [StringLength(100)]
        [Column("NAME")]
        public string Name { get; set; } // Tên nhóm danh mục

        [StringLength(200)]
        [Column("DESCRIPTION")]
        public string? Description { get; set; } // Mô tả

        [StringLength(1)]
        [Column("ISACTIVE")]
        public string IsActive { get; set; } = "Y"; // Y: Hoạt động, N: Ngừng

        // Danh sách các giá trị thuộc nhóm này
        public virtual ICollection<SystemCodeValue>? Values { get; set; }
    }
}