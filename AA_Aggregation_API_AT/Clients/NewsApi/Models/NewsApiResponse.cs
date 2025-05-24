using System.Text.Json.Serialization;
#nullable disable
namespace AA_Aggregation_API_AT.Clients.NewsApi.Models
{
    public class NewsApiResponse
    {
        [JsonPropertyName("status")]
        public string Status { get; set; }
        [JsonPropertyName("totalResults")]
        public int TotalResults { get; set; }
        [JsonPropertyName("articles")]
        public List<NewsApiArticle> Articles { get; set; }
    }

    public class NewsApiArticle
    {
        [JsonPropertyName("source")]
        public NewsApiArticleSource Source { get; set; }
        [JsonPropertyName("author")]
        public string Author { get; set; }
        [JsonPropertyName("title")]
        public string Title { get; set; }
        [JsonPropertyName("description")]
        public string Description { get; set; }
        [JsonPropertyName("url")]
        public string Url { get; set; }
        [JsonPropertyName("urlToImage")]
        public string UrlToImage { get; set; }
        [JsonPropertyName("content")]
        public string Content { get; set; }
        [JsonPropertyName("publishedAt")]
        public DateTime PublishedAt { get; set; }
    }

    public class NewsApiArticleSource
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}