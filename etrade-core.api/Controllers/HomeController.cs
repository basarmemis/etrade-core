using Microsoft.AspNetCore.Mvc;

namespace e_trade_core.api.Controllers
{
    /// <summary>
    /// Ana sayfa kontrolcüsü
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class HomeController : ControllerBase
    {
        /// <summary>
        /// API'nin çalışıp çalışmadığını kontrol eder
        /// </summary>
        /// <returns>API durumu mesajı</returns>
        /// <response code="200">API başarıyla çalışıyor</response>
        [HttpGet("")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult Index()
        {
            return Ok("E-Trade Core API çalışıyor.");
        }
    }
}