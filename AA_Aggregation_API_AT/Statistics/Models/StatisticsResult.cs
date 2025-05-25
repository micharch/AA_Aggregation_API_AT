namespace AA_Aggregation_API_AT.Statistics.Models
{
    public class StatisticsResult
    {
        public List<ApiStatsDto> Apis { get; set; } = new();
    }

    public class ApiStatsDto
    {
        public string ApiName { get; set; } = null!;
        public int TotalRequests { get; set; }
        public double AverageResponseTimeMs { get; set; }
        public Dictionary<PerformanceBucket, int> Buckets { get; set; } = new();
    }

    public enum PerformanceBucket
    {
        Fast,       // < 100 ms
        Average,    // 100 – 200 ms
        Slow        // > 200 ms
    }
}
