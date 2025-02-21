using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AshishGeneralStore.Common;
using AshishGeneralStore.Services.Admin.Dashboard;

namespace AshishGeneralStore.Controllers.Admin.Dashboard
{

    [ApiController]
    [Route($"{Constants.ApiRoutes.AdminInventoryBase}/analytics")]
    [Authorize(Roles = "Admin")]
    public class AnalyticsController : ControllerBase
    {
        private readonly IAnalyticsService _analyticsService;

        public AnalyticsController(IAnalyticsService analyticsService)
        {
            _analyticsService = analyticsService;
        }

        [HttpGet("purchase-sell-stats")]
        public async Task<IActionResult> GetPurchaseSellStats(
      [FromQuery] string periodType,
      [FromQuery] int? daysCount = null,
      [FromQuery] DateTime? startDate = null,
      [FromQuery] DateTime? endDate = null,
      [FromQuery] int? categoryId = null)
        {
            if (string.IsNullOrEmpty(periodType))
            {
                return BadRequest("Period type is required.");
            }

            if (startDate.HasValue && endDate.HasValue && startDate > endDate)
            {
                return BadRequest("Start date cannot be after end date.");
            }

            var stats = await _analyticsService.GetPurchaseSellStatsAsync(periodType, daysCount, startDate, endDate, categoryId);

            if (stats == null)
            {
                return NotFound("No statistics found for the given parameters.");
            }

            return Ok(stats);
        }

    }
}
