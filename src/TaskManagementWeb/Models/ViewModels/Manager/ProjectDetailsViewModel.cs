using Microsoft.AspNetCore.Mvc.Rendering;
using TaskManagementWeb.Models.Entities;

namespace TaskManagementWeb.ViewModels.Manager
{
    public class ProjectDetailsViewModel
    {
        public Project Project { get; set; } = null!;
        
        public List<TaskItem> Tasks { get; set; } = new();
        
        public List<ProjectFile> ProjectFiles { get; set; } = new();
        
        public List<ProjectMember> ProjectMembers { get; set; } = new();

        public List<AuditLog> AuditLogs { get; set; } = new();

        public List<SelectListItem> AvailableUsersToAdd { get; set; } = new();

        // Thống kê nhanh
        public int TotalTasks => Tasks.Count;
        public int CompletedTasks => Tasks.Count(t => t.Status.ToString().Equals("Completed", StringComparison.OrdinalIgnoreCase));
        public int InProgressTasks => Tasks.Count(t => t.Status.ToString().Equals("InProgress", StringComparison.OrdinalIgnoreCase));
        public int PendingTasks => Tasks.Count(t => t.Status.ToString().Equals("Pending", StringComparison.OrdinalIgnoreCase));

        public int ProgressPercentage
        {
            get
            {
                if (TotalTasks > 0)
                {
                    return (int)Math.Round((double)CompletedTasks / TotalTasks * 100);
                }
                return Project?.Progress ?? 0;
            }
        }

        public int DaysRemaining => (int)(Project.EndDate.Date - DateTime.Today).TotalDays;
        public bool IsOverdue => DateTime.Today > Project.EndDate.Date && Project.Status != Models.Enums.ProjectStatus.Completed;

        public double TotalHoursSpent => Tasks.SelectMany(t => t.TimeLogs ?? Enumerable.Empty<TimeLog>()).Sum(tl => tl.HoursSpent);
    }
}