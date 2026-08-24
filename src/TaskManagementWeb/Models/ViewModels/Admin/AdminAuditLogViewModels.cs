using TaskManagementWeb.Models.Entities;

namespace TaskManagementWeb.Models.ViewModels.Admin
{
    public class AuditLogListViewModel
    {
        public List<AuditLogItemViewModel> Logs { get; set; } = new();
        public string? SearchKey { get; set; }
        public string? ActionFilter { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public List<string> AvailableActions { get; set; } = new();
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; } = 1;
        public int TotalItems { get; set; }
    }

    public class AuditLogItemViewModel
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public int? ProjectId { get; set; }
        public string? ProjectTitle { get; set; }
        public string Action { get; set; } = string.Empty;
        public string EntityName { get; set; } = string.Empty;
        public string? Details { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
