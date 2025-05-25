using AA_Aggregation_API_AT.Aggregation.Interfaces;
using AA_Aggregation_API_AT.Aggregation.Models;
using AA_Aggregation_API_AT.Statistics.Interfaces;
using System.Diagnostics;

namespace AA_Aggregation_API_AT.Aggregation.Services
{
    public class AggregationService : IAggregationService
    {
        private readonly IEnumerable<IApiClient> _clients;
        private readonly IStatisticsService _statisticsService;
        private readonly ILogger<AggregationService> _logger;
        public AggregationService(
            IEnumerable<IApiClient> clients,
            IStatisticsService statisticsService,
            ILogger<AggregationService> logger)
        {
            _clients = clients;
            _statisticsService = statisticsService;
            _logger = logger;
        }

        public async Task<AggregationResponse> GetAllAsync(AggregationRequest request)
        {
            var results = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);

            var tasks = new List<Task<(string Client, object? Data)>>();

            foreach (var client in _clients)
            {
                tasks.Add(FetchAsync(client, request));
            }

            var completed = await Task.WhenAll(tasks);

            foreach (var (Client, Data) in completed)
            {
                results[Client] = Data;
            }

            var response = new AggregationResponse { Results = results};
            response.EvaluateStatus();

            return response;
        }

        private async Task<(string Client, object? Data)> FetchAsync(IApiClient client, AggregationRequest request)
        {
            object? data = null;
            var sw = Stopwatch.StartNew();
            try
            {
                data = await client.GetResultsAsync(request);
                return (client.Name, data);
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, "Error on calling {0}", client.Name);
                return (client.Name, null);
            }
            finally
            {
                sw.Stop();
                _statisticsService.Record(client.Name, sw.Elapsed);
            }
        }
    }
}
