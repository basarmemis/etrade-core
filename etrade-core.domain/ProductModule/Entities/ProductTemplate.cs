using etrade_core.domain.Entities.Base;
using etrade_core.domain.CategoryModule.Entities;

namespace etrade_core.domain.ProductModule.Entities
{
    public class ProductTemplate : BaseEntity<long>
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public long CategoryId { get; set; }
        public virtual Category Category { get; set; } = null!;
        public bool IsActive { get; set; } = true;
        
        // Template'e özel attribute tanımları
        public virtual ICollection<ProductTemplateAttribute> TemplateAttributes { get; set; } = new List<ProductTemplateAttribute>();
        
        // Bu template'den oluşturulan ürünler
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }
} 