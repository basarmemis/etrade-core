using etrade_core.domain.Entities.Base;

namespace etrade_core.domain.Entities.Core
{
    public class Product : BaseEntity<long>
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public string SKU { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }
} 