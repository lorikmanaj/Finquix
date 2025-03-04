using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text;
using Newtonsoft.Json.Linq;

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

        [HttpPost("kerko1")]
        public async Task<IActionResult> Kerko1([FromBody] string teksti)
        {
            try
            {
                var kbuilder = Kernel.CreateBuilder().AddOllamaChatCompletion("llama3.2", "http://localhost:11434");
                kbuilder.Services.AddScoped<HttpClient>();
                var kernel = kbuilder.Build();

                var response = await kernel.InvokePromptAsync(teksti);
                string answer = response.ToString().Contains("<think>")
                    ? response.ToString().Substring(17)
                    : response.ToString();

                return Ok(new { Answer = answer });
            }
            catch
            {
                return BadRequest(new { Answer = "Gabim" });
            }
        }

        [HttpPost("kerko")]
        public async Task<IActionResult> Kerko([FromBody] string teksti)
        {
            try
            {
                string apiUrl = "http://localhost:4891/v1/chat/completions";

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
            catch
            {
                return BadRequest(new { Answer = "Gabim" });
            }
        }
    }
}
