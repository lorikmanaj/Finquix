using Codeblaze.SemanticKernel.Connectors.Ollama;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel;
using Newtonsoft.Json.Linq;
using static System.Net.WebRequestMethods;

namespace DSchatApp.Controllers
{
    public class DSchatCon : Controller
    {
        private readonly HttpClient Http;// = new HttpClient();

        public DSchatCon(HttpClient http)
        {
            Http = http;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<ViewResult> Kerko1(string Teksti) 
        {
            //ViewBag.Answer = "Loading...";
            var kbuilder = Kernel.CreateBuilder().AddOllamaChatCompletion("deepseek-r1:1.5b", "http://localhost:11434");//llama3.2
            kbuilder.Services.AddScoped<HttpClient>();
            var kernel = kbuilder.Build();
           
            var response = await kernel.InvokePromptAsync(Teksti);
            if ((response.ToString()).Contains("<think>"))
            {
                ViewBag.Answer = (response.ToString()).Substring(17); //responsEdituar;
            }
            else ViewBag.Answer = response;
            
            return View("Index");
        }

        public async Task<ViewResult> Kerko(string Teksti)
        {
            try
            {
                string apiUrl = "http://localhost:4891/v1/chat/completions";

                var requestData = new
                {
                    model ="Llama 3 8B Instruct", //"DeepSeek - R1 - Distill - Qwen - 7B",//"Mini Orca(Small)",//"Mistral Instruct",//
                    max_tokens = 2048,
                    messages = new[]
                    {
                new { role="system", content="You are an AI assistant."},
                new { role="user", content= Teksti}
                    },
                };

                var response = await Http.PostAsJsonAsync(apiUrl, requestData);
                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync();
                var parsedJson = JObject.Parse(responseBody);
                string answer = parsedJson["choices"][0]["messages"]["content"].ToString();
                ViewBag.Answer = answer;
            }
            catch { ViewBag.Answer = "Gabim"; }

            return View("Index");
        }

    }
}
