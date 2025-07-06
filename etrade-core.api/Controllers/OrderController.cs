using etrade_core.application.Services;
using etrade_core.domain.OrderModule.Entities;
using Microsoft.AspNetCore.Mvc;

namespace etrade_core.api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly OrderService _orderService;

        public OrderController(OrderService orderService)
        {
            _orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
        }

        /// <summary>
        /// Yeni sipariş oluşturur
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<Order>> CreateOrder([FromBody] CreateOrderRequest request)
        {
            try
            {
                var order = new Order
                {
                    UserProfileId = request.UserProfileId,
                    Status = "Pending",
                    TotalAmount = request.OrderItems.Sum(item => item.Quantity * item.UnitPrice)
                };

                // Convert OrderItemRequest to OrderItem
                var orderItems = request.OrderItems.Select(item => new OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice
                }).ToList();

                var result = await _orderService.CreateOrderAsync(order, orderItems);
                return CreatedAtAction(nameof(GetOrder), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Siparişi getirir
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(long id)
        {
            // Bu örnek için basit bir implementasyon
            // Gerçek uygulamada OrderService'e GetByIdAsync metodu eklenebilir
            await Task.CompletedTask;
            return Ok(new { message = $"Order {id} retrieved successfully" });
        }

        /// <summary>
        /// Kullanıcının siparişlerini getirir
        /// </summary>
        [HttpGet("user/{userProfileId}")]
        public async Task<ActionResult<IEnumerable<Order>>> GetUserOrders(long userProfileId)
        {
            try
            {
                var orders = await _orderService.GetUserOrdersAsync(userProfileId);
                return Ok(orders);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Sipariş durumunu günceller
        /// </summary>
        [HttpPut("{id}/status")]
        public async Task<ActionResult> UpdateOrderStatus(long id, [FromBody] UpdateOrderStatusRequest request)
        {
            try
            {
                await _orderService.UpdateOrderStatusAsync(id, request.Status);
                return Ok(new { message = $"Order {id} status updated to {request.Status}" });
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Siparişi iptal eder
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> CancelOrder(long id)
        {
            try
            {
                await _orderService.CancelOrderAsync(id);
                return Ok(new { message = $"Order {id} cancelled successfully" });
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }

    // Request DTOs
    public class CreateOrderRequest
    {
        public long UserProfileId { get; set; }
        public List<OrderItemRequest> OrderItems { get; set; } = new();
    }

    public class OrderItemRequest
    {
        public long ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }

    public class UpdateOrderStatusRequest
    {
        public string Status { get; set; } = string.Empty;
    }
} 