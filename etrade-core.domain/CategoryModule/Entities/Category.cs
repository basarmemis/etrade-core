using etrade_core.domain.ProductModule.Entities;

namespace etrade_core.domain.CategoryModule.Entities
{
    public class Category
    {
        public virtual ICollection<Product> Products { get; set; } = [];
    }
}