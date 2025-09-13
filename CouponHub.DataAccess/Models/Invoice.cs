using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CouponHub.DataAccess.Models
{
    public class Invoice
    {
        [Key]
        public int Id { get; set; }

        // Human-friendly unique invoice number (e.g. INV-2025-0001)
        [Required]
        public string InvoiceNumber { get; set; } = string.Empty;

        // Customer who is billed
        [Required]
        public int CustomerId { get; set; }
        public Customer Customer { get; set; } = null!;

        // Coupon (optional) - invoice may be related to a coupon usage
        public int? CouponId { get; set; }
        public Coupon? Coupon { get; set; }

        // Service center where the service was provided
        [Required]
        public int ServiceCenterId { get; set; }
        public ServiceCenter ServiceCenter { get; set; } = null!;

        // Invoice can be linked to multiple redemptions (aggregate invoice)
        public ICollection<ServiceRedemption> ServiceRedemptions { get; set; } = new List<ServiceRedemption>();

        // Monetary fields
        [Column(TypeName = "decimal(18,2)")]
        public decimal SubTotal { get; set; } = 0m;

        [Column(TypeName = "decimal(18,2)")]
        public decimal TaxAmount { get; set; } = 0m;

        [Column(TypeName = "decimal(18,2)")]
        public decimal DiscountAmount { get; set; } = 0m;

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; } = 0m;

        // Currency (optional)
        public string Currency { get; set; } = "INR";

        // Payment / status info
        public string PaymentMethod { get; set; } = string.Empty; // e.g. Cash, Card, UPI, Wallet
        public string PaymentStatus { get; set; } = "Unpaid"; // Unpaid, Paid, Pending, Refunded
        public DateTime? PaidAt { get; set; }

        // Metadata
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsDeleted { get; set; } = false;
        public string Notes { get; set; } = string.Empty;
    }
}
