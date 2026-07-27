using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pizza_API.Entities.Dtos.Dashboard;
using Pizza_API.Services;

namespace Pizza_API.Controllers
{
    [Route("api/admin/dashboard")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class DashboardAdminController : ControllerBase
    {
        private readonly IDashboardAdminService _dashboardAdminService;

        public DashboardAdminController(IDashboardAdminService dashboardAdminService)
        {
            _dashboardAdminService = dashboardAdminService;
        }

        [HttpGet("today-kpis")]
        public async Task<ActionResult<TodayKpisDto>> GetTodayKpis(
            CancellationToken cancellationToken)
        {
            var kpis = await _dashboardAdminService.GetTodayKpisAsync(
                cancellationToken);

            return Ok(kpis);
        }
    }
}
