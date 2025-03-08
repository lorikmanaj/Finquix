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

        /// <summary>
        /// Calls the Ollama API directly.
        /// </summary>
        [HttpPost("kerko1")]
        public async Task<IActionResult> Kerko1([FromBody] string teksti)
        {
            try
            {
                string apiUrl = "http://localhost:11434/api/generate"; // Ollama API

                var requestData = new
                {
                    model = "llama3.2",
                    prompt = teksti
                };

                var jsonRequest = new StringContent(JsonSerializer.Serialize(requestData), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(apiUrl, jsonRequest);
                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync();
                var parsedJson = JObject.Parse(responseBody);
                string answer = parsedJson["response"]?.ToString() ?? "No response";

                return Ok(new { Answer = answer });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Answer = "Gabim", Error = ex.Message });
            }
        }

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
