using etrade_core.domain.Entities.Base;
using etrade_core.domain.ProductModule.Enums;
using etrade_core.domain.CategoryModule.Entities;

namespace etrade_core.domain.ProductModule.Entities
{
    public class Product : BaseEntity<long>
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public string SKU { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public ProductOfferingTypes OfferingType { get; set; } = ProductOfferingTypes.None;
        
        // Generic özellikler için
        public string Brand { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public int? Year { get; set; }
        public string Condition { get; set; } = string.Empty; // New, Used, Refurbished
        
        // Kategori ilişkisi
        public long CategoryId { get; set; }
        public virtual Category Category { get; set; } = null!;
        
        // Template ilişkisi (opsiyonel)
        public long? ProductTemplateId { get; set; }
        public virtual ProductTemplate? ProductTemplate { get; set; }
        
        // Dinamik özellikler için
        public virtual ICollection<ProductAttribute> Attributes { get; set; } = new List<ProductAttribute>();
        
        // Medya dosyaları için
        public virtual ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
        
        // Fiyat geçmişi için
        public virtual ICollection<ProductPriceHistory> PriceHistory { get; set; } = new List<ProductPriceHistory>();
    }
}