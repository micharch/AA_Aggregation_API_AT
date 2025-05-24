using AA_Aggregation_API_AT.Clients.OpenWeather.Models;
using AA_Aggregation_API_AT.Clients.OpenWeather.Options;
using Microsoft.Extensions.Options;

namespace AA_Aggregation_API_AT.Clients.OpenWeather
{
    public class OpenWeatherClient
    {
        private readonly HttpClient _httpClient;
        private readonly IOptions<OpenWeatherOptions> _options;
        private readonly ILogger<OpenWeatherClient> _logger;

        public OpenWeatherClient(
            HttpClient httpClient,
            IOptions<OpenWeatherOptions> openWeatherOptions,
            ILogger<OpenWeatherClient> logger
            )
        {
            _httpClient = httpClient;
            _options = openWeatherOptions;
            _logger = logger;
        }


        public async Task<OpenWeatherResponse?> GetResultsAsync(string query)
        {
            var url = $"?q={query}&units={_options.Value.Units}&appid={_options.Value.ApiKey}";
            var message = await _httpClient.GetAsync(url);
            return await message.Content.ReadFromJsonAsync<OpenWeatherResponse>();
        }
    }
}
