using System.ComponentModel.DataAnnotations;

namespace The_Last_Dance_Project.Dtos
{
    public class CustomerContactDto
    {
        public string ContactId { get; set; }
        public string CustId { get; set; }
        public string CountryId { get; set; }
        public string AddType { get; set; } // A: Địa chỉ, S: SMS, E: Email, F: FAX
        public string InfoType { get; set; } // TEP, ARB, HOM, OFC, HTL, OFT, MTL, EML...
        public string Contact { get; set; }
        public string? FaxAttention { get; set; }
        public string IsDefault { get; set; } = "N";
        public string? Description { get; set; }
    }

    public class CustomerContactCreateDto
    {
        [Required]
        [StringLength(50)]
        public string ContactId { get; set; }

        [Required]
        [StringLength(20)]
        public string CustId { get; set; }

        [Required]
        [StringLength(100)]
        public string CountryId { get; set; }

        [Required]
        [StringLength(3)]
        public string AddType { get; set; }

        [Required]
        [StringLength(3)]
        public string InfoType { get; set; }

        [Required]
        [StringLength(100)]
        public string Contact { get; set; }

        [StringLength(50)]
        public string? FaxAttention { get; set; }

        [Required]
        [StringLength(1)]
        public string IsDefault { get; set; } = "N";

        [StringLength(100)]
        public string? Description { get; set; }
    }

    public class CustomerContactUpdateDto
    {
        [Required]
        [StringLength(100)]
        public string CountryId { get; set; }

        [Required]
        [StringLength(3)]
        public string AddType { get; set; }

        [Required]
        [StringLength(3)]
        public string InfoType { get; set; }

        [Required]
        [StringLength(100)]
        public string Contact { get; set; }

        [StringLength(50)]
        public string? FaxAttention { get; set; }

        [Required]
        [StringLength(1)]
        public string IsDefault { get; set; } = "N";

        [StringLength(100)]
        public string? Description { get; set; }
    }
}
