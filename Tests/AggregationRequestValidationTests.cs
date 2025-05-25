using AA_Aggregation_API_AT.Aggregation.Models;
using System.ComponentModel.DataAnnotations;

namespace Tests
{
    public class AggregationRequestValidationTests
    {
        [Fact]
        public void ValidRequest_NoValidationErrors()
        {
            // Arrange: Valid Request
            var req = new AggregationRequest
            {
                City = "Athens",
                Query = "bitcoin",
                From = new DateTime(2025, 5, 1),
                To = new DateTime(2025, 5, 2),
                Sort = SortOptions.PublishedAt
            };

            // Act
            var errors = ValidateModel(req);

            // Assert
            Assert.Empty(errors);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void MissingCity_ShouldProduceRequiredError(string? city)
        {
            var req = new AggregationRequest
            {
                City = city,
                Query = "x",
                Sort = SortOptions.Relevancy
            };
            var errors = ValidateModel(req);

            Assert.Contains(errors, e =>
                e.MemberNames.Contains(nameof(AggregationRequest.City))
                && e.ErrorMessage!.Contains("required")
            );
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void MissingQuery_ShouldProduceRequiredError(string? query)
        {
            var req = new AggregationRequest
            {
                City = "Athens",
                Query = query,
                Sort = SortOptions.Relevancy
            };
            var errors = ValidateModel(req);

            Assert.Contains(errors, e =>
                e.MemberNames.Contains(nameof(AggregationRequest.Query))
                && e.ErrorMessage!.Contains("required")
            );
        }

        [Fact]
        public void FromAfterTo_ShouldProduceCrossFieldError()
        {
            var req = new AggregationRequest
            {
                City = "Athens",
                Query = "x",
                From = new DateTime(2025, 6, 1),
                To = new DateTime(2025, 5, 1),
                Sort = SortOptions.Relevancy
            };
            var errors = ValidateModel(req);

            Assert.Contains(errors, e =>
                e.MemberNames.Contains(nameof(AggregationRequest.From))
                && e.MemberNames.Contains(nameof(AggregationRequest.To))
                && e.ErrorMessage!.Contains("earlier")
            );
        }

        [Fact]
        public void InvalidSortEnum_ShouldProduceEnumError()
        {
            var invalid = (SortOptions)999;
            var req = new AggregationRequest
            {
                City = "Athens",
                Query = "x",
                Sort = invalid
            };
            var errors = ValidateModel(req);

            Assert.Contains(errors, e =>
                e.MemberNames.Contains(nameof(AggregationRequest.Sort))
                && e.ErrorMessage!.Contains("Invalid sort option")
            );
        }

        private IList<ValidationResult> ValidateModel(object model)
        {
            var results = new List<ValidationResult>();
            var context = new ValidationContext(model, serviceProvider: null, items: null);
            Validator.TryValidateObject(model, context, results, validateAllProperties: true);
            if (model is IValidatableObject validatable)
            {
                foreach (var vr in validatable.Validate(context))
                    results.Add(vr);
            }
            return results;
        }
    }
}
