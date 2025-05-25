using Microsoft.Extensions.Options;
using AA_Aggregation_API_AT.Clients.Rawg.Options;
using AA_Aggregation_API_AT.Clients.Rawg.Models;
using AA_Aggregation_API_AT.Aggregation.Interfaces;
using AA_Aggregation_API_AT.Aggregation.Models;
using Microsoft.Extensions.Caching.Memory;
using AA_Aggregation_API_AT.Cache;

namespace AA_Aggregation_API_AT.Clients.Rawg
{
    public class RawgClient : IApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly IOptions<RawgOptions> _options;
        private readonly ILogger<RawgClient> _logger;
        private readonly IMemoryCache _cache;
        private static readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(10);
        public string Name { get; }

        public RawgClient(
            HttpClient httpClient,
            IOptions<RawgOptions> newsApiOptions,
            ILogger<RawgClient> logger,
            IMemoryCache cache)
        {
            _httpClient = httpClient;
            _options = newsApiOptions;
            _logger = logger;
            this.Name = "Games";
            _cache = cache;
        }


        public async Task<object?> GetResultsAsync(AggregationRequest request)
        {
            var key = $"{Name}_{request.ToCacheKey()}";

            if (_cache.TryGetValue(key, out var value)) { return value; }

            try
            {
                var url = $"genres?key={_options.Value.ApiKey}";
                var message = await _httpClient.GetAsync(url);
                var result = await message.Content.ReadFromJsonAsync<RawgResponse>();

                if (result is not null)
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
                return new RawgResponse { Success = false, ErrorMessage = "An error has occured" };
            }
        }
    }
}
