using etrade_core.application.IRepositories;
using etrade_core.domain.ProductModule.Entities;
using etrade_core.persistence.Context;
using etrade_core.persistence.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace etrade_core.persistence.Repositories
{
    public class ProductAttributeRepository : BaseRepository<ProductAttribute, long, DomainDbContext>, IProductAttributeRepository
    {
        public ProductAttributeRepository(DomainDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<ProductAttribute>> GetByProductIdAsync(long productId)
        {
            return await _context.ProductAttributes
                .Where(pa => pa.ProductId == productId && !pa.IsDeleted)
                .OrderBy(pa => pa.DisplayOrder)
                .ThenBy(pa => pa.AttributeKey)
                .ToListAsync();
        }

        public async Task<ProductAttribute?> GetByProductIdAndKeyAsync(long productId, string attributeKey)
        {
            return await _context.ProductAttributes
                .FirstOrDefaultAsync(pa => pa.ProductId == productId && 
                                          pa.AttributeKey == attributeKey && 
                                          !pa.IsDeleted);
        }

        public async Task<IEnumerable<ProductAttribute>> GetByAttributeKeyAsync(string attributeKey)
        {
            return await _context.ProductAttributes
                .Where(pa => pa.AttributeKey == attributeKey && !pa.IsDeleted)
                .ToListAsync();
        }

        public async Task<bool> ExistsAsync(long productId, string attributeKey)
        {
            return await _context.ProductAttributes
                .AnyAsync(pa => pa.ProductId == productId && 
                               pa.AttributeKey == attributeKey && 
                               !pa.IsDeleted);
        }
    }
} 