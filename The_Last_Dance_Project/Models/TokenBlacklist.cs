using System.ComponentModel.DataAnnotations;

namespace The_Last_Dance_Project.Models
{
    public class TokenBlacklist
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Token { get; set; } = string.Empty;

        public DateTime BlacklistedAt { get; set; } = DateTime.UtcNow;

        public DateTime ExpiryDate { get; set; }
    }
}
