using FinquixAPI.Infrastructure.Database;
using FinquixAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinquixAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FinancialSignals(FinquixDbContext context) : ControllerBase
    {
        private readonly FinquixDbContext _context = context;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<FinancialSignal>>> GetSignals()
        {
            return await _context.FinancialSignals.ToListAsync();
        }

        [HttpPost("generate-signal")]
        public async Task<IActionResult> GenerateMockSignal()
        {
            var random = new Random();
            var newSignal = new FinancialSignal
            {
                Ticker = "AAPL",
                CompanyName = "Apple Inc.",
                CurrentPrice = random.Next(100, 200),
                ChangePercent = (decimal)(random.NextDouble() * 10 - 5), // Simulate -5% to +5%
                SignalDate = DateTime.UtcNow,
                Recommendation = random.Next(0, 3) switch
                {
                    0 => "Buy",
                    1 => "Hold",
                    _ => "Sell"
                }
            };

            _context.FinancialSignals.Add(newSignal);
            await _context.SaveChangesAsync();

            return Ok(newSignal);
        }

        //FinancialSignalsController(HttpClient httpClient, IConfiguration configuration, ILogger<FinancialSignalsController> logger)

        //private readonly HttpClient _httpClient = httpClient;
        //private readonly ILogger<FinancialSignalsController> _logger = logger;
        //private readonly string _fmpApiKey = configuration["FMP_API_KEY"] ?? throw new ArgumentNullException("FMP_API_KEY is not set in configuration");

        //[HttpGet]
        //public async Task<IActionResult> GetFinancialSignals()
        //{
        //    try
        //    {
        //        string fmpUrl = $"https://financialmodelingprep.com/api/v3/stock_market/gainers?apikey={_fmpApiKey}";
        //        var response = await _httpClient.GetStringAsync(fmpUrl);

        //        var rawData = JsonSerializer.Deserialize<List<FmpStock>>(response);

        //        // Select only important fields
        //        var formattedData = rawData.Select(stock => new
        //        {
        //            stock.symbol,
        //            stock.name,
        //            stock.price,
        //            stock.change,
        //            stock.changePercentage
        //        });

        //        return Ok(formattedData);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error fetching financial signals");
        //        return StatusCode(500, "Error fetching data");
        //    }
        //}
    }
}
