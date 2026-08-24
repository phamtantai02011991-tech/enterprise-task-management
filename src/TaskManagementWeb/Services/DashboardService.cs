using Microsoft.EntityFrameworkCore;
using TaskManagementWeb.Data;
using TaskManagementWeb.Models.Enums;
using TaskManagementWeb.Models.ViewModels;

namespace TaskManagementWeb.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly ApplicationDbContext _context;
        public DashboardService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardViewModel> GetDashboardStatsAsync(int userId, string roleName)
        {
            var taskQuery = _context.TaskItems
                .Include(t => t.Project)
                .Include(t => t.AssignedUser)
                .AsQueryable();

            if (roleName == "Employee")
            {
                taskQuery = taskQuery.Where(t => t.AssignedUserId == userId);
            }

            var totalProjects = await _context.Projects.CountAsync();
            var totalTasks = await taskQuery.CountAsync();
            var completedTasks = await taskQuery.CountAsync(t => t.Status == TaskStatusEnum.Completed);
            var inProgressTasks = await taskQuery.CountAsync(t => t.Status == TaskStatusEnum.InProgress);
            var overdueTasks = await taskQuery.CountAsync(t => t.Deadline < DateTime.Today && t.Status != TaskStatusEnum.Completed);

            var recentTasks = await taskQuery
                .OrderBy(t => t.Deadline)
                .Take(5)
                .Select(t => new TaskViewModel
                {
                    Id = t.Id,
                    Title = t.Title,
                    Priority = t.Priority,
                    Status = t.Status,
                    Deadline = t.Deadline,
                    ProjectId = t.ProjectId,
                    ProjectTitle = t.Project != null ? t.Project.Title : "N/A",
                    AssignedUserId = t.AssignedUserId,
                    AssignedUserName = t.AssignedUser != null ? t.AssignedUser.FullName : "unassigned"
                })
                .ToListAsync();

            return new DashboardViewModel
            {
                TotalProjects = totalProjects,
                TotalTasks = totalTasks,
                CompletedTasks = completedTasks,
                InProgressTasks = inProgressTasks,
                OverdueTasks = overdueTasks,
                RecentTasks = recentTasks
            };
        }
    }
}
