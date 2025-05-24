using AA_Aggregation_API_AT.Clients.NewsApi.Models;
using AA_Aggregation_API_AT.Clients.NewsApi.Options;
using AA_Aggregation_API_AT.Clients.NewsApi;
using Microsoft.Extensions.Options;
using AA_Aggregation_API_AT.Clients.Rawg.Options;
using AA_Aggregation_API_AT.Clients.Rawg.Models;

namespace AA_Aggregation_API_AT.Clients.Rawg
{
    public class RawgClient
    {
        private readonly HttpClient _httpClient;
        private readonly IOptions<RawgOptions> _options;
        private readonly ILogger<RawgClient> _logger;

        public RawgClient(
            HttpClient httpClient,
            IOptions<RawgOptions> newsApiOptions,
            ILogger<RawgClient> logger
            )
        {
            _httpClient = httpClient;
            _options = newsApiOptions;
            _logger = logger;
        }


        public async Task<RawgResponse?> GetResultsAsync()
        {
            var url = $"genres?key={_options.Value.ApiKey}";
            var message = await _httpClient.GetAsync(url);
            return await message.Content.ReadFromJsonAsync<RawgResponse>();
        }
    }
}
