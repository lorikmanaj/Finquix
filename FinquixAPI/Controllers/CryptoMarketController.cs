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
            //return Ok(await _marketDataService.GetCryptoAssetsAsync());
            //return Ok(await _marketDataService.GetLatestCryptoDataAsync());//Cache, not DB
            var data = await _marketDataService.GetLatestCryptoDataAsync();
            if (data.Count == 0)
            {
                await _marketDataService.GenerateAndUpdateMarketData();
                data = await _marketDataService.GetLatestCryptoDataAsync();
            }
            return Ok(data);
        }

        [HttpPost("simulate-update")]
        public async Task<IActionResult> SimulateMarketUpdate()
        {
            await _marketDataService.GenerateAndUpdateMarketData();
            return Ok(new { message = "Crypto market data updated successfully." });
        }
    }
}
