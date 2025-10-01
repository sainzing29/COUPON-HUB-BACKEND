using Microsoft.AspNetCore.Mvc;
using CouponHub.Business.Interfaces;
using CouponHub.Business.DTOs;

namespace CouponHub.Api.Controllers
{
public class DashboardController : BaseController
{
     private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    [HttpGet("stats")]
    public async Task<IActionResult> GetStats([FromQuery] int? serviceCenterId)
    {
        return await HandleServiceCenterAuthorizationAsync(serviceCenterId, async (effectiveServiceCenterId) =>
        {
            var stats = await _dashboardService.GetDashboardStatsAsync(effectiveServiceCenterId).ConfigureAwait(false);
            return Ok(stats);
        }).ConfigureAwait(false);
    }

    [HttpGet("widgets")]
    public async Task<IActionResult> GetDashboardWidgets([FromQuery] int? serviceCenterId, [FromQuery] int months = 12)
    {
        return await HandleServiceCenterAuthorizationAsync(serviceCenterId, async (effectiveServiceCenterId) =>
        {
            var widgets = await _dashboardService.GetDashboardWidgetsAsync(effectiveServiceCenterId, months).ConfigureAwait(false);
            return Ok(widgets);
        }).ConfigureAwait(false);
    }
}
}
