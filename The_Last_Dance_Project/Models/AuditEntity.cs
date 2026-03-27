using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace The_Last_Dance_Project.Models
{
    public class AuditEntity
    {
        [Key]
        [Column("MTTRANID")]
        public long MtTranId { get; set; } // Mã tự tăng (NUMBER 20)

        [Required]
        [StringLength(50)]
        [Column("OBJCHANGE")]
        public string ObjChange { get; set; } // Đối tượng tác động (Customer, Account...)

        [Required]
        [Column("BUSDATE")]
        public DateTime BusDate { get; set; } // Ngày giao dịch (dd/MM/yyyy)

        [Required]
        [Column("ACTIONDATE")]
        public DateTime ActionDate { get; set; } // Ngày thực tế lưu (dd/MM/yyyy hh24:mi:ss)

        [Required]
        [StringLength(10)]
        [Column("MODCODE")]
        public string ModCode { get; set; } // Phân hệ

        [StringLength(100)]
        [Column("KEYFIELD")]
        public string? KeyField { get; set; } // Key 1 của bảng

        [StringLength(1000)]
        [Column("KEYVALUE")]
        public string? KeyValue { get; set; } // Giá trị 1

        [StringLength(100)]
        [Column("KEYFIELDEXT")]
        public string? KeyFieldExt { get; set; } // Key 2 của bảng

        [StringLength(1000)]
        [Column("KEYVALUEEXT")]
        public string? KeyValueExt { get; set; } // Giá trị 2

        [StringLength(3)]
        [Column("MTLSTATUS")]
        public string? MtlStatus { get; set; } // N: Chờ duyệt, A: Đã duyệt

        [StringLength(3)]
        [Column("MTLTYPE")]
        public string? MtlType { get; set; } // A, AP, APA, APD, APE, C, CA, CD, CE, CV, D, E, R, RA...

        [StringLength(1000)]
        [Column("DESCRIPTION")]
        public string? Description { get; set; } // Mô tả

        [StringLength(50)]
        [Column("MAKER")]
        public string? Maker { get; set; } // Tài khoản tạo

        [StringLength(50)]
        [Column("MAIPADDRESS")]
        public string? MaIpAddress { get; set; } // IP maker

        [StringLength(50)]
        [Column("MAWSNAME")]
        public string? MaWsName { get; set; } // Tên máy thực hiện

        [StringLength(50)]
        [Column("CHECKER")]
        public string? Checker { get; set; } // Tài khoản duyệt

        [StringLength(20)]
        [Column("CHKIPADDRESS")]
        public string? ChkIpAddress { get; set; } // IP checker

        [StringLength(50)]
        [Column("CHKWSNAME")]
        public string? ChkWsName { get; set; } // Tên máy duyệt giao dịch

        // Navigation: danh sách các trường chi tiết của bản ghi audit
        public List<AuditEntityKey> Keys { get; set; } = new();
    }
}
