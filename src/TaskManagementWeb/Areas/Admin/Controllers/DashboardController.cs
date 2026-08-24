using Microsoft.AspNetCore.Mvc;
using TaskManagementWeb.Services.Admin;

namespace TaskManagementWeb.Areas.Admin.Controllers
{
    public class DashboardController : AdminBaseController
    {
        private readonly IAdminDashboardService _dashboardService;

        public DashboardController(IAdminDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        public async Task<IActionResult> Index()
        {
            var model = await _dashboardService.GetAdminDashboardDataAsync();
            return View(model);
        }
    }
}
