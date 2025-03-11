using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text;
using Newtonsoft.Json.Linq;
using Microsoft.SemanticKernel;
using Codeblaze.SemanticKernel.Connectors.Ollama;
using FinquixAPI.Models.AI;

namespace FinquixAPI.Controllers.AI
{
    [Route("api/[controller]")]
    [ApiController]
    public class DSChatCon : ControllerBase
    {
        private readonly HttpClient _httpClient;

        public DSChatCon(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [HttpPost("Kerko1")]
        public async IAsyncEnumerable<Answer> Kerko1([FromBody] string Teksti) //Change Teksti to Input { Id, Text, Action }
        {
            //Question Simulators 
            //Read Goals from userData

            //Step 1 : Inject MarketDataService, Inject UserDataService

            //UserDataService -> var userData { Budget, Income, Goals, Expenses }
            //MarketDataService -> var marketData { CryptoAssets { Title, Price }, FinancialSignals { Title, Price } }

            //Step 2: kbuilderi -> Receives Prompt { Question: { Action = 'Increase Finance' } }

            //Step 2.1: kbuilderi -> Action = Goal, apply Action over data from Step 1.
            //Step 1 outputs var userData edhe marketData, Step 2.1 manipulates data from Step .

            //Step 3: Calculate potential decisions (answer) against FinancialGoals by calculating 

            //Step 4: return response/suggestion to User (do not make decisions)


            var kbuilder = Kernel.CreateBuilder().AddOllamaChatCompletion("llama3.2", "http://localhost:11434");
            //"deepseek-r1"
            //llama3.2 
            kbuilder.Services.AddScoped<HttpClient>();
            var kernel = kbuilder.Build(); 
            Answer ans = new Answer();
            var response = await kernel.InvokePromptAsync(Teksti);

            //Step 1
            //var dummyCryptoData

            //Step 2
            //manipulate with dummyCryptoData using input
            if ((response.ToString()).Contains("<think>"))
                ans.AnswerDS = (response.ToString()).Substring(17); //responsEdituar;
            else
                ans.AnswerDS = response.ToString();

            yield return ans;
        }

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

        /// <summary>
        /// Calls another AI API (OpenAI-style).
        /// </summary>
        [HttpPost("kerko")]
        public async Task<IActionResult> Kerko([FromBody] string teksti)
        {
            try
            {
                string apiUrl = "http://localhost:4891/v1/chat/completions"; // OpenAI-style API

                var requestData = new
                {
                    model = "Llama 3 8B Instruct",
                    max_tokens = 2048,
                    messages = new[]
                    {
                        new { role = "system", content = "You are an AI assistant." },
                        new { role = "user", content = teksti }
                    }
                };

                var jsonRequest = new StringContent(JsonSerializer.Serialize(requestData), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(apiUrl, jsonRequest);
                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync();
                var parsedJson = JObject.Parse(responseBody);
                string answer = parsedJson["choices"]?[0]?["messages"]?["content"]?.ToString() ?? "No response";

                return Ok(new { Answer = answer });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Answer = "Gabim", Error = ex.Message });
            }
        }
    }
}
