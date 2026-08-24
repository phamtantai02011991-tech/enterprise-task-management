using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TaskManagementWeb.Data;
using TaskManagementWeb.Models.Entities;
using TaskManagementWeb.Services.Admin;
using System.Security.Claims;

namespace TaskManagementWeb.Areas.Manager.Controllers
{
    [Area("Manager")]
    [Authorize(Roles = "Manager,Admin")]
    public class TaskController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IAuditLogService _auditLogService;
        private readonly TaskManagementWeb.Services.Common.INotificationService _notificationService;

        public TaskController(
            ApplicationDbContext context, 
            IAuditLogService auditLogService,
            TaskManagementWeb.Services.Common.INotificationService notificationService)
        {
            _context = context;
            _auditLogService = auditLogService;
            _notificationService = notificationService;
        }

        private (int UserId, string UserName) GetCurrentUserInfo()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            int userId = (userIdClaim != null && int.TryParse(userIdClaim.Value, out int id)) ? id : 0;
            string userName = User.Identity?.Name ?? "Manager";
            return (userId, userName);
        }

        private async Task<bool> CanUserAccessProjectAsync(int projectId)
        {
            if (User.IsInRole("Admin")) return true;
            var (userId, _) = GetCurrentUserInfo();
            return await _context.Projects.AnyAsync(p => p.Id == projectId && 
                (p.CreatedByUserId == userId || p.ProjectMembers.Any(pm => pm.UserId == userId)));
        }

        // GET: Manager/Task/Index/5
        public async Task<IActionResult> Index(int? projectId)
        {
            var (userId, _) = GetCurrentUserInfo();
            var query = _context.TaskItems
                .Include(t => t.Project)
                .Include(t => t.AssignedUser)
                .AsQueryable();

            if (!User.IsInRole("Admin"))
            {
                query = query.Where(t => t.Project.CreatedByUserId == userId || 
                    t.Project.ProjectMembers.Any(pm => pm.UserId == userId));
            }

            if (projectId.HasValue)
            {
                if (!await CanUserAccessProjectAsync(projectId.Value))
                {
                    TempData["ErrorMessage"] = "Bạn không có quyền xem công việc của dự án này.";
                    return RedirectToAction(nameof(Index), new { projectId = (int?)null });
                }
                query = query.Where(t => t.ProjectId == projectId.Value);
                ViewBag.ProjectId = projectId.Value;
            }

            var tasks = await query.ToListAsync();
            return View(tasks);
        }

        // GET: Manager/Task/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var taskItem = await _context.TaskItems
                .Include(t => t.Project)
                .Include(t => t.AssignedUser)
                .Include(t => t.TimeLogs)
                    .ThenInclude(tl => tl.User)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (taskItem == null) return NotFound();

            if (!await CanUserAccessProjectAsync(taskItem.ProjectId))
            {
                TempData["ErrorMessage"] = "Bạn không có quyền truy cập vào công việc này.";
                return RedirectToAction(nameof(Index), new { projectId = (int?)null });
            }

            return View(taskItem);
        }

        // GET: Manager/Task/Create?projectId=5
        public async Task<IActionResult> Create(int projectId)
        {
            if (!await CanUserAccessProjectAsync(projectId))
            {
                TempData["ErrorMessage"] = "Bạn không có quyền tạo công việc cho dự án này.";
                return RedirectToAction(nameof(Index), new { projectId = (int?)null });
            }

            var project = await _context.Projects.FindAsync(projectId);
            if (project == null) return NotFound();

            ViewBag.ProjectId = projectId;
            ViewBag.ProjectTitle = project.Title;
            ViewBag.ProjectStartDate = project.StartDate.ToString("yyyy-MM-ddTHH:mm");
            ViewBag.ProjectEndDate = project.EndDate.ToString("yyyy-MM-ddTHH:mm");

            var projectMembers = await _context.ProjectMembers
                .Where(pm => pm.ProjectId == projectId)
                .Include(pm => pm.User)
                .Where(pm => pm.User.IsActive)
                .Select(pm => new { pm.UserId, pm.User.FullName })
                .ToListAsync();

            ViewBag.AssignedUserId = new SelectList(projectMembers, "UserId", "FullName");

            return View();
        }

        // POST: Manager/Task/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Description,Priority,Status,Deadline,ProjectId,AssignedUserId")] TaskItem taskItem)
        {
            if (!await CanUserAccessProjectAsync(taskItem.ProjectId))
            {
                TempData["ErrorMessage"] = "Bạn không có quyền tạo công việc cho dự án này.";
                return RedirectToAction(nameof(Index), new { projectId = (int?)null });
            }

            var project = await _context.Projects.FindAsync(taskItem.ProjectId);
            if (project == null || project.IsArchived)
            {
                ModelState.AddModelError("", "Dự án không tồn tại hoặc đã bị lưu trữ.");
            }
            else
            {
                if (taskItem.Deadline.Date > project.EndDate.Date)
                {
                    ModelState.AddModelError("Deadline", $"Hạn hoàn thành ({taskItem.Deadline:dd/MM/yyyy}) không được vượt quá ngày kết thúc của Dự án ({project.EndDate:dd/MM/yyyy}).");
                }
                if (taskItem.Deadline.Date < project.StartDate.Date)
                {
                    ModelState.AddModelError("Deadline", $"Hạn hoàn thành ({taskItem.Deadline:dd/MM/yyyy}) không được trước ngày bắt đầu của Dự án ({project.StartDate:dd/MM/yyyy}).");
                }
            }

            if (ModelState.IsValid)
            {
                _context.Add(taskItem);
                await _context.SaveChangesAsync();

                var (userId, userName) = GetCurrentUserInfo();
                await _auditLogService.LogActionAsync(
                    userId,
                    userName,
                    taskItem.ProjectId,
                    "CREATE",
                    "TaskItem",
                    $"Tạo công việc mới [{taskItem.Title}] thuộc dự án ID {taskItem.ProjectId}"
                );

                if (taskItem.AssignedUserId.HasValue)
                {
                    await _notificationService.NotifyTaskAssignedAsync(
                        taskItem.Id,
                        taskItem.AssignedUserId.Value,
                        taskItem.Title,
                        project?.Title ?? "Dự án",
                        userName
                    );
                }

                TempData["SuccessMessage"] = $"Tạo công việc [{taskItem.Title}] thành công.";
                return RedirectToAction(nameof(Index), new { projectId = taskItem.ProjectId });
            }

            var projectMembers = await _context.ProjectMembers
                .Where(pm => pm.ProjectId == taskItem.ProjectId)
                .Include(pm => pm.User)
                .Select(pm => new { pm.UserId, pm.User.FullName })
                .ToListAsync();

            ViewBag.AssignedUserId = new SelectList(projectMembers, "UserId", "FullName", taskItem.AssignedUserId);
            ViewBag.ProjectId = taskItem.ProjectId;
            return View(taskItem);
        }

        // GET: Manager/Task/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var taskItem = await _context.TaskItems.FindAsync(id);
            if (taskItem == null) return NotFound();

            if (!await CanUserAccessProjectAsync(taskItem.ProjectId))
            {
                TempData["ErrorMessage"] = "Bạn không có quyền chỉnh sửa công việc này.";
                return RedirectToAction(nameof(Index), new { projectId = (int?)null });
            }

            var projectMembers = await _context.ProjectMembers
                .Where(pm => pm.ProjectId == taskItem.ProjectId)
                .Include(pm => pm.User)
                .Select(pm => new { pm.UserId, pm.User.FullName })
                .ToListAsync();

            ViewBag.AssignedUserId = new SelectList(projectMembers, "UserId", "FullName", taskItem.AssignedUserId);
            return View(taskItem);
        }

        // POST: Manager/Task/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,Priority,Status,Deadline,ProjectId,AssignedUserId")] TaskItem taskItem)
        {
            if (id != taskItem.Id) return NotFound();

            if (!await CanUserAccessProjectAsync(taskItem.ProjectId))
            {
                TempData["ErrorMessage"] = "Bạn không có quyền chỉnh sửa công việc này.";
                return RedirectToAction(nameof(Index), new { projectId = (int?)null });
            }

            var project = await _context.Projects.FindAsync(taskItem.ProjectId);
            if (project == null || project.IsArchived)
            {
                ModelState.AddModelError("", "Dự án không tồn tại hoặc đã bị lưu trữ.");
            }
            else
            {
                if (taskItem.Deadline.Date > project.EndDate.Date)
                {
                    ModelState.AddModelError("Deadline", $"Hạn hoàn thành ({taskItem.Deadline:dd/MM/yyyy}) không được vượt quá ngày kết thúc của Dự án ({project.EndDate:dd/MM/yyyy}).");
                }
                if (taskItem.Deadline.Date < project.StartDate.Date)
                {
                    ModelState.AddModelError("Deadline", $"Hạn hoàn thành ({taskItem.Deadline:dd/MM/yyyy}) không được trước ngày bắt đầu của Dự án ({project.StartDate:dd/MM/yyyy}).");
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(taskItem);
                    await _context.SaveChangesAsync();

                    var (userId, userName) = GetCurrentUserInfo();
                    await _auditLogService.LogActionAsync(
                        userId,
                        userName,
                        taskItem.ProjectId,
                        "UPDATE",
                        "TaskItem",
                        $"Cập nhật công việc [{taskItem.Title}] ID {taskItem.Id}"
                    );

                    TempData["SuccessMessage"] = $"Cập nhật công việc [{taskItem.Title}] thành công.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.TaskItems.Any(e => e.Id == taskItem.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index), new { projectId = taskItem.ProjectId });
            }

            var projectMembers = await _context.ProjectMembers
                .Where(pm => pm.ProjectId == taskItem.ProjectId)
                .Include(pm => pm.User)
                .Select(pm => new { pm.UserId, pm.User.FullName })
                .ToListAsync();

            ViewBag.AssignedUserId = new SelectList(projectMembers, "UserId", "FullName", taskItem.AssignedUserId);
            return View(taskItem);
        }

        // GET: Manager/Task/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var taskItem = await _context.TaskItems
                .Include(t => t.Project)
                .Include(t => t.AssignedUser)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (taskItem == null) return NotFound();

            if (!await CanUserAccessProjectAsync(taskItem.ProjectId))
            {
                TempData["ErrorMessage"] = "Bạn không có quyền xóa công việc này.";
                return RedirectToAction(nameof(Index), new { projectId = (int?)null });
            }

            return View(taskItem);
        }

        // POST: Manager/Task/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var taskItem = await _context.TaskItems.FindAsync(id);
            int projectId = taskItem != null ? taskItem.ProjectId : 0;

            if (taskItem != null)
            {
                if (!await CanUserAccessProjectAsync(taskItem.ProjectId))
                {
                    TempData["ErrorMessage"] = "Bạn không có quyền xóa công việc này.";
                    return RedirectToAction(nameof(Index), new { projectId = (int?)null });
                }

                string title = taskItem.Title;
                _context.TaskItems.Remove(taskItem);
                await _context.SaveChangesAsync();

                var (userId, userName) = GetCurrentUserInfo();
                await _auditLogService.LogActionAsync(
                    userId,
                    userName,
                    projectId,
                    "DELETE",
                    "TaskItem",
                    $"Xóa công việc ID {id} [{title}]"
                );

                TempData["SuccessMessage"] = $"Đã xóa công việc [{title}].";
            }

            return RedirectToAction(nameof(Index), new { projectId = projectId });
        }
    }
}