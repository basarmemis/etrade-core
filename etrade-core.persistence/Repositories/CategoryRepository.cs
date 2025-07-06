using etrade_core.application.IRepositories;
using etrade_core.domain.CategoryModule.Entities;
using etrade_core.persistence.Context;
using etrade_core.persistence.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace etrade_core.persistence.Repositories
{
    public class CategoryRepository : BaseRepository<Category, long, DomainDbContext>, ICategoryRepository
    {
        public CategoryRepository(DomainDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Category>> GetRootCategoriesAsync()
        {
            return await _context.Categories
                .Where(c => c.ParentCategoryId == null && c.IsActive && !c.IsDeleted)
                .OrderBy(c => c.DisplayOrder)
                .ThenBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Category>> GetSubCategoriesAsync(long parentCategoryId)
        {
            return await _context.Categories
                .Where(c => c.ParentCategoryId == parentCategoryId && c.IsActive && !c.IsDeleted)
                .OrderBy(c => c.DisplayOrder)
                .ThenBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Category>> GetCategoryHierarchyAsync(long categoryId)
        {
            var category = await _context.Categories
                .Include(c => c.SubCategories)
                .FirstOrDefaultAsync(c => c.Id == categoryId && !c.IsDeleted);

            if (category == null)
                return Enumerable.Empty<Category>();

            var hierarchy = new List<Category> { category };
            
            // Recursively add subcategories
            foreach (var subCategory in category.SubCategories.Where(sc => sc.IsActive && !sc.IsDeleted))
            {
                var subHierarchy = await GetCategoryHierarchyAsync(subCategory.Id);
                hierarchy.AddRange(subHierarchy);
            }

            return hierarchy;
        }

        public async Task<bool> HasSubCategoriesAsync(long categoryId)
        {
            return await _context.Categories
                .AnyAsync(c => c.ParentCategoryId == categoryId && c.IsActive && !c.IsDeleted);
        }

        public async Task<IEnumerable<Category>> GetActiveCategoriesAsync()
        {
            return await _context.Categories
                .Where(c => c.IsActive && !c.IsDeleted)
                .OrderBy(c => c.DisplayOrder)
                .ThenBy(c => c.Name)
                .ToListAsync();
        }
    }
} 