using FinquixDemo.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace FinquixDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FinancialSignalsController(HttpClient httpClient, IConfiguration configuration, ILogger<FinancialSignalsController> logger) : ControllerBase
    {
        private readonly HttpClient _httpClient = httpClient;
        private readonly ILogger<FinancialSignalsController> _logger = logger;
        private readonly string _fmpApiKey = configuration["FMP_API_KEY"] ?? throw new ArgumentNullException("FMP_API_KEY is not set in configuration");

        [HttpGet]
        public async Task<IActionResult> GetFinancialSignals()
        {
            try
            {
                string fmpUrl = $"https://financialmodelingprep.com/api/v3/stock_market/gainers?apikey={_fmpApiKey}";
                var response = await _httpClient.GetStringAsync(fmpUrl);

                var rawData = JsonSerializer.Deserialize<List<FmpStock>>(response);

                // Select only important fields
                var formattedData = rawData.Select(stock => new
                {
                    stock.symbol,
                    stock.name,
                    stock.price,
                    stock.change,
                    stock.changePercentage
                });

                return Ok(formattedData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching financial signals");
                return StatusCode(500, "Error fetching data");
            }
        }
    }
}
