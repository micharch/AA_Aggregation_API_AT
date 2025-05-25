using AA_Aggregation_API_AT.Aggregation.Interfaces;
using AA_Aggregation_API_AT.Clients.NewsApi.Options;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Extensions.Http;

namespace AA_Aggregation_API_AT.Clients.NewsApi
{
    public static class NewsApiHelper
    {
        public static IServiceCollection AddNewsApiClient(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<NewsApiOptions>(config.GetSection("Clients:NewsApi"));
            services.AddHttpClient<NewsApiClient>((sp, client) =>
            {
                var options = sp.GetRequiredService<IOptions<NewsApiOptions>>().Value;
                client.BaseAddress = new Uri(options.BaseUrl);
                client.DefaultRequestHeaders.Add("x-api-key", options.ApiKey);
            }).AddPolicyHandler((sp, request) =>
            {
                var logger = sp.GetRequiredService<ILogger<NewsApiClient>>();
                return HttpPolicyExtensions
                  .HandleTransientHttpError()
                  .WaitAndRetryAsync(
                    retryCount: 3,
                    sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    onRetry: (outcome, timespan, retryNumber, ctx) =>
                    {
                        logger.LogWarning(
                          "[NewsApiClient] Retry {RetryNumber} after {Delay}s because {Error}",
                          retryNumber,
                          timespan.TotalSeconds,
                          outcome.Exception?.Message ?? outcome.Result.StatusCode.ToString()
                        );
                    }
                  );
            })
              .AddPolicyHandler((sp, request) =>
              {
                  var logger = sp.GetRequiredService<ILogger<NewsApiClient>>();
                  return HttpPolicyExtensions
                    .HandleTransientHttpError()
                    .CircuitBreakerAsync(
                      handledEventsAllowedBeforeBreaking: 5,
                      durationOfBreak: TimeSpan.FromMinutes(1),
                      onBreak: (outcome, breakDelay, ctx) =>
                      {
                          logger.LogError(
                            "[NewsApiClient] Circuit opened for {BreakDelay}s due to {Error}",
                            breakDelay.TotalSeconds,
                            outcome.Exception?.Message ?? outcome.Result.StatusCode.ToString()
                          );
                      },
                      onReset: ctx =>
                      {
                          logger.LogInformation("[NewsApiClient] Circuit closed; operations flow normally.");
                      }
                    );
              });

            services.AddTransient<IApiClient>(sp => sp.GetRequiredService<NewsApiClient>());

            return services;
        }
    }
}
