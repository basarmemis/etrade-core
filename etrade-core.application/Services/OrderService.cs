using etrade_core.application.IRepositories;
using etrade_core.domain.OrderModule.Entities;
using etrade_core.domain.OrderModule.Enums;

namespace etrade_core.application.Services
{
    /// <summary>
    /// Order işlemleri için service - Unit of Work pattern kullanır
    /// </summary>
    public class OrderService
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        /// <summary>
        /// Yeni sipariş oluşturur (transaction ile)
        /// </summary>
        public async Task<Order> CreateOrderAsync(Order order, List<OrderItem> orderItems)
        {
            return await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                // Siparişi kaydet
                await _unitOfWork.Orders.AddAsync(order);
                
                // Sipariş kalemlerini kaydet
                foreach (var item in orderItems)
                {
                    item.OrderId = order.Id;
                    await _unitOfWork.OrderItems.AddAsync(item);
                }

                return order;
            });
        }

        /// <summary>
        /// Sipariş durumunu günceller (transaction ile)
        /// </summary>
        public async Task UpdateOrderStatusAsync(long orderId, OrderStatus newStatus)
        {
            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
                if (order == null)
                {
                    throw new ArgumentException($"Order with ID {orderId} not found.");
                }

                order.Status = newStatus;
                _unitOfWork.Orders.Update(order);
            });
        }

        /// <summary>
        /// Siparişi iptal eder (transaction ile)
        /// </summary>
        public async Task CancelOrderAsync(long orderId)
        {
            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
                if (order == null)
                {
                    throw new ArgumentException($"Order with ID {orderId} not found.");
                }

                // Siparişi iptal et
                order.Status = OrderStatus.Cancelled;
                _unitOfWork.Orders.Update(order);

                // Sipariş kalemlerini de iptal et
                var orderItems = await _unitOfWork.OrderItems.GetAsync(oi => oi.OrderId == orderId);
                foreach (var item in orderItems)
                {
                    _unitOfWork.OrderItems.Delete(item);
                }
            });
        }

        /// <summary>
        /// Kullanıcının siparişlerini getirir
        /// </summary>
        public async Task<IEnumerable<Order>> GetUserOrdersAsync(long userProfileId)
        {
            return await _unitOfWork.Orders.GetOrdersByUserProfileIdAsync(userProfileId);
        }

        /// <summary>
        /// Manuel transaction yönetimi örneği
        /// </summary>
        public async Task<Order> CreateOrderWithManualTransactionAsync(Order order, List<OrderItem> orderItems)
        {
            try
            {
                // Transaction başlat
                await _unitOfWork.BeginTransactionAsync();

                // Siparişi kaydet
                await _unitOfWork.Orders.AddAsync(order);
                
                // Sipariş kalemlerini kaydet
                foreach (var item in orderItems)
                {
                    item.OrderId = order.Id;
                    await _unitOfWork.OrderItems.AddAsync(item);
                }

                // Değişiklikleri kaydet
                await _unitOfWork.SaveChangesAsync();

                // Transaction'ı commit et
                await _unitOfWork.CommitTransactionAsync();

                return order;
            }
            catch
            {
                // Hata durumunda rollback
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }
    }
} 