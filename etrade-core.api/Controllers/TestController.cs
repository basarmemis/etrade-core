using Microsoft.AspNetCore.Mvc;
using etrade_core.persistence.Context;
using etrade_core.persistence.data;
using Sample.Messages;

namespace e_trade_core.api.Controllers
{
    /// <summary>
    /// Ana sayfa kontrolcüsü
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class TestController : ControllerBase
    {
        private readonly OrderCreatedMessageSender sender;

        public TestController(OrderCreatedMessageSender sender)
        {
            this.sender = sender;
        }



        [HttpPost("sendandforget")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Index()
        {
            var req = new OrderCreatedMessageRequest(Guid.NewGuid(), 500000, "TRY");
            await sender.PublishToQueueAsync(req);
            return Ok("E-Trade Core API çalışıyor.");
        }
        [HttpPost("sendandwait")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Index2()
        {
            var req = new OrderCreatedMessageRequest(Guid.NewGuid(), 500000, "TRY");
            var resp = await sender.SendAndWaitAsync(req);
            return Ok("E-Trade Core API çalışıyor.");
        }
        [HttpPost("publish")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Index3()
        {
            var req = new OrderCreatedMessageRequest(Guid.NewGuid(), 500000, "TRY");
            await sender.PublishToAllAsync(req);
            return Ok("E-Trade Core API çalışıyor.");
        }

        [HttpPost("schsend")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Index4()
        {
            var req = new OrderCreatedMessageRequest(Guid.NewGuid(), 500000, "TRY");
            await sender.ScheduleSendAsync(req, DateTime.Now.ToUniversalTime().AddSeconds(5));
            return Ok("E-Trade Core API çalışıyor.");
        }
        [HttpPost("schpub")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Index5()
        {
            var req = new OrderCreatedMessageRequest(Guid.NewGuid(), 500000, "TRY");
            await sender.SchedulePublishAsync(req, DateTime.Now.ToUniversalTime().AddSeconds(5));
            return Ok("E-Trade Core API çalışıyor.");
        }
    }
}