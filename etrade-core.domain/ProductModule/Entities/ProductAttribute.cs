using etrade_core.domain.Entities.Base;

namespace etrade_core.domain.ProductModule.Entities
{
    public class ProductAttribute : BaseEntity<long>
    {
        public long ProductId { get; set; }
        public virtual Product Product { get; set; } = null!;
        
        public string AttributeKey { get; set; } = string.Empty; // Örn: "MaxGuests", "FrameSize", "Color"
        public string AttributeValue { get; set; } = string.Empty; // Örn: "4", "Large", "Black"
        public string AttributeType { get; set; } = string.Empty; // Örn: "Number", "Text", "Boolean", "Enum"
        public string? Unit { get; set; } // Örn: "persons", "cm", "kg"
        public bool IsRequired { get; set; } = false;
        public int DisplayOrder { get; set; } = 0;
    }
} 