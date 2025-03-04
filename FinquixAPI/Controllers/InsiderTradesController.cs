using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace FinquixAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InsiderTradesController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly string _fmpApiKey;
        private readonly ILogger<InsiderTradesController> _logger;

        public InsiderTradesController(HttpClient httpClient, IConfiguration configuration, ILogger<InsiderTradesController> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _fmpApiKey = configuration.GetValue<string>("FMP_API_KEY");
            if (string.IsNullOrEmpty(_fmpApiKey))
            {
                throw new ArgumentNullException(nameof(_fmpApiKey), "FMP API Key is missing in configuration.");
            }
        }

        /// <summary>
        /// Fetches insider trading activities for a given stock symbol.
        /// </summary>
        [HttpGet("{symbol}")]
        public async Task<IActionResult> GetInsiderTrades(string symbol)
        {
            string url = $"https://financialmodelingprep.com/api/v4/insider-trading?symbol={symbol}&page=0&apikey={_fmpApiKey}";

            try
            {
                //var response = await _httpClient.GetAsync(url);
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Add("User-Agent", "Mozilla/5.0");
                request.Headers.Add("Accept", "application/json");
                var response = await _httpClient.SendAsync(request);


                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"Failed request: {url}, Status Code: {response.StatusCode}");
                    return StatusCode((int)response.StatusCode, $"Error fetching insider trades for {symbol}.");
                }

                var responseData = await response.Content.ReadAsStringAsync();
                return Ok(JsonSerializer.Deserialize<List<object>>(responseData) ?? new List<object>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to fetch insider trades for {symbol}");
                return StatusCode(500, "Error fetching data.");
            }
        }
    }
}
