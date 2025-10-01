using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
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
        [JsonConverter(typeof(NullableIntConverter))]
        public int? ServiceCenterId { get; set; } // only for Admin users
        [JsonIgnore]
        public ServiceCenter? ServiceCenter { get; set; }
        
        // Computed property for API response (not stored in DB)
        [NotMapped]
        public string? ServiceCenterName { get; set; }

        // System Fields
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
        public bool DelStatus { get; set; } = false; // Soft delete flag: false = not deleted, true = deleted
    }

    // Custom JSON converter to handle empty strings for nullable int
    public class NullableIntConverter : JsonConverter<int?>
    {
        public override int? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return null;
            }
            
            if (reader.TokenType == JsonTokenType.String)
            {
                var stringValue = reader.GetString();
                if (string.IsNullOrEmpty(stringValue))
                {
                    return null;
                }
                
                if (int.TryParse(stringValue, out var intValue))
                {
                    return intValue;
                }
            }
            
            if (reader.TokenType == JsonTokenType.Number)
            {
                return reader.GetInt32();
            }
            
            throw new JsonException($"Unable to convert to int? from {reader.TokenType}");
        }

        public override void Write(Utf8JsonWriter writer, int? value, JsonSerializerOptions options)
        {
            if (value.HasValue)
            {
                writer.WriteNumberValue(value.Value);
            }
            else
            {
                writer.WriteNullValue();
            }
        }
    }
}
