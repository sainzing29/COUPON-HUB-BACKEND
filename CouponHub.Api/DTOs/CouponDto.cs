namespace CouponHub.Api.DTOs
{
    public class CouponDto
    {
        public int Id { get; set; }
        public string CouponCode { get; set; } = string.Empty;
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public int TotalServices { get; set; }
        public int UsedServices { get; set; }
        public int RemainingServices { get; set; }
        public DateTime PurchaseDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public bool IsValid { get; set; }
        public int RedemptionCount { get; set; }
    }

    public class CreateCouponDto
    {
        public int CustomerId { get; set; }
        public int TotalServices { get; set; } = 5;
        public DateTime ExpiryDate { get; set; }
        public string Status { get; set; } = "Active";
    }

    public class UpdateCouponDto
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int TotalServices { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string Status { get; set; }
    }

    public class RedeemCouponDto
    {
        public int CouponId { get; set; }
        public int ServiceCenterId { get; set; }
        public int CustomerId { get; set; }
        public string Notes { get; set; } = string.Empty;
    }
}























