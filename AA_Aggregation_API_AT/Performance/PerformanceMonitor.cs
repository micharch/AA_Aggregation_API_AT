using AA_Aggregation_API_AT.Statistics.Interfaces;

namespace AA_Aggregation_API_AT.Performance
{
    public class PerformanceMonitor : BackgroundService
    {
        private readonly IStatisticsService _stats;
        private readonly ILogger<PerformanceMonitor> _logger;
        private static readonly TimeSpan CheckInterval = TimeSpan.FromMinutes(1);
        private static readonly TimeSpan Window = TimeSpan.FromMinutes(5);
        private const double ThresholdFactor = 1.5;

        public PerformanceMonitor(
            IStatisticsService stats,
            ILogger<PerformanceMonitor> logger)
        {
            _stats = stats;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("PerformanceMonitor started, checking every {Interval}", CheckInterval);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    foreach (var api in _stats.GetApiNames())
                    {
                        var globalAvg = _stats.GetGlobalAverageMs(api);
                        var windowAvg = _stats.GetWindowAverageMs(api, Window);

                        if (globalAvg > 0 && windowAvg > globalAvg * ThresholdFactor)
                        {
                            _logger.LogWarning(
                              "Performance anomaly for {Api}: 5-min avg = {WindowAvg:F1}ms, " +
                              "global avg = {GlobalAvg:F1}ms (> {Factor:P0} threshold)",
                              api, windowAvg, globalAvg, ThresholdFactor - 1
                            );
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in PerformanceMonitor loop");
                }

                await Task.Delay(CheckInterval, stoppingToken);
            }
        }
    }
}
