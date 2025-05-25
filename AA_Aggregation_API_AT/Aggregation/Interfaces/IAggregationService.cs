using AA_Aggregation_API_AT.Aggregation.Models;

namespace AA_Aggregation_API_AT.Aggregation.Interfaces
{
    public interface IAggregationService
    {
        Task<AggregationResponse> GetAllAsync(AggregationRequest request);
    }
}
