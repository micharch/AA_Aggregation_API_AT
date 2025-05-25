using Microsoft.AspNetCore.Mvc;

namespace AA_Aggregation_API_AT
{
    public static class ApiHelper
    {
        public static IServiceCollection SetUpValidationMessages(this IServiceCollection services)
        {
            services.Configure<ApiBehaviorOptions>(opts =>
            {
                opts.InvalidModelStateResponseFactory = context =>
                {
                    var errors = context.ModelState
                        .Where(x => x.Value.Errors.Count > 0)
                        .Select(x => new {
                            Field = x.Key,
                            Message = x.Value.Errors.First().ErrorMessage
                        });

                    return new BadRequestObjectResult(new
                    {
                        Message = "Validation Failed",
                        Errors = errors
                    });
                };
            });

            return services;
        }
    }
}
