using etrade_core.application.IRepositories;
using etrade_core.domain.OrderModule.Entities;
using etrade_core.domain.OrderModule.Enums;
using etrade_core.persistence.Context;
using etrade_core.persistence.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace etrade_core.persistence.Repositories
{
    /// <summary>
    /// Order entity için repository implementation
    /// </summary>
    public class OrderRepository : BaseRepository<Order, long, DomainDbContext>, IOrderRepository
    {
        public OrderRepository(DomainDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Kullanıcının siparişlerini getirir
        /// </summary>
        public async Task<IEnumerable<Order>> GetOrdersByUserProfileIdAsync(long userProfileId)
        {
            return await GetQueryable()
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Where(o => o.UserProfileId == userProfileId)
                .OrderByDescending(o => o.CreatedDate)
                .ToListAsync();
        }

        /// <summary>
        /// Belirli tarih aralığındaki siparişleri getirir
        /// </summary>
        public async Task<IEnumerable<Order>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await GetQueryable()
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Include(o => o.UserProfile)
                .Where(o => o.CreatedDate >= startDate && o.CreatedDate <= endDate)
                .OrderByDescending(o => o.CreatedDate)
                .ToListAsync();
        }

        /// <summary>
        /// Sipariş durumuna göre siparişleri getirir
        /// </summary>
        public async Task<IEnumerable<Order>> GetOrdersByStatusAsync(OrderStatus status)
        {
            return await GetQueryable()
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Include(o => o.UserProfile)
                .Where(o => o.Status == status)
                .OrderByDescending(o => o.CreatedDate)
                .ToListAsync();
        }
    }
} 