using AA_Aggregation_API_AT.Clients.NewsApi;
using AA_Aggregation_API_AT.Clients.NewsApi.Options;
using AA_Aggregation_API_AT.Clients.OpenWeather;
using AA_Aggregation_API_AT.Clients.OpenWeather.Options;
using AA_Aggregation_API_AT.Clients.Rawg;
using AA_Aggregation_API_AT.Clients.Rawg.Options;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();


// Set up Weather Client
builder.Services.Configure<OpenWeatherOptions>(builder.Configuration.GetSection("Clients:OpenWeather"));
builder.Services.AddHttpClient<OpenWeatherClient>((sp, client) =>
{
    var options = sp.GetRequiredService<IOptions<OpenWeatherOptions>>().Value;
    client.BaseAddress = new Uri(options.BaseUrl);
});


// Set up News Api Client
builder.Services.Configure<NewsApiOptions>(builder.Configuration.GetSection("Clients:NewsApi"));
builder.Services.AddHttpClient<NewsApiClient>((sp, client) =>
{
    var options = sp.GetRequiredService<IOptions<NewsApiOptions>>().Value;
    client.BaseAddress = new Uri(options.BaseUrl);
    client.DefaultRequestHeaders.Add("x-api-key", options.ApiKey);
});

// Set up Rawg Client
builder.Services.Configure<RawgOptions>(builder.Configuration.GetSection("Clients:Rawg"));
builder.Services.AddHttpClient<RawgClient>((sp, client) =>
{
    var options = sp.GetRequiredService<IOptions<RawgOptions>>().Value;
    client.BaseAddress = new Uri(options.BaseUrl);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
