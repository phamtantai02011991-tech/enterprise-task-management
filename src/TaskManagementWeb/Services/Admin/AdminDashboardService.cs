using Microsoft.EntityFrameworkCore;
using TaskManagementWeb.Data;
using TaskManagementWeb.Models.Entities;
using TaskManagementWeb.Models.Enums;
using TaskManagementWeb.Models.ViewModels.Admin;

namespace TaskManagementWeb.Services.Admin
{
    public class AdminDashboardService : IAdminDashboardService
    {
        private readonly ApplicationDbContext _context;

        public AdminDashboardService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<AdminDashboardViewModel> GetAdminDashboardDataAsync()
        {
            // 1. Core KPIs
            int totalUsers = await _context.Users.CountAsync();
            int totalDepartments = await _context.Departments.CountAsync();
            int totalProjects = await _context.Projects.CountAsync();
            int totalTasks = await _context.TaskItems.CountAsync();

            int completedTasks = await _context.TaskItems.CountAsync(t => t.Status == TaskStatusEnum.Completed);
            
            // Overdue tasks: deadline passed and not completed
            var now = DateTime.Today;
            int overdueTasks = await _context.TaskItems.CountAsync(t => t.Deadline < now && t.Status != TaskStatusEnum.Completed);

            int activeProjects = await _context.Projects.CountAsync(p => !p.IsArchived && p.Status == ProjectStatus.Active);
            int planningProjects = await _context.Projects.CountAsync(p => !p.IsArchived && p.Status == ProjectStatus.Planning);
            int onHoldProjects = await _context.Projects.CountAsync(p => !p.IsArchived && p.Status == ProjectStatus.OnHold);

            int overallCompletionRate = totalTasks > 0 ? (int)Math.Round((double)completedTasks / totalTasks * 100) : 0;

            // 2. Role Distribution
            var roles = await _context.Roles.Include(r => r.Users).ToListAsync();
            var roleCounts = roles.Select(r => new RoleUserCountDto
            {
                RoleName = r.RoleName,
                UserCount = r.Users.Count,
                Percentage = totalUsers > 0 ? Math.Round((double)r.Users.Count / totalUsers * 100, 1) : 0
            }).ToList();

            // 3. Department Distribution
            var deptCounts = await _context.Departments
                .Select(d => new DepartmentUserCountDto
                {
                    DepartmentName = d.Name,
                    UserCount = d.UserDepartments.Count
                })
                .ToListAsync();

            // 4. Project Performance & Health
            var projects = await _context.Projects
                .Include(p => p.ManagerUser)
                .Include(p => p.CreatedByUser)
                .Include(p => p.Tasks)
                .Where(p => !p.IsArchived)
                .OrderByDescending(p => p.CreatedAt)
                .Take(6)
                .ToListAsync();

            var projectPerformanceList = projects.Select(p =>
            {
                int pTotal = p.Tasks.Count;
                int pCompleted = p.Tasks.Count(t => t.Status == TaskStatusEnum.Completed);
                int pProgress = pTotal > 0 ? (int)Math.Round((double)pCompleted / pTotal * 100) : p.Progress;

                return new ProjectPerformanceDto
                {
                    Id = p.Id,
                    ProjectCode = string.IsNullOrWhiteSpace(p.ProjectCode) ? $"PRJ-{p.Id:D3}" : p.ProjectCode,
                    Title = p.Title,
                    Progress = pProgress,
                    TotalTasks = pTotal,
                    CompletedTasks = pCompleted,
                    Status = p.Status,
                    Priority = p.Priority,
                    ManagerName = p.ManagerUser?.FullName ?? p.CreatedByUser?.FullName ?? "Manager",
                    EndDate = p.EndDate,
                    IsOverdue = DateTime.Today > p.EndDate.Date && p.Status != ProjectStatus.Completed
                };
            }).ToList();

            // 5. Recent Audit Logs
            var recentAuditLogs = await _context.AuditLogs
                .Include(a => a.Project)
                .OrderByDescending(a => a.Timestamp)
                .Take(8)
                .Select(a => new AuditLogItemViewModel
                {
                    Id = a.Id,
                    UserId = a.UserId,
                    UserName = a.UserName,
                    ProjectId = a.ProjectId,
                    ProjectTitle = a.Project != null ? a.Project.Title : null,
                    Action = a.Action,
                    EntityName = a.EntityName,
                    Details = a.Details,
                    Timestamp = a.Timestamp
                })
                .ToListAsync();

            // 6. Recent Users
            var recentUsers = await _context.Users
                .Include(u => u.Role)
                .OrderByDescending(u => u.CreatedAt)
                .Take(5)
                .Select(u => new RecentUserDto
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    Email = u.Email,
                    RoleName = u.Role != null ? u.Role.RoleName : "N/A",
                    CreatedAt = u.CreatedAt
                })
                .ToListAsync();

            // 7. Active Announcements
            var today = DateTime.Today;
            var activeAnnouncements = await _context.Announcements
                .Include(a => a.CreatedByUser)
                .Where(a => a.IsActive && (!a.ExpiryDate.HasValue || a.ExpiryDate.Value.Date >= today))
                .OrderByDescending(a => a.IsPinned)
                .ThenByDescending(a => a.CreatedAt)
                .Take(5)
                .ToListAsync();

            return new AdminDashboardViewModel
            {
                TotalUsers = totalUsers,
                TotalDepartments = totalDepartments,
                TotalProjects = totalProjects,
                TotalTasks = totalTasks,
                CompletedTasks = completedTasks,
                OverdueTasks = overdueTasks,
                ActiveProjects = activeProjects,
                PlanningProjects = planningProjects,
                OnHoldProjects = onHoldProjects,
                OverallCompletionRate = overallCompletionRate,
                RoleUserCounts = roleCounts,
                DepartmentUserCounts = deptCounts,
                ProjectPerformanceList = projectPerformanceList,
                RecentAuditLogs = recentAuditLogs,
                RecentUsers = recentUsers,
                ActiveAnnouncements = activeAnnouncements
            };
        }
    }
}
