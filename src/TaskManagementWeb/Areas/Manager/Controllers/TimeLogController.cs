using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagementWeb.Data;
using System.Security.Claims;

namespace TaskManagementWeb.Areas.Manager.Controllers
{
    [Area("Manager")]
    [Authorize(Roles = "Manager,Admin")]
    public class TimeLogController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TimeLogController(ApplicationDbContext context)
        {
            _context = context;
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return (userIdClaim != null && int.TryParse(userIdClaim.Value, out int id)) ? id : 0;
        }

        // GET: Manager/TimeLog
        public async Task<IActionResult> Index(int? projectId)
        {
            int currentUserId = GetCurrentUserId();

            var query = _context.TimeLogs
                .Include(tl => tl.User)
                .Include(tl => tl.TaskItem)
                    .ThenInclude(t => t.Project)
                .AsQueryable();

            if (!User.IsInRole("Admin"))
            {
                query = query.Where(tl => tl.TaskItem.Project.CreatedByUserId == currentUserId || 
                    tl.TaskItem.Project.ProjectMembers.Any(pm => pm.UserId == currentUserId));
            }

            if (projectId.HasValue)
            {
                query = query.Where(tl => tl.TaskItem.ProjectId == projectId.Value);
                ViewBag.ProjectId = projectId.Value;
            }

            var timeLogs = await query
                .OrderByDescending(tl => tl.DateLogged)
                .ToListAsync();

            ViewBag.TotalHours = timeLogs.Sum(tl => tl.HoursSpent);

            return View(timeLogs);
        }

        // GET: Manager/TimeLog/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            int currentUserId = GetCurrentUserId();

            var timeLog = await _context.TimeLogs
                .Include(tl => tl.User)
                .Include(tl => tl.TaskItem)
                    .ThenInclude(t => t.Project)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (timeLog == null) return NotFound();

            if (!User.IsInRole("Admin"))
            {
                bool isAuthorized = timeLog.TaskItem != null && timeLog.TaskItem.Project != null &&
                    (timeLog.TaskItem.Project.CreatedByUserId == currentUserId || 
                     timeLog.TaskItem.Project.ProjectMembers.Any(pm => pm.UserId == currentUserId));

                if (!isAuthorized)
                {
                    TempData["ErrorMessage"] = "Bạn không có quyền xem thông tin nhật ký làm việc này.";
                    return RedirectToAction(nameof(Index));
                }
            }

            return View(timeLog);
        }
    }
}