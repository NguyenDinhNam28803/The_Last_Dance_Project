using System.ComponentModel.DataAnnotations;

namespace The_Last_Dance_Project.Dto
{
    public class UserResponseDto
    {
        public string CustId { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? RoleId { get; set; }
        public string? RoleName { get; set; }
        public string? Status { get; set; }
        public string? RecordStatus { get; set; }
        public DateTime? CreatedDate { get; set; }
    }

    public class UserCreateDto
    {
        [Required]
        [StringLength(20)]
        public string CustId { get; set; }

        [Required]
        [StringLength(50)]
        public string UserName { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        public string? PhoneNumber { get; set; }

        [Required]
        public string RoleId { get; set; }
    }

    public class UserUpdateDto
    {
        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        public string? PhoneNumber { get; set; }

        public string? Status { get; set; }
    }

    public class ChangeRoleDto
    {
        [Required]
        public string RoleId { get; set; }
    }
}
