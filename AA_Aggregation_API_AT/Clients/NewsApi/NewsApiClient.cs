using Microsoft.Extensions.Options;
using AA_Aggregation_API_AT.Clients.NewsApi.Options;
using AA_Aggregation_API_AT.Clients.NewsApi.Models;

namespace AA_Aggregation_API_AT.Clients.NewsApi
{
    public class NewsApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<NewsApiClient> _logger;

        public NewsApiClient(
            HttpClient httpClient,
            IOptions<NewsApiOptions> newsApiOptions,
            ILogger<NewsApiClient> logger
            )
        {
            _httpClient = httpClient;
            _logger = logger;
        }


        public async Task<NewsApiResponse?> GetResultsAsync(string query)
        {
            var url = $"everything?q={query}";
            var message = await _httpClient.GetAsync(url);
            return await message.Content.ReadFromJsonAsync<NewsApiResponse>();
        }
    }
}
