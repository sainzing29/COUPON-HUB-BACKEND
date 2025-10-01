namespace CouponHub.DataAccess.Models
{
    public class ServiceCenter
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string ContactNumber { get; set; } = string.Empty;

        // Relations
        public ICollection<User> Admins { get; set; } = new List<User>();
        public ICollection<Coupon> Coupons { get; set; } = new List<Coupon>();
        public ICollection<ServiceRedemption> Redemptions { get; set; } = new List<ServiceRedemption>();
        public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
    }
}
