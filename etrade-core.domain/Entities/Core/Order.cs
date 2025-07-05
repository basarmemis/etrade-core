using etrade_core.domain.Entities.Base;

namespace etrade_core.domain.Entities.Core
{
    public class Order : BaseEntity<long>
    {
        public string OrderNumber { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        
        // Foreign key for UserProfile relationship (köprü entity)
        public long UserProfileId { get; set; }
        
        // Navigation properties
        public UserProfile UserProfile { get; set; } = null!;
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
} 