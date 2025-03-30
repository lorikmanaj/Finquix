using Codeblaze.SemanticKernel.Connectors.Ollama;
using FinquixAPI.Infrastructure.Database;
using FinquixAPI.Infrastructure.Services.FinancialAnalysis;
using FinquixAPI.Infrastructure.Services.MarketData;
using FinquixAPI.Models.AI;
using FinquixAPI.Models.User;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.SemanticKernel;

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
            var kbuilder = Kernel.CreateBuilder()
                .AddOllamaChatCompletion("llama3.2", "http://localhost:11434");

            kbuilder.Services.AddScoped<HttpClient>();
            var kernel = kbuilder.Build();

            var userData = await _financialService.AnalyzeUserFinances(input.UserId);
            if (userData == null)
                return NotFound("User data not found");
            
            var stockMarketData = await _marketService.GetLatestStockDataAsync();
            var cryptoMarketData = await _marketService.GetLatestCryptoDataAsync();

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
                1. A **very short summary answer** (max 2 sentences).
                2. Then a **longer explanation** below a line like: '--- Details ---'
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

        //[HttpPost("ask")]
        //public async IAsyncEnumerable<Answer> Ask([FromBody] Question input)
        //{
        //    var kbuilder = Kernel.CreateBuilder()
        //        .AddOllamaChatCompletion("llama3.2", "http://localhost:11434");

        //    kbuilder.Services.AddScoped<HttpClient>();
        //    var kernel = kbuilder.Build();

        //    //Fetch user financial data
        //    var userData = await _financialService.AnalyzeUserFinances(input.UserId);

        //    var stockMarketData = await _marketService.GetFinancialSignalsAsync();
        //    var cryptoMarketData = await _marketService.GetCryptoAssetsAsync();

        //    // Construct AI prompt
        //    var aiPrompt = $@"
        //        User: {userData.UserName} has an income of {userData.Income} and savings of {userData.Savings}.
        //        Their current financial goals:
        //        {string.Join("\n", userData.Goals.Select(g => $"- {g.GoalType}: {g.CurrentProgress}/{g.EstimatedValue} saved"))}

        //        📊 **Market Overview:**
        //        🔹 **Stock Market Signals:**
        //        {string.Join("\n", stockMarketData.Take(3).Select(m => $"- {m.Ticker}: {m.Recommendation} at {m.CurrentPrice}"))}

        //        🔹 **Crypto Market Signals:**
        //        {string.Join("\n", cryptoMarketData.Take(3).Select(c => $"- {c.Symbol}: Current Price {c.CurrentPrice}, Change {c.ChangePercent}%"))}

        //        💬 **User Question:** {input.Text}
        //    ";

        //    var response = await kernel.InvokePromptAsync(aiPrompt);
        //    Answer ans = new();

        //    if ((response.ToString()).Contains("<think>"))
        //        ans.AnswerDS = (response.ToString()).Substring(17);
        //    else
        //        ans.AnswerDS = response.ToString();

        //    yield return ans;
        //}

        // 6️⃣ AI Analysis Endpoint
        //[HttpPost("analyze")]
        //public async Task<IActionResult> Analyze([FromBody] UserQuery input)
        //{
        //    var userData = await _financialService.AnalyzeUserFinances(input.UserId);
        //    var marketData = await _marketService.GetFinancialSignalsAsync();

        //    var aiPrompt = $@"
        //        User: {userData.UserName} has an income of {userData.Income} and savings of {userData.Savings}.
        //        Their current financial goals:
        //        {string.Join("\n", userData.Goals.Select(g => $"- {g.GoalType}: {g.CurrentProgress}/{g.EstimatedValue} saved"))}

        //        Market signals:
        //        {string.Join("\n", marketData.Take(3).Select(m => $"- {m.Ticker}: {m.Recommendation} at {m.CurrentPrice}"))}

        //        {input.Text}
        //    ";

        //    var ollamaResponse = await SendToOllama(aiPrompt);
        //    return Ok(new { Answer = ollamaResponse });
        //}

        //private async Task<string> SendToOllama(string prompt)
        //{
        //    var requestData = new
        //    {
        //        model = "llama3.2",
        //        prompt = prompt
        //    };

        //    var jsonRequest = new StringContent(JsonSerializer.Serialize(requestData), Encoding.UTF8, "application/json");
        //    var response = await _httpClient.PostAsync("http://localhost:11434/api/generate", jsonRequest);
        //    response.EnsureSuccessStatusCode();

        //    var responseBody = await response.Content.ReadAsStringAsync();
        //    var parsedJson = JObject.Parse(responseBody);
        //    return parsedJson["response"]?.ToString() ?? "No response";
        //}
    }
}
//Original
//[HttpPost("Kerko1")]
//public async IAsyncEnumerable<Answer> Kerko1([FromBody] string Teksti) //Change Teksti to Input { Id, Text, Action }
//{
//    //Question Simulators 
//    //Read Goals from userData

//    //Step 1 : Inject MarketDataService, Inject UserDataService

//    //UserDataService -> var userData { Budget, Income, Goals, Expenses }
//    //MarketDataService -> var marketData { CryptoAssets { Title, Price }, FinancialSignals { Title, Price } }

//    //Step 2: kbuilderi -> Receives Prompt { Question: { Action = 'Increase Finance' } }

//    //Step 2.1: kbuilderi -> Action = Goal, apply Action over data from Step 1.
//    //Step 1 outputs var userData edhe marketData, Step 2.1 manipulates data from Step .

//    //Step 3: Calculate potential decisions (answer) against FinancialGoals by calculating 

//    //Step 4: return response/suggestion to User (do not make decisions)


//    var kbuilder = Kernel.CreateBuilder().AddOllamaChatCompletion("llama3.2", "http://localhost:11434");
//    //"deepseek-r1"
//    //llama3.2 
//    kbuilder.Services.AddScoped<HttpClient>();
//    var kernel = kbuilder.Build(); 
//    Answer ans = new Answer();
//    var response = await kernel.InvokePromptAsync(Teksti);

//    //var dummyCryptoData and otherfinancial data

//    //manipulate with dummyCryptoData and otherfinancial data using input
//    if ((response.ToString()).Contains("<think>"))
//        ans.AnswerDS = (response.ToString()).Substring(17); //responsEdituar;
//    else
//        ans.AnswerDS = response.ToString();

//    yield return ans;
//}

/// <summary>
/// Calls the Ollama API directly.
/// </summary>
//[HttpPost("kerko1")]
//public async Task<IActionResult> Kerko1([FromBody] string teksti)
//{
//    try
//    {
//        string apiUrl = "http://localhost:11434/api/generate"; // Ollama API
//        var kbuilder = Kernel.CreateBuilder().AddOllamaChatCompletion("deepseek-r1:1.5b", "http://localhost:11434");

//        var requestData = new
//        {
//            model = "llama3.2",
//            prompt = teksti
//        };

//        var jsonRequest = new StringContent(JsonSerializer.Serialize(requestData), Encoding.UTF8, "application/json");
//        var response = await _httpClient.PostAsync(apiUrl, jsonRequest);
//        response.EnsureSuccessStatusCode();

//        var responseBody = await response.Content.ReadAsStringAsync();
//        var parsedJson = JObject.Parse(responseBody);
//        string answer = parsedJson["response"]?.ToString() ?? "No response";

//        return Ok(new { Answer = answer });
//    }
//    catch (Exception ex)
//    {
//        return BadRequest(new { Answer = "Gabim", Error = ex.Message });
//    }
//}

///// <summary>
///// Calls another AI API (OpenAI-style).
///// </summary>
//[HttpPost("kerko")]
//public async Task<IActionResult> Kerko([FromBody] string teksti)
//{
//    try
//    {
//        string apiUrl = "http://localhost:4891/v1/chat/completions"; // OpenAI-style API

//        var requestData = new
//        {
//            model = "Llama 3 8B Instruct",
//            max_tokens = 2048,
//            messages = new[]
//            {
//                new { role = "system", content = "You are an AI assistant." },
//                new { role = "user", content = teksti }
//            }
//        };

//        var jsonRequest = new StringContent(JsonSerializer.Serialize(requestData), Encoding.UTF8, "application/json");
//        var response = await _httpClient.PostAsync(apiUrl, jsonRequest);
//        response.EnsureSuccessStatusCode();

//        var responseBody = await response.Content.ReadAsStringAsync();
//        var parsedJson = JObject.Parse(responseBody);
//        string answer = parsedJson["choices"]?[0]?["messages"]?["content"]?.ToString() ?? "No response";

//        return Ok(new { Answer = answer });
//    }
//    catch (Exception ex)
//    {
//        return BadRequest(new { Answer = "Gabim", Error = ex.Message });
//    }
//}

