using Microsoft.AspNetCore.Mvc;
using etrade_core.persistence.Context;
using etrade_core.persistence.data;

namespace e_trade_core.api.Controllers
{
    /// <summary>
    /// Ana sayfa kontrolcüsü
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class HomeController : ControllerBase
    {
        private readonly DomainDbContext _context;

        public HomeController(DomainDbContext context)
        {
            _context = context;
        }

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

        /// <summary>
        /// Test verilerini veritabanına ekler
        /// </summary>
        /// <returns>Seed işlemi sonucu</returns>
        /// <response code="200">Seed işlemi başarılı</response>
        [HttpPost("seed")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> SeedData1()
        {
            try
            {
                await SeedData.SeedAsync(_context);
                return Ok("Test verileri başarıyla eklendi.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Seed işlemi başarısız: {ex.Message}");
            }
        }
    }
}