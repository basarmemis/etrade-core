using etrade_core.domain.ProductModule.Entities;

namespace etrade_core.application.IRepositories
{
    public interface IProductTemplateRepository : IRepository<ProductTemplate, long>
    {
        Task<IEnumerable<ProductTemplate>> GetByCategoryIdAsync(long categoryId);
        Task<ProductTemplate?> GetWithAttributesAsync(long templateId);
        Task<IEnumerable<ProductTemplate>> GetActiveTemplatesAsync();
        Task<bool> ExistsByNameAsync(string name);
        Task<IEnumerable<ProductTemplate>> GetTemplatesWithAttributesAsync();
    }
} 