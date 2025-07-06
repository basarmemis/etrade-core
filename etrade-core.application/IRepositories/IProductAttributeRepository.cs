using etrade_core.domain.ProductModule.Entities;

namespace etrade_core.application.IRepositories
{
    public interface IProductAttributeRepository : IRepository<ProductAttribute, long>
    {
        Task<IEnumerable<ProductAttribute>> GetByProductIdAsync(long productId);
        Task<ProductAttribute?> GetByProductIdAndKeyAsync(long productId, string attributeKey);
        Task<IEnumerable<ProductAttribute>> GetByAttributeKeyAsync(string attributeKey);
        Task<bool> ExistsAsync(long productId, string attributeKey);
    }
} 