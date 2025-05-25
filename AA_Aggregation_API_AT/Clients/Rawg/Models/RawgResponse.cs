using AA_Aggregation_API_AT.Clients.Base;
using System.Text.Json.Serialization;
#nullable disable
namespace AA_Aggregation_API_AT.Clients.Rawg.Models
{
    public class RawgResponse : ApiResponseBase
    {
        [JsonPropertyName("count")]
        public int TotalResults { get; set; }
        [JsonPropertyName("results")]
        public List<RawgGenre> Genres { get; set; }
    }

    public class RawgGenre
    {
        [JsonPropertyName("id")]
        public int ID { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("slug")]
        public string Slug { get; set; }
        [JsonPropertyName("games")]
        public List<RawgGame> Games { get; set; }
    }

    public class RawgGame
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}
