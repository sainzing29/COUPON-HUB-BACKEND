namespace CouponHub.Business.DTOs
{
    public class SalesTrendDto
    {
        public string Month { get; set; } = string.Empty;
        public int Year { get; set; }
        public decimal Revenue { get; set; }
        public int CouponsSold { get; set; }
    }
}

