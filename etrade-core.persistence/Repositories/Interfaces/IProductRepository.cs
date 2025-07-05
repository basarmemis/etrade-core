using etrade_core.domain.Entities.Core;

namespace etrade_core.persistence.Repositories.Interfaces
{
    public interface IProductRepository : IRepository<Product, long>
    {
        Task<IEnumerable<Product>> GetActiveProductsAsync();
        Task<IEnumerable<Product>> GetProductsByPriceRangeAsync(decimal minPrice, decimal maxPrice);
        Task<Product?> GetBySKUAsync(string sku);
        Task<IEnumerable<Product>> GetLowStockProductsAsync(int threshold = 10);
    }
} 