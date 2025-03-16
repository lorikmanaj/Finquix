using FinquixAPI.Infrastructure.Services.MarketData;
using FinquixAPI.Models.Assets;
using Microsoft.AspNetCore.Mvc;

namespace FinquixAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CryptoMarketController(IMarketDataSimulatorService marketDataService) : ControllerBase
    {
        private readonly IMarketDataSimulatorService _marketDataService = marketDataService;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CryptoAsset>>> GetCryptoAssets()
        {
            return Ok(await _marketDataService.GetCryptoAssetsAsync());
        }

        [HttpPost("simulate-update")]
        public async Task<IActionResult> SimulateMarketUpdate()
        {
            await _marketDataService.GenerateAndUpdateMarketData();
            return Ok(new { message = "Crypto market data updated successfully." });
        }
    }
}
