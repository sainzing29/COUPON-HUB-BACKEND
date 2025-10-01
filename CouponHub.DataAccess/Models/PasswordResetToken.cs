using System.ComponentModel.DataAnnotations;

namespace CouponHub.DataAccess.Models
{
    public class PasswordResetToken
    {
        [Key]
        public int Id { get; set; }
        
        public int UserId { get; set; }
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiryDate { get; set; }
        public bool IsUsed { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation property
        public User User { get; set; } = null!;
    }
}
