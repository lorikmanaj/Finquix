using FinquixAPI.Infrastructure.Services.MarketData;
using FinquixAPI.Models.Assets;
using Microsoft.AspNetCore.Mvc;

namespace FinquixAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StockMarketController(IMarketDataSimulatorService marketDataService) : ControllerBase
    {
        private readonly IMarketDataSimulatorService _marketDataService = marketDataService;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<StockAsset>>> GetStockAssets()
        {
            return Ok(await _marketDataService.GetFinancialSignalsAsync());
        }

        [HttpPost("simulate-update")]
        public async Task<IActionResult> SimulateMarketUpdate()
        {
            await _marketDataService.GenerateAndUpdateMarketData();
            var updatedStocks = await _marketDataService.GetFinancialSignalsAsync();
            return Ok(updatedStocks);
        }
    }
}
