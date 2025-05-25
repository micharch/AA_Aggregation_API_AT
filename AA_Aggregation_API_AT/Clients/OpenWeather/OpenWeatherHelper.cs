using AA_Aggregation_API_AT.Aggregation.Interfaces;
using Microsoft.Extensions.Options;
using Polly.Extensions.Http;
using Polly;
using AA_Aggregation_API_AT.Clients.OpenWeather.Options;

namespace AA_Aggregation_API_AT.Clients.OpenWeather
{
    public static class OpenWeatherHelper
    {
        public static IServiceCollection AddOpenWeatherClient(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<OpenWeatherOptions>(config.GetSection("Clients:OpenWeather"));
            services.AddHttpClient<OpenWeatherClient>((sp, client) =>
            {
                var options = sp.GetRequiredService<IOptions<OpenWeatherOptions>>().Value;
                client.BaseAddress = new Uri(options.BaseUrl);
            }).AddPolicyHandler((sp, request) =>
            {
                var logger = sp.GetRequiredService<ILogger<OpenWeatherClient>>();
                return HttpPolicyExtensions
                  .HandleTransientHttpError()
                  .WaitAndRetryAsync(
                    retryCount: 3,
                    sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    onRetry: (outcome, timespan, retryNumber, ctx) =>
                    {
                        logger.LogWarning(
                          "[OpenWeatherClient] Retry {RetryNumber} after {Delay}s because {Error}",
                          retryNumber,
                          timespan.TotalSeconds,
                          outcome.Exception?.Message ?? outcome.Result.StatusCode.ToString()
                        );
                    }
                  );
            })
              .AddPolicyHandler((sp, request) =>
              {
                  var logger = sp.GetRequiredService<ILogger<OpenWeatherClient>>();
                  return HttpPolicyExtensions
                    .HandleTransientHttpError()
                    .CircuitBreakerAsync(
                      handledEventsAllowedBeforeBreaking: 5,
                      durationOfBreak: TimeSpan.FromMinutes(1),
                      onBreak: (outcome, breakDelay, ctx) =>
                      {
                          logger.LogError(
                            "[OpenWeatherClient] Circuit opened for {BreakDelay}s due to {Error}",
                            breakDelay.TotalSeconds,
                            outcome.Exception?.Message ?? outcome.Result.StatusCode.ToString()
                          );
                      },
                      onReset: ctx =>
                      {
                          logger.LogInformation("[OpenWeatherClient] Circuit closed; operations flow normally.");
                      }
                    );
              });

            services.AddTransient<IApiClient>(sp => sp.GetRequiredService<OpenWeatherClient>());

            return services;
        }
    }
}
