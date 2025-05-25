using AA_Aggregation_API_AT.Aggregation.Interfaces;
using AA_Aggregation_API_AT.Clients.NewsApi.Options;
using AA_Aggregation_API_AT.Clients.NewsApi;
using Microsoft.Extensions.Options;
using Polly.Extensions.Http;
using Polly;
using AA_Aggregation_API_AT.Clients.Rawg.Options;

namespace AA_Aggregation_API_AT.Clients.Rawg
{
    public static class RawgHelper
    {
        public static IServiceCollection AddRawgClient(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<RawgOptions>(config.GetSection("Clients:Rawg"));
            services.AddHttpClient<RawgClient>((sp, client) =>
            {
                var options = sp.GetRequiredService<IOptions<RawgOptions>>().Value;
                client.BaseAddress = new Uri(options.BaseUrl);
            }).AddPolicyHandler((sp, request) =>
            {
                var logger = sp.GetRequiredService<ILogger<RawgClient>>();
                return HttpPolicyExtensions
                  .HandleTransientHttpError()
                  .WaitAndRetryAsync(
                    retryCount: 3,
                    sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    onRetry: (outcome, timespan, retryNumber, ctx) =>
                    {
                        logger.LogWarning(
                          "[RawgClient] Retry {RetryNumber} after {Delay}s because {Error}",
                          retryNumber,
                          timespan.TotalSeconds,
                          outcome.Exception?.Message ?? outcome.Result.StatusCode.ToString()
                        );
                    }
                  );
            })
              .AddPolicyHandler((sp, request) =>
              {
                  var logger = sp.GetRequiredService<ILogger<RawgClient>>();
                  return HttpPolicyExtensions
                    .HandleTransientHttpError()
                    .CircuitBreakerAsync(
                      handledEventsAllowedBeforeBreaking: 5,
                      durationOfBreak: TimeSpan.FromMinutes(1),
                      onBreak: (outcome, breakDelay, ctx) =>
                      {
                          logger.LogError(
                            "[RawgClient] Circuit opened for {BreakDelay}s due to {Error}",
                            breakDelay.TotalSeconds,
                            outcome.Exception?.Message ?? outcome.Result.StatusCode.ToString()
                          );
                      },
                      onReset: ctx =>
                      {
                          logger.LogInformation("[RawgClient] Circuit closed; operations flow normally.");
                      }
                    );
              });

            services.AddTransient<IApiClient>(sp => sp.GetRequiredService<RawgClient>());

            return services;
        }
    }
}
