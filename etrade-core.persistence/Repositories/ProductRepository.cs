using etrade_core.application.IRepositories;
using etrade_core.domain.ProductModule.Entities;
using etrade_core.persistence.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace etrade_core.persistence.Repositories
{
    public class ProductRepository : BaseRepository<Product, long, DbContext>, IProductRepository
    {
        public ProductRepository(DbContext context) : base(context)
        {
        }

        // Custom methods specific to Product
        public async Task<IEnumerable<Product>> GetActiveProductsAsync()
        {
            return await GetAsync(p => p.IsActive);
        }

        public async Task<IEnumerable<Product>> GetProductsByPriceRangeAsync(decimal minPrice, decimal maxPrice)
        {
            return await GetAsync(p => p.Price >= minPrice && p.Price <= maxPrice);
        }

        public async Task<Product?> GetBySKUAsync(string sku)
        {
            return await GetFirstAsync(p => p.SKU == sku);
        }

        public async Task<IEnumerable<Product>> GetLowStockProductsAsync(int threshold = 10)
        {
            return await GetAsync(p => p.StockQuantity <= threshold);
        }
    }
}