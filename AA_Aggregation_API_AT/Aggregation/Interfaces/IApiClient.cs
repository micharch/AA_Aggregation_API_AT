using AA_Aggregation_API_AT.Aggregation.Models;
using AA_Aggregation_API_AT.Clients.NewsApi.Models;

namespace AA_Aggregation_API_AT.Aggregation.Interfaces
{
    public interface IApiClient
    {
        string Name { get; }
        Task<object?> GetResultsAsync(AggregationRequest request);
    }
}
