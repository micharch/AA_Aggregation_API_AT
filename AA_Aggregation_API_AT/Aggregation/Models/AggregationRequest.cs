using System.ComponentModel.DataAnnotations;

namespace AA_Aggregation_API_AT.Aggregation.Models
{
    public class AggregationRequest : IValidatableObject
    {
        [Required(AllowEmptyStrings = false)]
        public string? City { get; init; }
        [Required(AllowEmptyStrings = false)]
        public string? Query { get; init; }
        public DateTime? From { get; init; }
        public DateTime? To { get; init; }
        public SortOptions? Sort { get; init; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (From.HasValue && To.HasValue && From > To)
            {
                yield return new ValidationResult(
                    "'From' must be earlier than or equal to 'To'.",
                    new[] { nameof(From), nameof(To) }
                );
            }

            if (Sort.HasValue && !Enum.IsDefined(typeof(SortOptions), Sort.Value))
            {
                yield return new ValidationResult(
                    $"Invalid sort option '{Sort}'.",
                    new[] { nameof(Sort) }
                );
            }
        }
    }

    public enum SortOptions
    {
        Relevancy,
        Popularity,
        PublishedAt
    }
}
