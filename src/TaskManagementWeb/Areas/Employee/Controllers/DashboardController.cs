using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskManagementWeb.Models.ViewModels;
using TaskManagementWeb.Services;

namespace TaskManagementWeb.Areas.Employee.Controllers
{
    [Area("Employee")]
    [Authorize(Roles = "Employee")]
    public class DashboardController : Controller
    {
        private readonly IDashboardService _dashboardService;
        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        public async Task<IActionResult> Index()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var roleClaim = User.FindFirstValue(ClaimTypes.Role) ?? "Employee";

            if (!int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized();
            }
            var dashboardModel = await _dashboardService
                .GetDashboardStatsAsync(userId, roleClaim);

            if (dashboardModel == null)
            {
                dashboardModel = new DashboardViewModel();
            }
            return View(dashboardModel);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            return RedirectToActionPermanentPreserveMethod(
                "Logout", "Account", new {area = ""}
            );
        }
    }
}