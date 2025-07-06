using etrade_core.domain.Entities.Base;

namespace etrade_core.domain.ProductModule.Entities
{
    public class ProductImage : BaseEntity<long>
    {
        public long ProductId { get; set; }
        public virtual Product Product { get; set; } = null!;
        
        public string ImageUrl { get; set; } = string.Empty;
        public string? AltText { get; set; }
        public string? Title { get; set; }
        public bool IsPrimary { get; set; } = false;
        public int DisplayOrder { get; set; } = 0;
        public string? ImageType { get; set; } // "Thumbnail", "Gallery", "Detail"
    }
} 