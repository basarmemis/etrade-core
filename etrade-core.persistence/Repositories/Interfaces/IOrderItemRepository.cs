using etrade_core.domain.Entities.Core;

namespace etrade_core.persistence.Repositories.Interfaces
{
    public interface IOrderItemRepository : ICompositeKeyRepository<OrderItem>
    {
        Task<IEnumerable<OrderItem>> GetByOrderIdAsync(int orderId);
        Task<IEnumerable<OrderItem>> GetByProductIdAsync(int productId);
        Task<decimal> GetTotalAmountByOrderIdAsync(int orderId);
    }
} 