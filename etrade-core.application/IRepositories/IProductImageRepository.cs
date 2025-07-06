using etrade_core.domain.ProductModule.Entities;

namespace etrade_core.application.IRepositories
{
    public interface IProductImageRepository : IRepository<ProductImage, long>
    {
        Task<IEnumerable<ProductImage>> GetByProductIdAsync(long productId);
        Task<ProductImage?> GetPrimaryImageAsync(long productId);
        Task<IEnumerable<ProductImage>> GetByProductIdAndTypeAsync(long productId, string imageType);
        Task<bool> HasPrimaryImageAsync(long productId);
        Task<int> GetImageCountAsync(long productId);
    }
} 