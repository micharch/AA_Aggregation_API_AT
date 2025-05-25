using Microsoft.Extensions.Options;
using AA_Aggregation_API_AT.Clients.NewsApi.Options;
using AA_Aggregation_API_AT.Clients.NewsApi.Models;
using AA_Aggregation_API_AT.Aggregation.Interfaces;
using AA_Aggregation_API_AT.Aggregation.Models;
using Microsoft.AspNetCore.WebUtilities;
using System.Globalization;
using static AA_Aggregation_API_AT.Cache.CacheHelper;
using Microsoft.Extensions.Caching.Memory;

namespace AA_Aggregation_API_AT.Clients.NewsApi
{
    public class NewsApiClient : IApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<NewsApiClient> _logger;
        private readonly IMemoryCache _cache;
        private static readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(1);
        private static string[] _includedProperties =
            {
                nameof(AggregationRequest.Query),
                nameof(AggregationRequest.From),
                nameof(AggregationRequest.To),
                nameof(AggregationRequest.Sort)
            };
        public string Name { get; }

        public NewsApiClient(
            HttpClient httpClient,
            IOptions<NewsApiOptions> newsApiOptions,
            ILogger<NewsApiClient> logger,
            IMemoryCache cache)
        {
            _httpClient = httpClient;
            _logger = logger;
            this.Name = "News";
            _cache = cache;
        }

        public async Task<object?> GetResultsAsync(AggregationRequest request)
        {
            var key = $"{Name}_{request.ToCacheKey(_includedProperties)}";

            if (_cache.TryGetValue(key, out var value)) { return value; }

            try
            {
                var baseUrl = $"everything?q={Uri.EscapeDataString(request.Query)}";
                var url = DefineUrl(baseUrl, request);
                var message = await _httpClient.GetAsync(url);
                var result = await message.Content.ReadFromJsonAsync<NewsApiResponse>();

                if (result is not null && result.Status == "ok")
                {
                    _cache.Set(key, result, _cacheDuration);
                    result.Success = true;
                }
                else if (result is not null)
                {
                    result.ErrorMessage = $"{Name} client. No data found";
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Internal Error");

                if (_cache.TryGetValue(key, out var stale))
                    return stale;
                return new NewsApiResponse { Success = false, ErrorMessage = "An error has occured" };
            }
        }

        private string DefineUrl(string baseUrl, AggregationRequest request)
        {
            var extraParameters = new Dictionary<string, string>();

            if (request.From.HasValue) extraParameters["from"] = request.From.Value.ToString("o", CultureInfo.InvariantCulture);
            if (request.To.HasValue) extraParameters["to"] = request.To.Value.ToString("o", CultureInfo.InvariantCulture);
            if (request.Sort.HasValue) extraParameters["sortBy"] = request.Sort.Value.ToString().ToLower();


            return extraParameters.Count > 0
                ? QueryHelpers.AddQueryString(baseUrl, extraParameters!) 
                : baseUrl;
        }
    }
}
