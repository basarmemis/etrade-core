using etrade_core.application.IRepositories;
using etrade_core.domain.ProductModule.Entities;
using etrade_core.persistence.Context;
using etrade_core.persistence.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace etrade_core.persistence.Repositories
{
    public class ProductTemplateRepository : BaseRepository<ProductTemplate, long, DomainDbContext>, IProductTemplateRepository
    {
        public ProductTemplateRepository(DomainDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<ProductTemplate>> GetByCategoryIdAsync(long categoryId)
        {
            return await _context.ProductTemplates
                .Where(pt => pt.CategoryId == categoryId && pt.IsActive && !pt.IsDeleted)
                .OrderBy(pt => pt.Name)
                .ToListAsync();
        }

        public async Task<ProductTemplate?> GetWithAttributesAsync(long templateId)
        {
            return await _context.ProductTemplates
                .Include(pt => pt.TemplateAttributes)
                .FirstOrDefaultAsync(pt => pt.Id == templateId && !pt.IsDeleted);
        }

        public async Task<IEnumerable<ProductTemplate>> GetActiveTemplatesAsync()
        {
            return await _context.ProductTemplates
                .Where(pt => pt.IsActive && !pt.IsDeleted)
                .OrderBy(pt => pt.Name)
                .ToListAsync();
        }

        public async Task<bool> ExistsByNameAsync(string name)
        {
            return await _context.ProductTemplates
                .AnyAsync(pt => pt.Name == name && !pt.IsDeleted);
        }

        public async Task<IEnumerable<ProductTemplate>> GetTemplatesWithAttributesAsync()
        {
            return await _context.ProductTemplates
                .Include(pt => pt.TemplateAttributes)
                .Where(pt => pt.IsActive && !pt.IsDeleted)
                .OrderBy(pt => pt.Name)
                .ToListAsync();
        }
    }
} 