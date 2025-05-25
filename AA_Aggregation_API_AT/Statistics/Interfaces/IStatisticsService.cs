using AA_Aggregation_API_AT.Statistics.Models;

namespace AA_Aggregation_API_AT.Statistics.Interfaces
{
    public interface IStatisticsService
    {
        void Record(string apiName, TimeSpan responseTime);
        StatisticsResult GetStatistics();
        IEnumerable<string> GetApiNames();
        double GetGlobalAverageMs(string apiName);
        double GetWindowAverageMs(string apiName, TimeSpan window);
    }
}
