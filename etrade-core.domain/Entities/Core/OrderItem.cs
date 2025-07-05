using System.ComponentModel.DataAnnotations;
using etrade_core.domain.Entities.Base;

namespace etrade_core.domain.Entities.Core
{
    public class OrderItem : BaseEntityWithCompositeKey
    {
        [Key]
        public int OrderId { get; set; }
        
        [Key]
        public int ProductId { get; set; }
        
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        
        // Navigation properties
        public Order Order { get; set; } = null!;
        public Product Product { get; set; } = null!;
    }
} 