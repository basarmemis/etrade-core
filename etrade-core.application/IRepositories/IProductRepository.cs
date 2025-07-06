

using etrade_core.domain.ProductModule.Entities;

namespace etrade_core.application.IRepositories
{
    public interface IProductRepository : IRepository<Product, long>
    {
        Task<IEnumerable<Product>> GetActiveProductsAsync();
        Task<IEnumerable<Product>> GetProductsByPriceRangeAsync(decimal minPrice, decimal maxPrice);
        Task<Product?> GetBySKUAsync(string sku);
        Task<IEnumerable<Product>> GetLowStockProductsAsync(int threshold = 10);
    }
} 