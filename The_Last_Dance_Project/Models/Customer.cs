using System.ComponentModel.DataAnnotations;

namespace The_Last_Dance_Project.Models
{
    public class Customer
    {
        [Key]
        [StringLength(20)]
        public string CustId { get; set; }

        [Required]
        [StringLength(3)]
        public string RecordStatus { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        public string? NameOther { get; set; }
        public string? ShortName { get; set; }
        public string? RegistrationType { get; set; }
        public string? Nationality { get; set; }
        public string? SignPaths { get; set; }
        public string? InstitutionTypeId { get; set; }
        public string? InvestorCode { get; set; }
        public string? Gender { get; set; }
        public string? DateOfBirth { get; set; }
        public string? PlaceOfBirth { get; set; }
        public string? ResidentCountryId { get; set; }
        public string? FATCA { get; set; }
        public string? CustodyCd { get; set; }
        public string? IsStaff { get; set; }
        public string? OpenDate { get; set; }
        public string? CloseDate { get; set; }
        public string? Status { get; set; }
        public string? OpenVia { get; set; }
        public string? RejectDes { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? LastChangeBy { get; set; }
        public DateTime? LastChangeDate { get; set; }
        public string? ApproveBy { get; set; }
        public DateTime? ApproveDate { get; set; }
    }
}