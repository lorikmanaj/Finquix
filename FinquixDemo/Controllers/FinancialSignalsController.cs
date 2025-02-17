using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace FinquixDemo.Controllers
{
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
