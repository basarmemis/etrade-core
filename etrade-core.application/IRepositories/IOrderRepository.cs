using etrade_core.domain.OrderModule.Entities;
using etrade_core.domain.OrderModule.Enums;

namespace etrade_core.application.IRepositories
{
    /// <summary>
    /// Order entity için repository interface
    /// </summary>
    public interface IOrderRepository : IRepository<Order, long>
    {
        /// <summary>
        /// Kullanıcının siparişlerini getirir
        /// </summary>
        Task<IEnumerable<Order>> GetOrdersByUserProfileIdAsync(long userProfileId);
        
        /// <summary>
        /// Belirli tarih aralığındaki siparişleri getirir
        /// </summary>
        Task<IEnumerable<Order>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate);
        
        /// <summary>
        /// Sipariş durumuna göre siparişleri getirir
        /// </summary>
        Task<IEnumerable<Order>> GetOrdersByStatusAsync(OrderStatus status);
    }
} 