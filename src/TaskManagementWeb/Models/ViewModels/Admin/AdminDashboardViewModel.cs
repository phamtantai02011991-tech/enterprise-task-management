using TaskManagementWeb.Models.Entities;
using TaskManagementWeb.Models.Enums;

namespace TaskManagementWeb.Models.ViewModels.Admin
{
    public class AdminDashboardViewModel
    {
        // 6 Thẻ KPI chính
        public int TotalUsers { get; set; }
        public int TotalProjects { get; set; }
        public int TotalTasks { get; set; }
        public int CompletedTasks { get; set; }
        public int OverdueTasks { get; set; }
        public int ActiveProjects { get; set; }

        public int TotalDepartments { get; set; }
        public int PlanningProjects { get; set; }
        public int OnHoldProjects { get; set; }
        public int OverallCompletionRate { get; set; } // % toàn hệ thống

        // Thống kê phân bố
        public List<RoleUserCountDto> RoleUserCounts { get; set; } = new();
        public List<DepartmentUserCountDto> DepartmentUserCounts { get; set; } = new();
        public List<ProjectPerformanceDto> ProjectPerformanceList { get; set; } = new();
        public List<AuditLogItemViewModel> RecentAuditLogs { get; set; } = new();
        public List<RecentUserDto> RecentUsers { get; set; } = new();
        public List<Announcement> ActiveAnnouncements { get; set; } = new();
    }

    public class RoleUserCountDto
    {
        public string RoleName { get; set; } = string.Empty;
        public int UserCount { get; set; }
        public double Percentage { get; set; }
    }

    public class ProjectPerformanceDto
    {
        public int Id { get; set; }
        public string ProjectCode { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public int Progress { get; set; }
        public int TotalTasks { get; set; }
        public int CompletedTasks { get; set; }
        public ProjectStatus Status { get; set; }
        public ProjectPriority Priority { get; set; }
        public string ManagerName { get; set; } = string.Empty;
        public DateTime EndDate { get; set; }
        public bool IsOverdue { get; set; }
    }

    public class DepartmentUserCountDto
    {
        public string DepartmentName { get; set; } = string.Empty;
        public int UserCount { get; set; }
    }

    public class RecentUserDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
