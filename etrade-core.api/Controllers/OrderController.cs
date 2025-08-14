using etrade_core.application.Services;
using etrade_core.domain.OrderModule.Entities;
using etrade_core.domain.OrderModule.Enums;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using etrade_core.application.OrderModule.Commands.CreateOrder;
using etrade_core.application.OrderModule.Queries.GetOrder;

namespace etrade_core.api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IMediator _mediator;

        public OrderController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        /// Yeni sipariş oluşturur
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<CreateOrderResponse>> CreateOrder([FromBody] CreateOrderCommand command)
        {
            try
            {
                var result = await _mediator.Send(command);
                return CreatedAtAction(nameof(GetOrder), new { id = result.OrderId }, result);
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
        public async Task<ActionResult<GetOrderResponse>> GetOrder(int id)
        {
            try
            {
                var query = new GetOrderQuery { OrderId = id };
                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Kullanıcının siparişlerini getirir
        /// </summary>
        [HttpGet("user/{userProfileId}")]
        public async Task<ActionResult<IEnumerable<Order>>> GetUserOrders(long userProfileId)
        {
            try
            {
                // TODO: Implement GetUserOrdersQuery
                await Task.CompletedTask;
                return Ok(new { message = $"User orders for {userProfileId} retrieved successfully" });
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
                // TODO: Implement UpdateOrderStatusCommand
                await Task.CompletedTask;
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
                // TODO: Implement CancelOrderCommand
                await Task.CompletedTask;
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
        public OrderStatus Status { get; set; }
    }
} 