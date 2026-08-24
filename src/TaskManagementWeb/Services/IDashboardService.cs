using TaskManagementWeb.Models.ViewModels;

namespace TaskManagementWeb.Services
{
    public interface IDashboardService
    {
        Task<DashboardViewModel> GetDashboardStatsAsync(int userId, string roleName);
    }
}
