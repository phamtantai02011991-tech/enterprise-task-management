using TaskManagementWeb.Models.ViewModels.Admin;

namespace TaskManagementWeb.Services.Admin
{
    public interface IAdminDashboardService
    {
        Task<AdminDashboardViewModel> GetAdminDashboardDataAsync();
    }
}
