using AA_Aggregation_API_AT;
using AA_Aggregation_API_AT.Aggregation.Interfaces;
using AA_Aggregation_API_AT.Aggregation.Services;
using AA_Aggregation_API_AT.Clients.NewsApi;
using AA_Aggregation_API_AT.Clients.OpenWeather;
using AA_Aggregation_API_AT.Clients.Rawg;
using AA_Aggregation_API_AT.Statistics.Interfaces;
using AA_Aggregation_API_AT.Statistics.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddOpenWeatherClient(builder.Configuration);
builder.Services.AddNewsApiClient(builder.Configuration);
builder.Services.AddRawgClient(builder.Configuration);
builder.Services.SetUpValidationMessages();
builder.Services.AddScoped<IAggregationService, AggregationService>();
builder.Services.AddSingleton<IStatisticsService, StatisticsService>();

builder.Services.AddMemoryCache();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
