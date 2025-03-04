using FinquixAPI.Infrastructure.Database;
using FinquixAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinquixAPI.Controllers
{
    [Route("api/[controller]")] 
    [ApiController]
    public class StockMarketController(FinquixDbContext context) : ControllerBase
    {
        private readonly FinquixDbContext _context = context;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<StockAsset>>> GetStockAssets()
        {
            return await _context.StockAssets.ToListAsync();
        }

        [HttpPost("simulate-update")]
        public async Task<IActionResult> SimulateMarketUpdate()
        {
            var stocks = await _context.StockAssets.ToListAsync();
            var random = new Random();

            foreach (var stock in stocks)
            {
                stock.CurrentPrice += (decimal)(random.NextDouble() * 10 - 5); // -5 to +5
                stock.ChangePercent = (decimal)(random.NextDouble() * 5 - 2.5); // -2.5% to +2.5%
                stock.LastUpdated = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            return Ok(stocks);
        }
    }
}
