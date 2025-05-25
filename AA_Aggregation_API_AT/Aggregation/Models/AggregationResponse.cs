using AA_Aggregation_API_AT.Clients.Base;

namespace AA_Aggregation_API_AT.Aggregation.Models
{
    public class AggregationResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public Dictionary<string, object?> Results { get; set; } = new();

        public void EvaluateStatus()
        {
            var responses = Results
                .Values
                .OfType<ApiResponseBase>()
                .ToList();

            if (responses.Count == 0)
            {
                Success = false;
                Message = "No data found";
                return;
            }

            Success = responses.All(r => r.Success);

            var errors = responses
                .Where(r => !r.Success && !string.IsNullOrWhiteSpace(r.ErrorMessage))
                .Select(r => r.ErrorMessage!.Trim());

            Message = errors.Any()
                ? string.Join(". ", errors)
                : string.Empty;
        }
    }
}
