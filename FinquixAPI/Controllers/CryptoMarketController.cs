using FinquixAPI.Infrastructure.Database;
using FinquixAPI.Models.Assets;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinquixAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CryptoMarketController : ControllerBase
    {
        private readonly FinquixDbContext _context;

        public CryptoMarketController(FinquixDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CryptoAsset>>> GetCryptoAssets()
        {
            return await _context.CryptoAssets.ToListAsync();
        }

        [HttpPost("simulate-update")]
        public async Task<IActionResult> SimulateMarketUpdate()
        {
            var cryptos = await _context.CryptoAssets.ToListAsync();
            var random = new Random();

            foreach (var crypto in cryptos)
            {
                crypto.CurrentPrice += (decimal)(random.NextDouble() * 100 - 50); // Simulate -50 to +50 change
                crypto.ChangePercent = (decimal)(random.NextDouble() * 10 - 5);  // Simulate -5% to +5% change
                crypto.LastUpdated = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            return Ok(cryptos);
        }
    }
}
