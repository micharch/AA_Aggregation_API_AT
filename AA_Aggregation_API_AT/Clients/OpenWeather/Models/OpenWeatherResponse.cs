using AA_Aggregation_API_AT.Clients.Base;
using System.Text.Json.Serialization;
#nullable disable
namespace AA_Aggregation_API_AT.Clients.OpenWeather.Models
{
    public class OpenWeatherResponse : ApiResponseBase
    {
        [JsonPropertyName("main")]
        public OpenWeatherMain Main { get; set; }
        [JsonPropertyName("cod")]
        public int Status { get; set; }
    }

    public class OpenWeatherMain
    {
        [JsonPropertyName("temp")]
        public decimal Temp { get; set; }
        [JsonPropertyName("feels_like")]
        public decimal FeelsLike { get; set; }
        [JsonPropertyName("temp_min")]
        public decimal TempMin { get; set; }
        [JsonPropertyName("temp_max")]
        public decimal TempMax { get; set; }
        [JsonPropertyName("pressure")]
        public decimal Pressure { get; set; }
        [JsonPropertyName("humidity")]
        public decimal Humidity { get; set; }
    }
}
