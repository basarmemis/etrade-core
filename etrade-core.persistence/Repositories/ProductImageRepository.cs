using etrade_core.application.IRepositories;
using etrade_core.domain.ProductModule.Entities;
using etrade_core.persistence.Context;
using etrade_core.persistence.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace etrade_core.persistence.Repositories
{
    public class ProductImageRepository : BaseRepository<ProductImage, long, DomainDbContext>, IProductImageRepository
    {
        public ProductImageRepository(DomainDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<ProductImage>> GetByProductIdAsync(long productId)
        {
            return await _context.ProductImages
                .Where(pi => pi.ProductId == productId && !pi.IsDeleted)
                .OrderBy(pi => pi.DisplayOrder)
                .ThenBy(pi => pi.Id)
                .ToListAsync();
        }

        public async Task<ProductImage?> GetPrimaryImageAsync(long productId)
        {
            return await _context.ProductImages
                .FirstOrDefaultAsync(pi => pi.ProductId == productId && 
                                          pi.IsPrimary && 
                                          !pi.IsDeleted);
        }

        public async Task<IEnumerable<ProductImage>> GetByProductIdAndTypeAsync(long productId, string imageType)
        {
            return await _context.ProductImages
                .Where(pi => pi.ProductId == productId && 
                            pi.ImageType == imageType && 
                            !pi.IsDeleted)
                .OrderBy(pi => pi.DisplayOrder)
                .ThenBy(pi => pi.Id)
                .ToListAsync();
        }

        public async Task<bool> HasPrimaryImageAsync(long productId)
        {
            return await _context.ProductImages
                .AnyAsync(pi => pi.ProductId == productId && 
                               pi.IsPrimary && 
                               !pi.IsDeleted);
        }

        public async Task<int> GetImageCountAsync(long productId)
        {
            return await _context.ProductImages
                .CountAsync(pi => pi.ProductId == productId && !pi.IsDeleted);
        }
    }
} 