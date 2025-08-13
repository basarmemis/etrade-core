using etrade_core.domain.Entities.Base;

namespace etrade_core.domain.OrderModule.Entities
{
    public class OrderItemAttribute : BaseEntity<long>
    {
        public long OrderItemId { get; set; }
        public virtual OrderItem OrderItem { get; set; } = null!;
        
        public string AttributeKey { get; set; } = string.Empty; // Örn: "DepositAmount", "RentalStatus"
        public string AttributeValue { get; set; } = string.Empty; // Örn: "1000", "Active"
        public string AttributeType { get; set; } = string.Empty; // Örn: "Decimal", "Enum", "Text"
        public string? Unit { get; set; } // Örn: "TL", "days"
        public bool IsRequired { get; set; } = false;
        public int DisplayOrder { get; set; } = 0;
    }
} 