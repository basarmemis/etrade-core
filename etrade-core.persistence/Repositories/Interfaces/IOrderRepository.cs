using etrade_core.domain.Entities.Core;

namespace etrade_core.persistence.Repositories.Interfaces
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
        Task<IEnumerable<Order>> GetOrdersByStatusAsync(string status);
    }
} 