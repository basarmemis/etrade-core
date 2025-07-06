using etrade_core.domain.Entities.Base;
using etrade_core.domain.ProductModule.Entities;

namespace etrade_core.domain.CategoryModule.Entities
{
    public class Category : BaseEntity<long>
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsActive { get; set; } = true;
        public int DisplayOrder { get; set; } = 0;
        
        // Hiyerarşik kategori yapısı için
        public long? ParentCategoryId { get; set; }
        public virtual Category? ParentCategory { get; set; }
        public virtual ICollection<Category> SubCategories { get; set; } = new List<Category>();
        
        // Ürünler
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }
}