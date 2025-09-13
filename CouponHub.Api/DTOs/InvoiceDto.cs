namespace CouponHub.Api.DTOs
{
    public class InvoiceDto
    {
        public int Id { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public int ServiceCenterId { get; set; }
        public string ServiceCenterName { get; set; } = string.Empty;
        public int? CouponId { get; set; }
        public string? CouponCode { get; set; }
        public decimal SubTotal { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty;
        public string PaymentStatus { get; set; } = string.Empty;
        public DateTime? PaidAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Notes { get; set; } = string.Empty;
    }

    public class CreateInvoiceDto
    {
        public int CustomerId { get; set; }
        public int ServiceCenterId { get; set; }
        public int? CouponId { get; set; }
        public decimal SubTotal { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public string Currency { get; set; } = "INR";
        public string PaymentMethod { get; set; } = string.Empty;
        public string PaymentStatus { get; set; } = "Unpaid";
        public string Notes { get; set; } = string.Empty;
    }

    public class UpdateInvoiceDto
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int ServiceCenterId { get; set; }
        public int? CouponId { get; set; }
        public decimal SubTotal { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public string Currency { get; set; } = "INR";
        public string PaymentMethod { get; set; }
        public string PaymentStatus { get; set; }
        public string Notes { get; set; } = string.Empty;
    }
}


