﻿using Codeblaze.SemanticKernel.Connectors.Ollama;
using FinquixAPI.Infrastructure.Database;
using FinquixAPI.Infrastructure.Services.FinancialAnalysis;
using FinquixAPI.Infrastructure.Services.MarketData;
using FinquixAPI.Models.AI;
using FinquixAPI.Models.Assets;
using FinquixAPI.Models.DTOs;
using FinquixAPI.Models.Financials;
using FinquixAPI.Models.User;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.SemanticKernel;
using System.Runtime.Intrinsics.X86;
using static System.Net.Mime.MediaTypeNames;

namespace FinquixAPI.Controllers.AI
{
    [Route("api/[controller]")]
    [ApiController]
    public class DSChatCon(FinquixDbContext context,
    IFinancialAnalysisService financialService,
    IMarketDataSimulatorService marketService) : ControllerBase
    {
        private readonly FinquixDbContext _context = context;
        private readonly IFinancialAnalysisService _financialService = financialService;
        private readonly IMarketDataSimulatorService _marketService = marketService;

        [HttpGet("questions")]
        public async Task<ActionResult<IEnumerable<Question>>> GetQuestions()
        {
            return Ok(await _context.Questions.ToListAsync());
        }

        [HttpPost("ask")]
        public async Task<IActionResult> Ask([FromBody] UserQuery input)
        {
            return await ProcessQuery(input, null, null);
        }

        [HttpPost("ask-with-context")]
        public async Task<IActionResult> AskWithContext([FromBody] UserQueryWithContextDto input)
        {
            return await ProcessQuery(input, input.CurrentStockData, input.CurrentCryptoData);
        }

        private async Task<IActionResult> ProcessQuery(
        UserQuery input,
        List<FinancialSignal> stockMarketDataFromFrontend,
        List<CryptoAsset> cryptoMarketDataFromFrontend)
        {
            try
            {
                var kbuilder = Kernel.CreateBuilder()
                    .AddOllamaChatCompletion("llama3.2", "http://localhost:11434");

                kbuilder.Services.AddScoped<HttpClient>();
                var kernel = kbuilder.Build();

                var userData = await _financialService.AnalyzeUserFinances(input.UserId);
                if (userData == null)
                    return NotFound("User data not found");

                var stockMarketData = stockMarketDataFromFrontend ?? await _marketService.GetLatestStockDataAsync();
                var cryptoMarketData = cryptoMarketDataFromFrontend ?? await _marketService.GetLatestCryptoDataAsync();

                var aiPrompt = $@"
                    User: {userData.UserName} has an income of {userData.Income} and savings of {userData.Savings}.
                    Their current financial goals:
                    {string.Join("\n", userData.Goals.Select(g => $"- {g.GoalType}: {g.CurrentProgress}/{g.EstimatedValue} saved"))}

                    📊 **Market Overview:**
                    🔹 **Stock Market Signals:**
                    {string.Join("\n", stockMarketData.Take(3).Select(m => $"- {m.Ticker}: {m.Recommendation} at {m.CurrentPrice}"))}

                    🔹 **Crypto Market Signals:**
                    {string.Join("\n", cryptoMarketData.Take(3).Select(c => $"- {c.Symbol}: Current Price {c.CurrentPrice}, Change {c.ChangePercent}%"))}

                    💬 **User Question:** {input.QuestionText}

                    ✂️ Please reply with:
                    - A **very short summary answer** (max 2 sentences).
                    - Then a **longer explanation** below a line like: '--- Details ---'

                    - All answers provided must be in consideration and matching with the actual data entered by the user profile, so compare against userData.
                    - There should be no missmatch between data and calculations, so please double check the responses before wrapping the answer.
                    - The response should always be dedicated to the question, without providing further advice on other un-related topics.
                    - The response must properly be broken down by bullets or points of focus instead of everything being crunched up in a continuous sentence.
                    - Format the long response into a proper readable text.
                    - There should be a space or new line for text-break the structure of each goal area into separate sections.
                ";

                var response = await kernel.InvokePromptAsync(aiPrompt);
                var responseText = response.ToString();

                var parts = responseText.Split(["--- Details ---"], StringSplitOptions.None);

                var ans = new Answer
                {
                    Summary = parts.ElementAtOrDefault(0)?.Trim(),
                    Details = parts.ElementAtOrDefault(1)?.Trim()
                };

                return Ok(ans);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message, stackTrace = ex.StackTrace });
            }

            //-In the end, i want to see the following outputs from that response:
        }
    }
}

