using etrade_core.domain.Entities.Base;
using etrade_core.domain.UserModule.Entities;
using etrade_core.domain.OrderModule.Enums;
using etrade_core.domain.ProductModule.Enums;

namespace etrade_core.domain.OrderModule.Entities
{
    public class Order : BaseEntity<long>
    {
        public string OrderNumber { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public string CustomerEmail { get; set; } = string.Empty;
        
        // Order'ın içerdiği işlem türleri (Flags enum - birden fazla olabilir)
        public ProductOfferingTypes OfferingTypes { get; set; } = ProductOfferingTypes.None;
        
        // Foreign key for UserProfile relationship (köprü entity)
        public long UserProfileId { get; set; }
        
        // Navigation properties
        public UserProfile UserProfile { get; set; } = null!;
        public ICollection<OrderItem> OrderItems { get; set; } = [];
    }
} 