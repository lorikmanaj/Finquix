using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace FinquixDemo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CryptoController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly string _fmpApiKey;
        private readonly ILogger<CryptoController> _logger;

        public CryptoController(HttpClient httpClient, IConfiguration configuration, ILogger<CryptoController> logger)
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
        /// Fetches real-time crypto quotes from Financial Modeling Prep API.
        /// </summary>
        [HttpGet("quotes")]
        public async Task<IActionResult> GetCryptoQuotes()
        {
            string url = $"https://financialmodelingprep.com/api/v3/quotes/crypto?apikey={_fmpApiKey}";

            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                var response = await _httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"Failed request: {url}, Status Code: {response.StatusCode}");
                    return StatusCode((int)response.StatusCode, "Error fetching crypto quotes.");
                }

                var responseData = await response.Content.ReadAsStringAsync();
                return Ok(JsonSerializer.Deserialize<List<object>>(responseData) ?? new List<object>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching {url}. Exception: {message}", url, ex.Message);
                return StatusCode(500, "Error fetching data.");
            }
        }

        /// <summary>
        /// Fetches historical price data for a specific cryptocurrency.
        /// </summary>
        [HttpGet("history/{symbol}")]
        public async Task<IActionResult> GetCryptoHistory(string symbol)
        {
            string url = $"https://financialmodelingprep.com/api/v3/historical-price-full/{symbol}?apikey={_fmpApiKey}";

            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                var response = await _httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"Failed request: {url}, Status Code: {response.StatusCode}");
                    return StatusCode((int)response.StatusCode, $"Error fetching historical data for {symbol}.");
                }

                var responseData = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<Dictionary<string, List<object>>>(responseData);

                return Ok(data?["historical"] ?? new List<object>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching {url}. Exception: {message}", url, ex.Message);
                return StatusCode(500, "Error fetching data.");
            }
        }
    }
}
