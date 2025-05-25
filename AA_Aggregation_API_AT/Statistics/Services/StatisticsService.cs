using AA_Aggregation_API_AT.Statistics.Interfaces;
using AA_Aggregation_API_AT.Statistics.Models;
using System.Collections.Concurrent;

namespace AA_Aggregation_API_AT.Statistics.Services
{
    public class StatisticsService : IStatisticsService
    {
        private readonly ConcurrentDictionary<string, ApiMetrics> _store = new();

        public void Record(string apiName, TimeSpan responseTime)
        {
            var metrics = _store.GetOrAdd(apiName, _ => new ApiMetrics());
            metrics.Add(responseTime);
        }

        public StatisticsResult GetStatistics()
        {
            var result = new StatisticsResult();

            foreach (var kv in _store)
            {
                var apiName = kv.Key;
                var metrics = kv.Value;

                var total = metrics.TotalRequests;
                var avg = metrics.TotalRequests > 0
                             ? metrics.TotalResponseTimeMs / metrics.TotalRequests
                             : 0;

                var buckets = new Dictionary<PerformanceBucket, int>
                {
                    [PerformanceBucket.Fast] = metrics.FastCount,
                    [PerformanceBucket.Average] = metrics.AverageCount,
                    [PerformanceBucket.Slow] = metrics.SlowCount
                };

                result.Apis.Add(new ApiStatsDto
                {
                    ApiName = apiName,
                    TotalRequests = total,
                    AverageResponseTimeMs = avg,
                    Buckets = buckets
                });
            }

            return result;
        }

        private class ApiMetrics
        {
            private long _totalRequests;
            private long _totalResponseTimeMs;
            private int _fast, _average, _slow;

            public int TotalRequests => (int)Interlocked.Read(ref _totalRequests);
            public long TotalResponseTimeMs => Interlocked.Read(ref _totalResponseTimeMs);

            public int FastCount => _fast;
            public int AverageCount => _average;
            public int SlowCount => _slow;

            public void Add(TimeSpan responseTime)
            {
                var ms = (long)responseTime.TotalMilliseconds;
                Interlocked.Increment(ref _totalRequests);
                Interlocked.Add(ref _totalResponseTimeMs, ms);

                if (ms < 100)
                    Interlocked.Increment(ref _fast);
                else if (ms < 200)
                    Interlocked.Increment(ref _average);
                else
                    Interlocked.Increment(ref _slow);
            }
        }
    }
}
