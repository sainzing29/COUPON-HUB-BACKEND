using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CouponHub.DataAccess.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        // Basic Info
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string MobileNumber { get; set; } = string.Empty;

        // Login & Security
        public string Role { get; set; } = "Admin"; // SuperAdmin, Admin

        public string? PasswordHash { get; set; }

        // For Customer (OTP or Google)
    

        // Relations
        public int? ServiceCenterId { get; set; } // only for Admin users
        [JsonIgnore]
        public ServiceCenter? ServiceCenter { get; set; }
        
        // Computed property for API response (not stored in DB)
        [NotMapped]
        public string? ServiceCenterName { get; set; }

        // System Fields
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
    }
}
