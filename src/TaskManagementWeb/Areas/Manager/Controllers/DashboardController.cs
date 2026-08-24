using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagementWeb.Data;
using System.Security.Claims;

namespace TaskManagementWeb.Areas.Manager.Controllers
{
    [Area("Manager")]
    [Authorize(Roles = "Manager,Admin")]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return (userIdClaim != null && int.TryParse(userIdClaim.Value, out int id)) ? id : 0;
        }

        // GET: Manager/Dashboard
        public async Task<IActionResult> Index()
        {
            int currentUserId = GetCurrentUserId();
            bool isAdmin = User.IsInRole("Admin");

            var projectQuery = _context.Projects.AsQueryable();
            var taskQuery = _context.TaskItems.AsQueryable();

            if (!isAdmin)
            {
                projectQuery = projectQuery.Where(p => p.CreatedByUserId == currentUserId || 
                    p.ProjectMembers.Any(pm => pm.UserId == currentUserId));

                taskQuery = taskQuery.Where(t => t.Project.CreatedByUserId == currentUserId || 
                    t.Project.ProjectMembers.Any(pm => pm.UserId == currentUserId));
            }

            ViewBag.TotalProjects = await projectQuery.CountAsync();
            ViewBag.TotalTasks = await taskQuery.CountAsync();
            ViewBag.CompletedTasks = await taskQuery.CountAsync(t => (int)t.Status == 2);
            ViewBag.TotalEmployees = await _context.Users.CountAsync(u => u.Role != null && u.Role.RoleName == "Employee");

            var recentProjects = await projectQuery
                .Include(p => p.CreatedByUser)
                .Include(p => p.ProjectMembers)
                .OrderByDescending(p => p.StartDate)
                .Take(5)
                .ToListAsync();

            return View(recentProjects);
        }
    }
}