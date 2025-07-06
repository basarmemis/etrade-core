using etrade_core.domain.Entities.Base;

namespace etrade_core.domain.ProductModule.Entities
{
    public class ProductTemplateAttribute : BaseEntity<long>
    {
        public long ProductTemplateId { get; set; }
        public virtual ProductTemplate ProductTemplate { get; set; } = null!;
        
        public string AttributeKey { get; set; } = string.Empty; // Örn: "MaxGuests", "FrameSize"
        public string AttributeName { get; set; } = string.Empty; // Örn: "Maximum Guests", "Frame Size"
        public string AttributeType { get; set; } = string.Empty; // "Number", "Text", "Boolean", "Enum", "Date"
        public string? DefaultValue { get; set; }
        public string? Unit { get; set; } // Örn: "persons", "cm"
        public bool IsRequired { get; set; } = false;
        public bool IsSearchable { get; set; } = false;
        public bool IsFilterable { get; set; } = false;
        public int DisplayOrder { get; set; } = 0;
        public string? ValidationRules { get; set; } // JSON formatında validation kuralları
        public string? AllowedValues { get; set; } // Enum değerleri için JSON array
    }
} 