using AA_Aggregation_API_AT.Statistics.Interfaces;
using AA_Aggregation_API_AT.Statistics.Models;
using Microsoft.AspNetCore.Mvc;

namespace AA_Aggregation_API_AT.Statistics.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StatisticsController : ControllerBase
    {
        private readonly IStatisticsService _stats;

        public StatisticsController(IStatisticsService stats)
        {
            _stats = stats;
        }

        [HttpGet]
        public ActionResult<StatisticsResult> Get()
        {
            var result = _stats.GetStatistics();
            return Ok(result);
        }
    }
}
