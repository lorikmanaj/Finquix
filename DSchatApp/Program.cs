using Codeblaze.SemanticKernel.Connectors.Ollama;
using Microsoft.SemanticKernel;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<HttpClient>();//sp => new HttpClient());

// var kbuilder = Kernel.CreateBuilder().AddOllamaChatCompletion("deepseek-r1", "http://localhost:11434");

////builder.Services.AddScoped<HttpClient>();
//var kernel = kbuilder.Build();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=DSchatCon}/{action=Index}/{id?}");

app.Run();
