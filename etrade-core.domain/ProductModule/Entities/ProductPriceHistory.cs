using etrade_core.domain.Entities.Base;

namespace etrade_core.domain.ProductModule.Entities
{
    public class ProductPriceHistory : BaseEntity<long>
    {
        public long ProductId { get; set; }
        public virtual Product Product { get; set; } = null!;
        
        public decimal OldPrice { get; set; }
        public decimal NewPrice { get; set; }
        public string? ChangeReason { get; set; } // "Seasonal", "Promotion", "Cost Increase"
        public DateTime EffectiveDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
} 