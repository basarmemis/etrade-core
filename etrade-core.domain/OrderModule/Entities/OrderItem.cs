using etrade_core.domain.Entities.Base;
using etrade_core.domain.ProductModule.Entities;
using etrade_core.domain.OrderModule.Enums;
using etrade_core.domain.ProductModule.Enums;

namespace etrade_core.domain.OrderModule.Entities
{
    public class OrderItem : BaseEntity<long>
    {
        public long OrderId { get; set; }
        public long ProductId { get; set; }
        
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        
        // Bu OrderItem'ın işlem türü
        public ProductOfferingTypes OfferingType { get; set; } = ProductOfferingTypes.None;
        
        // Kiralama için tarih bilgileri (OrderItem seviyesinde gerekli)
        public DateTime? RentalStartDate { get; set; }
        public DateTime? RentalEndDate { get; set; }
        public int? RentalDays { get; set; } // Hesaplanan gün sayısı
        
        // Navigation properties
        public Order Order { get; set; } = null!;
        public Product Product { get; set; } = null!;
        
        // Dinamik özellikler için
        public virtual ICollection<OrderItemAttribute> Attributes { get; set; } = new List<OrderItemAttribute>();
    }
} 