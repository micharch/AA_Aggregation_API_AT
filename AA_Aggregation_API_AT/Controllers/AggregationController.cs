using AA_Aggregation_API_AT.Aggregation.Interfaces;
using AA_Aggregation_API_AT.Aggregation.Models;
using Microsoft.AspNetCore.Mvc;

namespace AA_Aggregation_API_AT.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AggregationController : ControllerBase
    {
        private readonly IAggregationService _aggregationService;

        public AggregationController(IAggregationService aggregationService)
        {
            _aggregationService = aggregationService;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] AggregationRequest request)
        {
            var response = await _aggregationService.GetAllAsync(request);
            return response is null
                ? NotFound()
                : Ok(response);
        }
    }
}
