namespace CouponHub.Api.DTOs
{
    public class ServiceRedemptionDto
    {
        public int Id { get; set; }
        public int CouponId { get; set; }
        public string CouponCode { get; set; } = string.Empty;
        public int ServiceCenterId { get; set; }
        public string ServiceCenterName { get; set; } = string.Empty;
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public DateTime RedemptionDate { get; set; }
        public string Notes { get; set; } = string.Empty;
        public int? InvoiceId { get; set; }
        public string? InvoiceNumber { get; set; }
    }

    public class CreateServiceRedemptionDto
    {
        public int CouponId { get; set; }
        public int ServiceCenterId { get; set; }
        public int CustomerId { get; set; }
        public string Notes { get; set; } = string.Empty;
        public int? InvoiceId { get; set; }
    }

    public class UpdateServiceRedemptionDto
    {
        public int Id { get; set; }
        public int CouponId { get; set; }
        public int ServiceCenterId { get; set; }
        public int CustomerId { get; set; }
        public string Notes { get; set; } = string.Empty;
        public int? InvoiceId { get; set; }
    }
}


