using etrade_core.domain.OrderModule.Entities;

namespace etrade_core.application.IRepositories
{
    public interface IOrderItemRepository : ICompositeKeyRepository<OrderItem>
    {
        Task<IEnumerable<OrderItem>> GetByOrderIdAsync(int orderId);
        Task<IEnumerable<OrderItem>> GetByProductIdAsync(int productId);
        Task<decimal> GetTotalAmountByOrderIdAsync(int orderId);
    }
} 