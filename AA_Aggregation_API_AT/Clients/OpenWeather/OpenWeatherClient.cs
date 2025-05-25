using AA_Aggregation_API_AT.Aggregation.Interfaces;
using AA_Aggregation_API_AT.Aggregation.Models;
using AA_Aggregation_API_AT.Clients.Base;
using AA_Aggregation_API_AT.Clients.OpenWeather.Models;
using AA_Aggregation_API_AT.Clients.OpenWeather.Options;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using static AA_Aggregation_API_AT.Cache.CacheHelper;

namespace AA_Aggregation_API_AT.Clients.OpenWeather
{
    public class OpenWeatherClient : IApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly IOptions<OpenWeatherOptions> _options;
        private readonly ILogger<OpenWeatherClient> _logger;
        private readonly IMemoryCache _cache;
        private static readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(10);
        private static string[] _includedProperties =
            {
                nameof(AggregationRequest.City)
            };
        public string Name { get; }

        public OpenWeatherClient(
            HttpClient httpClient,
            IOptions<OpenWeatherOptions> openWeatherOptions,
            ILogger<OpenWeatherClient> logger
,
            IMemoryCache cache)
        {
            _httpClient = httpClient;
            _options = openWeatherOptions;
            _logger = logger;
            this.Name = "Weather";
            _cache = cache;
        }

        public async Task<object?> GetResultsAsync(AggregationRequest request)
        {
            var key = $"{Name}_{request.ToCacheKey(_includedProperties)}";

            if (_cache.TryGetValue(key, out var value)) { return value; }

            try
            {
                var url = $"?q={request.City}&units={_options.Value.Units}&appid={_options.Value.ApiKey}";
                var message = await _httpClient.GetAsync(url);
                var result = await message.Content.ReadFromJsonAsync<OpenWeatherResponse>();

                if (result is not null && result.Status == 200)
                {
                    _cache.Set(key, result, _cacheDuration);
                    result.Success = true;
                }

                return result;
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, "Internal Error");

                if (_cache.TryGetValue(key, out var stale))
                    return stale;
                return new OpenWeatherResponse { Success = false, ErrorMessage = "An error has occured" };
            }
        }
    }
}
