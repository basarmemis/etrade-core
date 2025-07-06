using etrade_core.domain.CategoryModule.Entities;

namespace etrade_core.application.IRepositories
{
    public interface ICategoryRepository : IRepository<Category, long>
    {
        Task<IEnumerable<Category>> GetRootCategoriesAsync();
        Task<IEnumerable<Category>> GetSubCategoriesAsync(long parentCategoryId);
        Task<IEnumerable<Category>> GetCategoryHierarchyAsync(long categoryId);
        Task<bool> HasSubCategoriesAsync(long categoryId);
        Task<IEnumerable<Category>> GetActiveCategoriesAsync();
    }
} 