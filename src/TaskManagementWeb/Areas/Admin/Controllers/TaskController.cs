using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagementWeb.Data;
using TaskManagementWeb.Models.Entities;
using TaskManagementWeb.Models.Enums;

namespace TaskManagementWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class TaskController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TaskController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Admin/Task (Giám sát toàn bộ công việc toàn doanh nghiệp)
        public async Task<IActionResult> Index(int? projectId, TaskStatusEnum? status, TaskPriority? priority, string? search, bool? overdue)
        {
            ViewData["ActivePage"] = "Tasks";
            ViewBag.SelectedProjectId = projectId;
            ViewBag.SelectedStatus = status;
            ViewBag.SelectedPriority = priority;
            ViewBag.SearchQuery = search;
            ViewBag.OverdueFilter = overdue;

            var query = _context.TaskItems
                .Include(t => t.Project)
                .Include(t => t.AssignedUser)
                .Include(t => t.TimeLogs)
                .AsQueryable();

            if (projectId.HasValue)
            {
                query = query.Where(t => t.ProjectId == projectId.Value);
            }

            if (status.HasValue)
            {
                query = query.Where(t => t.Status == status.Value);
            }

            if (priority.HasValue)
            {
                query = query.Where(t => t.Priority == priority.Value);
            }

            if (overdue == true)
            {
                var now = DateTime.Today;
                query = query.Where(t => t.Deadline < now && t.Status != TaskStatusEnum.Completed);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                string kw = search.Trim().ToLower();
                query = query.Where(t => t.Title.ToLower().Contains(kw) || 
                                         (t.Description != null && t.Description.ToLower().Contains(kw)) ||
                                         t.Project.Title.ToLower().Contains(kw) ||
                                         (t.AssignedUser != null && t.AssignedUser.FullName.ToLower().Contains(kw)));
            }

            var tasks = await query
                .OrderBy(t => t.Status == TaskStatusEnum.Completed)
                .ThenBy(t => t.Deadline)
                .ToListAsync();

            // Dropdown projects for filter
            ViewBag.Projects = await _context.Projects
                .Where(p => !p.IsArchived)
                .Select(p => new { p.Id, Title = $"[{p.ProjectCode}] {p.Title}" })
                .ToListAsync();

            // Metrics
            ViewBag.TotalCount = await _context.TaskItems.CountAsync();
            ViewBag.CompletedCount = await _context.TaskItems.CountAsync(t => t.Status == TaskStatusEnum.Completed);
            ViewBag.PendingCount = await _context.TaskItems.CountAsync(t => t.Status == TaskStatusEnum.Pending);
            ViewBag.InProgressCount = await _context.TaskItems.CountAsync(t => t.Status == TaskStatusEnum.InProgress);
            ViewBag.OverdueCount = await _context.TaskItems.CountAsync(t => t.Deadline < DateTime.Today && t.Status != TaskStatusEnum.Completed);

            return View(tasks);
        }

        // GET: /Admin/Task/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            ViewData["ActivePage"] = "Tasks";

            var taskItem = await _context.TaskItems
                .Include(t => t.Project)
                    .ThenInclude(p => p.ManagerUser)
                .Include(t => t.AssignedUser)
                .Include(t => t.TimeLogs)
                    .ThenInclude(tl => tl.User)
                .FirstOrDefaultAsync(t => t.Id == id.Value);

            if (taskItem == null) return NotFound();

            return View(taskItem);
        }
    }
}
