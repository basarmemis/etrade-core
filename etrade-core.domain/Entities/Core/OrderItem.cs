using etrade_core.domain.Entities.Base;

namespace etrade_core.domain.Entities.Core
{
    public class OrderItem : BaseEntity<long>
    {
        public long OrderId { get; set; }
        public long ProductId { get; set; }
        
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        
        // Navigation properties
        public Order Order { get; set; } = null!;
        public Product Product { get; set; } = null!;
    }
} 