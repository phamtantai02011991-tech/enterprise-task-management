using TaskManagementWeb.Models.ViewModels.Admin;

namespace TaskManagementWeb.Services.Admin
{
    public interface IAuditLogService
    {
        Task LogActionAsync(int userId, string userName, int? projectId, string action, string entityName, string details);
        Task<AuditLogListViewModel> GetAuditLogsAsync(string? searchKey, string? actionFilter, DateTime? startDate, DateTime? endDate, int page = 1, int pageSize = 15);
        Task<List<string>> GetDistinctActionsAsync();
    }
}
