using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace FinquixDemo.Controllers
{
    // Model for deserialization
    public class FmpStock
    {
        public string symbol { get; set; }
        public string name { get; set; }
        public decimal price { get; set; }
        public decimal change { get; set; }
        public decimal changePercentage { get; set; }
    }

    [ApiController]
    [Route("api/signals")]
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

        [HttpGet]
        public async Task<IActionResult> GetFinancialSignalsOLD()
        {
            try
            {
                string fmpUrl = $"https://financialmodelingprep.com/api/v3/stock_market/gainers?apikey={_fmpApiKey}";
                var response = await _httpClient.GetStringAsync(fmpUrl);

                var data = JsonSerializer.Deserialize<object>(response);
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching financial signals");
                return StatusCode(500, "Error fetching data");
            }
        }
    }
}
