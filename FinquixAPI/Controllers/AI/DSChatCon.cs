using Codeblaze.SemanticKernel.Connectors.Ollama;
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
using System.Text.Json;
using System.Text.RegularExpressions;

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
            Console.WriteLine($"Frontend crypto data: {cryptoMarketDataFromFrontend[0].Symbol}, Price: {cryptoMarketDataFromFrontend[0].CurrentPrice}, Last Updated: {cryptoMarketDataFromFrontend[0].LastUpdated}");

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

                Console.WriteLine($"Frontend crypto data: {cryptoMarketData[0].Symbol}, Price: {cryptoMarketData[0].CurrentPrice}, Last Updated: {cryptoMarketData[0].LastUpdated}");


                var aiPrompt = $@"
                    [ROLE]
                    You are a financial assistant for {userData.UserName} at Finquix.
                    Your expertise includes: investments, savings, budgeting, stocks, crypto, and personal finance.

                    [USER CONTEXT]
                    Income: {userData.Income}
                    Savings: {userData.Savings}
                    Goals: {string.Join(", ", userData.Goals.Select(g => g.GoalType))}

                    [MARKET DATA]
                    Stocks: {string.Join(", ", stockMarketData.Take(3).Select(m => m.Ticker))} 
                    Crypto: {string.Join(", ", cryptoMarketData.Take(3).Select(c => c.Symbol))}

                    [QUESTION]
                    {input.QuestionText}

                    [RESPONSE FORMAT]
                    You must respond with a single valid JSON object in this format:
                    - ""summary"": A direct and clear answer to the user's question. Keep it concise, include cash amounts instead of %. If the user's question is related to money growth, amount of money to invest or amount of money to spend, please include in your response amount of money in cash taking into consideration user's Income = {userData.Income}, Savings = {userData.Savings}, Fixed Expenses = {userData.FixedExpenses} and Variable Expenses = {userData.VariableExpenses}
                    - ""details"": A breakdown of supporting reasoning, organized by section (e.g., Income, Risk Analysis, Market Trends, etc.)

                    ✅ Example:
                    {{
                      ""summary"": ""You can safely invest 10–15% of your monthly income in cryptocurrency, based on your savings and financial goals."",
                      ""details"": [
                        {{
                          ""section"": ""Income and Savings"",
                          ""content"": [""You have {userData.Income} income and {userData.Savings} in savings""]
                        }},
                        {{
                          ""section"": ""Risk Analysis"",
                          ""content"": [""Moderate risk due to savings buffer"", ""Crypto is volatile but manageable""]
                        }},
                        {{
                          ""section"": ""Investment Recommendation"",
                          ""content"": [""Start with 10% allocation, monitor market performance monthly""]
                        }}
                      ]
                    }}

                    If the question is not about finance or is beyond your scope, politely decline while still maintaining valid JSON format.

                    
                    ❌ If the question is related to expected money growth, amount of money to invest or amount of money to spend, please include in your response amount of money in cash taking into consideration Income = {userData.Income}, Savings = {userData.Savings}, Fixed Expenses = {userData.FixedExpenses} and Variable Expenses = {userData.VariableExpenses}
                    ❌ Do not return any markdown.
                    ❌ Do not include any text outside the JSON.
                    ❌ Do not explain your reasoning outside the JSON fields.
                    ";

                var response = await kernel.InvokePromptAsync(aiPrompt);
                //var responseText = response.ToString();
                var responseText = response.ToString().Trim();

                // Auto-extract JSON from the response
                if (responseText.StartsWith("\"{") && responseText.EndsWith("}\""))
                {
                    responseText = JsonSerializer.Deserialize<string>(responseText); // Unwrap
                }

                // Auto-extract JSON block
                var start = responseText.IndexOf('{');
                var end = responseText.LastIndexOf('}');
                if (start >= 0 && end > start)
                {
                    responseText = responseText.Substring(start, end - start + 1);
                }

                responseText = CleanJsonResponse(responseText);

                responseText = FixJsonFormatting(responseText);

                //var parts = responseText.Split(["--- Details ---"], StringSplitOptions.None);

                //var ans = new Answer
                //{
                //    Summary = parts.ElementAtOrDefault(0)?.Trim(),
                //    Details = parts.ElementAtOrDefault(1)?.Trim()
                //};

                //return Ok(ans);
                try
                {
                    var options = new JsonSerializerOptions
                    {
                        AllowTrailingCommas = true, // This handles trailing commas
                        PropertyNameCaseInsensitive = true
                    };

                    // Parse and validate response
                    var structuredResponse = JsonSerializer.Deserialize<StructuredAnswer>(responseText, options);
                    structuredResponse = UnwrapNestedJsonIfNeeded(structuredResponse);

                    if (string.IsNullOrWhiteSpace(structuredResponse?.Summary))
                    {
                        throw new JsonException("Empty summary in response");
                    }

                    //    if (structuredResponse.Details == null || !structuredResponse.Details.Any())
                    //    {
                    //        structuredResponse.Details = new List<AnswerSection> {
                    //    new AnswerSection {
                    //        Section = "Details",
                    //        Content = new List<string> { "No additional details provided" }
                    //    }
                    //};
                    //    }

                    structuredResponse.Details ??= [];

                    Console.WriteLine("Successfully processed AI response");
                    return Ok(structuredResponse);
                }
                catch (JsonException jsonEx)
                {
                    Console.WriteLine($"JSON parse error: {jsonEx.Message}");
                    Console.WriteLine($"Response content: {responseText}");

                    // Create fallback response
                    //    var fallbackResponse = new StructuredAnswer
                    //    {
                    //        Summary = ExtractSummary(responseText),
                    //        Details = new List<AnswerSection> {
                    //    new AnswerSection {
                    //        Section = "Full Response",
                    //        Content = new List<string> { responseText }
                    //    }
                    //}
                    //    };

                    //    return Ok(fallbackResponse);
                    var fixedResponse = TryFixJson(responseText);
                    if (fixedResponse != null)
                    {
                        return Ok(fixedResponse);
                    }

                    return Ok(CreateFallbackResponse(responseText));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing query: {ex}");
                return StatusCode(500, new
                {
                    error = "An error occurred while processing your question",
                    details = ex.Message
                });
            }
        }

        private string FixJsonFormatting(string json)
        {
            // Fix trailing commas in arrays
            json = Regex.Replace(json, @",\s*\]", "]");
            // Fix trailing commas in objects
            json = Regex.Replace(json, @",\s*\}", "}");
            return json;
        }

        private StructuredAnswer TryFixJson(string brokenJson)
        {
            try
            {
                // Try common fixes
                var fixedJson = FixJsonFormatting(brokenJson);
                return JsonSerializer.Deserialize<StructuredAnswer>(fixedJson);
            }
            catch
            {
                return null;
            }
        }

        private StructuredAnswer UnwrapNestedJsonIfNeeded(StructuredAnswer parsed)
        {
            if (parsed.Summary != null && parsed.Summary.StartsWith("{") && parsed.Summary.Contains("\"summary\""))
            {
                try
                {
                    var inner = JsonSerializer.Deserialize<StructuredAnswer>(parsed.Summary);
                    return inner ?? parsed;
                }
                catch
                {
                    // If fail to unwrap, return the original
                    return parsed;
                }
            }

            return parsed;
        }

        private StructuredAnswer CreateFallbackResponse(string responseText)
        {
            //extract JSON portions if possible
            var jsonStart = responseText.IndexOf('{');
            var jsonEnd = responseText.LastIndexOf('}');

            if (jsonStart >= 0 && jsonEnd > jsonStart)
            {
                var possibleJson = responseText.Substring(jsonStart, jsonEnd - jsonStart + 1);
                var fixedResponse = TryFixJson(possibleJson);
                if (fixedResponse != null)
                {
                    return fixedResponse;
                }
            }

            return new StructuredAnswer
            {
                Summary = ExtractSummary(responseText),
                Details =
                [
                    new AnswerSection
                    {
                        Section = "Full Response",
                        Content = [responseText]
                    }
                ]
            };
        }

        private string CleanJsonResponse(string responseText)
        {
            // Remove JSON markdown blocks if present
            if (responseText.StartsWith("```json"))
            {
                responseText = responseText[7..];
            }
            if (responseText.EndsWith("```"))
            {
                responseText = responseText[..^3];
            }
            return responseText.Trim();
        }

        private string ExtractSummary(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return "No response received";

            // find the first sentence
            var firstPeriod = text.IndexOf('.');
            if (firstPeriod > 0 && firstPeriod < 200)
            {
                return text[..(firstPeriod + 1)].Trim();
            }

            return text.Length <= 200 ? text : text[..200] + "...";
        }
    }

    public class StructuredAnswer
    {
        public string Summary { get; set; }
        public List<AnswerSection> Details { get; set; } = [];
    }

    public class AnswerSection
    {
        public string Section { get; set; }
        public List<string> Content { get; set; } = [];
    }
}


//var aiPrompt1 = $@"
//    User: {userData.UserName} has an income of {userData.Income} and savings of {userData.Savings}.
//    Their current financial goals:
//    {string.Join("\n", userData.Goals.Select(g => $"- {g.GoalType}: {g.CurrentProgress}/{g.EstimatedValue} saved"))}

//    📊 **Market Overview:**
//    🔹 **Stock Market Signals:**
//    {string.Join("\n", stockMarketData.Take(3).Select(m => $"- {m.Ticker}: {m.Recommendation} at {m.CurrentPrice}"))}

//    🔹 **Crypto Market Signals:**
//    {string.Join("\n", cryptoMarketData.Take(3).Select(c => $"- {c.Symbol}: Current Price {c.CurrentPrice}, Change {c.ChangePercent}%"))}

//    💬 **User Question:** {input.QuestionText}

//    ✂️ Please reply with:
//    - A **very short summary answer** (max 2 sentences).
//    - Then a **longer explanation** below a line like: '--- Details ---'

//    - All answers provided must be in consideration and matching with the actual data entered by the user profile, so compare against userData.
//    - There should be no missmatch between data and calculations, so please double check the responses before wrapping the answer.
//    - The response should always be dedicated to the question, without providing further advice on other un-related topics.
//    - The response must properly be broken down by bullets or points of focus instead of everything being crunched up in a continuous sentence.
//    - Format the long response into a proper readable text.
//    - There should be a space or new line for text-break the structure of each goal area into separate sections.
//";

//var aiPrompt2 = $@"
//    [ROLE]
//    You are a financial assistant for {userData.UserName} at Finquix.
//    Your expertise covers: investments, savings, stocks, crypto, and personal finance.

//    [USER CONTEXT]
//    Income: {userData.Income}
//    Savings: {userData.Savings}
//    Goals: {string.Join(", ", userData.Goals.Select(g => g.GoalType))}

//    [MARKET DATA]
//    Stocks: {string.Join(", ", stockMarketData.Take(3).Select(m => m.Ticker))}
//    Crypto: {string.Join(", ", cryptoMarketData.Take(3).Select(c => c.Symbol))}

//    [QUESTION]
//    {input.QuestionText}

//    [RULES]
//    1. If question is within financial expertise:
//       - Use all provided context and data
//       - Provide detailed, numerical advice
//       - Double-check calculations match user data
//       - Format as JSON with summary and sections

//    2. If question is outside financial expertise:
//       - You MAY answer simple factual questions (basic definitions)
//       - For complex non-financial questions, politely explain limitations
//       - ALWAYS maintain JSON format

//    3. Response MUST:
//       - Be valid JSON in this exact structure
//       - Contain no text outside the JSON
//       - Have accurate numerical data for financial questions
//       - Include 'summary' and 'details' fields

//    [RESPONSE EXAMPLES]
//    Financial question response:
//    {{
//        ""summary"": ""Recommended 10% crypto allocation"",
//        ""details"": [
//            {{
//                ""section"": ""Risk Analysis"",
//                ""content"": [""Your savings allow moderate risk"", ""Crypto markets are volatile""]
//            }}
//        ]
//    }}

//    [RESPONSE FORMAT]
//    Always return a single JSON object with:
//    - ""summary"": a **direct and clear answer to the user's question** (max 1-2 sentences)
//    - ""details"": breakdown of relevant sections such as income, risk analysis, goals, etc.

//    For example:
//    {{
//      ""summary"": ""You can safely invest 10–15% of your income in crypto given your savings and risk profile."",
//      ""details"": [
//        {{
//          ""section"": ""Income and Savings"",
//          ""content"": [""You have $6000 in income and $8000 in savings""]
//        }},
//        {{
//          ""section"": ""Risk Analysis"",
//          ""content"": [""Moderate risk"", ""Crypto markets are volatile""]
//        }}
//      ]
//    }}

//    Permitted non-financial response:
//    {{
//        ""summary"": ""Pristina is Kosovo's capital"",
//        ""details"": [
//            {{
//                ""section"": ""Geography"",
//                ""content"": [""Kosovo is in Southeastern Europe""]
//            }}
//        ]
//    }}

//    Out-of-scope response:
//    {{
//        ""summary"": ""I specialize in financial topics"",
//        ""details"": [
//            {{
//                ""section"": ""How I can help"",
//                ""content"": [
//                    ""I can analyze your investment portfolio"",
//                    ""Ask me about stocks or crypto markets""
//                ]
//            }}
//        ]
//    }}
//";