using etrade_core.domain.Entities.Core;
using etrade_core.persistence.Repositories.Base;
using etrade_core.persistence.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace etrade_core.persistence.Repositories
{
    public class OrderItemRepository : BaseCompositeKeyRepository<OrderItem, DbContext>, IOrderItemRepository
    {
        public OrderItemRepository(DbContext context) : base(context)
        {
        }

        // Implement composite key lookup
        public override async Task<OrderItem?> GetByCompositeKeyAsync(object[] keys, bool includeDeleted = false)
        {
            if (keys.Length != 2 || keys[0] is not int orderId || keys[1] is not int productId)
            {
                throw new ArgumentException("Composite key must contain OrderId and ProductId as integers");
            }

            return await GetQueryable(includeDeleted)
                .FirstOrDefaultAsync(oi => oi.OrderId == orderId && oi.ProductId == productId);
        }

        // Custom methods specific to OrderItem
        public async Task<IEnumerable<OrderItem>> GetByOrderIdAsync(int orderId)
        {
            return await GetAsync(oi => oi.OrderId == orderId);
        }

        public async Task<IEnumerable<OrderItem>> GetByProductIdAsync(int productId)
        {
            return await GetAsync(oi => oi.ProductId == productId);
        }

        public async Task<decimal> GetTotalAmountByOrderIdAsync(int orderId)
        {
            var orderItems = await GetByOrderIdAsync(orderId);
            return orderItems.Sum(oi => oi.TotalPrice);
        }
    }
} 