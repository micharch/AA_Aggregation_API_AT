using AA_Aggregation_API_AT.Clients.NewsApi;
using AA_Aggregation_API_AT.Clients.OpenWeather;
using AA_Aggregation_API_AT.Clients.Rawg;
using Microsoft.AspNetCore.Mvc;

namespace AA_Aggregation_API_AT.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AggregationController : ControllerBase
    {

        private readonly ILogger<AggregationController> _logger;
        private readonly OpenWeatherClient _openWeatherClient;
        private readonly NewsApiClient _newsApiClient;
        private readonly RawgClient _rawgClient;

        public AggregationController(
            ILogger<AggregationController> logger,
            OpenWeatherClient openWeatherClient,
            NewsApiClient newsApiClient,
            RawgClient rawgClient)
        {
            _logger = logger;
            _openWeatherClient = openWeatherClient;
            _newsApiClient = newsApiClient;
            _rawgClient = rawgClient;
        }

        //[HttpGet("{query}")]
        //public async Task<IActionResult> Get(string query)
        //{
        //    var cityWeather = await _openWeatherClient.GetCityInfoAsync(query);
        //    return cityWeather is null 
        //        ? NotFound()
        //        : Ok(cityWeather);
        //}

        //[HttpGet("{query}")]
        //public async Task<IActionResult> Get(string query)
        //{
        //    var response = await _newsApiClient.GetResultsAsync(query);
        //    return response is null
        //        ? NotFound()
        //        : Ok(response);
        //}

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var response = await _rawgClient.GetResultsAsync();
            return response is null
                ? NotFound()
                : Ok(response);
        }
    }
}
