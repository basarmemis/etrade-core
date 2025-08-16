using Microsoft.AspNetCore.Mvc;
using etrade_core.infrastructure.CustomMessageQueue.IServices;
using etrade_core.infrastructure.Sample.Messages;

namespace e_trade_core.api.Controllers
{
    /// <summary>
    /// Ana sayfa kontrolcüsü
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class TestController : ControllerBase
    {
        private readonly IMessageSenderService sender;

        public TestController(IMessageSenderService sender)
        {
            this.sender = sender;
        }



        [HttpPost("SendAndForgetAsync")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Index0()
        {
            var req = new OrderCreatedMessageRequest
            {
                OrderId = Guid.NewGuid(),
                TotalAmount = 500000,
                Currency = "TRY"
            };
            await sender.SendAndForgetAsync(req);
            return Ok("E-Trade Core API çalışıyor.");
        }
        [HttpPost("PublishToQueueAsync")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Index1()
        {
            var req = new OrderCreatedMessageRequest
            {
                OrderId = Guid.NewGuid(),
                TotalAmount = 500000,
                Currency = "TRY"
            };
            await sender.PublishToQueueAsync(req);
            return Ok("E-Trade Core API çalışıyor.");
        }
        [HttpPost("SendAndWaitAsync")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Index2()
        {
            var req = new OrderCreatedMessageRequest
            {
                OrderId = Guid.NewGuid(),
                TotalAmount = 500000,
                Currency = "TRY"
            };
            var resp = await sender.SendAndWaitAsync<OrderCreatedMessageRequest, OrderCreatedMessageResponse>(req);
            return Ok("E-Trade Core API çalışıyor.");
        }
        [HttpPost("PublishToAllAsync")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Index3()
        {
            var req = new OrderCreatedMessageRequest
            {
                OrderId = Guid.NewGuid(),
                TotalAmount = 500000,
                Currency = "TRY"
            };
            await sender.PublishToAllAsync(req);
            return Ok("E-Trade Core API çalışıyor.");
        }

        [HttpPost("ScheduleSendAsync")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Index4()
        {
            var req = new OrderCreatedMessageRequest
            {
                OrderId = Guid.NewGuid(),
                TotalAmount = 500000,
                Currency = "TRY"
            };
            await sender.ScheduleSendAsync(req, DateTime.Now.ToUniversalTime().AddSeconds(5));
            return Ok("E-Trade Core API çalışıyor.");
        }
        [HttpPost("SchedulePublishAsync")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Index5()
        {
            var req = new OrderCreatedMessageRequest
            {
                OrderId = Guid.NewGuid(),
                TotalAmount = 500000,
                Currency = "TRY"
            };
            await sender.SchedulePublishAsync(req, DateTime.Now.ToUniversalTime().AddSeconds(5));
            return Ok("E-Trade Core API çalışıyor.");
        }
    }
}