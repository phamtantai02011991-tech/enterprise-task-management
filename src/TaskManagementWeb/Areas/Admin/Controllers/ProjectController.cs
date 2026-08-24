using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagementWeb.Data;
using TaskManagementWeb.Models.Entities;
using TaskManagementWeb.Models.Enums;
using TaskManagementWeb.Services.Admin;
using TaskManagementWeb.Services.Common;
using TaskManagementWeb.Services.Manager;
using TaskManagementWeb.ViewModels.Manager;

namespace TaskManagementWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ProjectController : Controller
    {
        private readonly IProjectManagementService _projectService;
        private readonly IAuditLogService _auditLogService;
        private readonly INotificationService _notificationService;
        private readonly ApplicationDbContext _context;

        public ProjectController(
            IProjectManagementService projectService,
            IAuditLogService auditLogService,
            INotificationService notificationService,
            ApplicationDbContext context)
        {
            _projectService = projectService;
            _auditLogService = auditLogService;
            _notificationService = notificationService;
            _context = context;
        }

        private (int UserId, string UserName) GetCurrentAdminInfo()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            int userId = (userIdClaim != null && int.TryParse(userIdClaim.Value, out int id)) ? id : 1;
            string userName = User.Identity?.Name ?? "Administrator";
            return (userId, userName);
        }

        // GET: /Admin/Project (Danh sách dự án toàn công ty với các tab: all, active, completed, archived)
        public async Task<IActionResult> Index(string filter = "all", string? search = null)
        {
            ViewData["ActivePage"] = "Projects";
            ViewBag.CurrentFilter = filter.ToLower();
            ViewBag.SearchQuery = search;

            ProjectStatus? statusFilter = null;
            bool showArchived = false;

            switch (filter.ToLower())
            {
                case "active":
                    statusFilter = ProjectStatus.Active;
                    break;
                case "completed":
                    statusFilter = ProjectStatus.Completed;
                    break;
                case "planning":
                    statusFilter = ProjectStatus.Planning;
                    break;
                case "onhold":
                    statusFilter = ProjectStatus.OnHold;
                    break;
                case "archived":
                    showArchived = true;
                    break;
                default:
                    // "all"
                    break;
            }

            var projects = await _projectService.GetAllProjectsAsync(statusFilter, showArchived, search, null, isAdmin: true);
            
            // Statistics for header badges
            ViewBag.CountAll = await _context.Projects.CountAsync(p => !p.IsArchived);
            ViewBag.CountPlanning = await _context.Projects.CountAsync(p => !p.IsArchived && p.Status == ProjectStatus.Planning);
            ViewBag.CountActive = await _context.Projects.CountAsync(p => !p.IsArchived && p.Status == ProjectStatus.Active);
            ViewBag.CountOnHold = await _context.Projects.CountAsync(p => !p.IsArchived && p.Status == ProjectStatus.OnHold);
            ViewBag.CountCompleted = await _context.Projects.CountAsync(p => !p.IsArchived && p.Status == ProjectStatus.Completed);
            ViewBag.CountArchived = await _context.Projects.CountAsync(p => p.IsArchived);

            return View(projects);
        }

        // GET: /Admin/Project/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewData["ActivePage"] = "Projects";
            var (adminId, _) = GetCurrentAdminInfo();
            var model = await _projectService.GetCreateViewModelAsync(adminId);
            return View(model);
        }

        // POST: /Admin/Project/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProjectCreateUpdateViewModel model)
        {
            ViewData["ActivePage"] = "Projects";
            var (adminId, adminName) = GetCurrentAdminInfo();

            if (ModelState.IsValid)
            {
                try
                {
                    var project = await _projectService.CreateProjectAsync(model, adminId, adminName);
                    TempData["SuccessMessage"] = $"Đã khởi tạo dự án [{project.ProjectCode}] - {project.Title} thành công!";
                    return RedirectToAction(nameof(Details), new { id = project.Id });
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Lỗi khi lưu dữ liệu: {ex.Message}";
                }
            }

            var reloadModel = await _projectService.GetCreateViewModelAsync(adminId);
            model.AvailableManagers = reloadModel.AvailableManagers;
            model.AvailableEmployees = reloadModel.AvailableEmployees;

            return View(model);
        }

        // GET: /Admin/Project/Details/5 (Trang quản trị toàn diện 7 phân hệ)
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            ViewData["ActivePage"] = "Projects";

            var viewModel = await _projectService.GetProjectDetailsViewModelAsync(id.Value);
            if (viewModel == null) return NotFound();

            return View(viewModel);
        }

        // GET: /Admin/Project/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            ViewData["ActivePage"] = "Projects";

            var model = await _projectService.GetEditViewModelAsync(id.Value);
            if (model == null) return NotFound();

            return View(model);
        }

        // POST: /Admin/Project/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProjectCreateUpdateViewModel model)
        {
            if (id != model.Id) return NotFound();
            ViewData["ActivePage"] = "Projects";
            var (adminId, adminName) = GetCurrentAdminInfo();

            if (ModelState.IsValid)
            {
                var success = await _projectService.UpdateProjectAsync(model, adminId, adminName);
                if (success)
                {
                    TempData["SuccessMessage"] = $"Cập nhật dự án [{model.ProjectCode}] thành công.";
                    return RedirectToAction(nameof(Details), new { id = model.Id });
                }
            }

            var reloadModel = await _projectService.GetEditViewModelAsync(id);
            if (reloadModel != null)
            {
                model.AvailableManagers = reloadModel.AvailableManagers;
                model.AvailableEmployees = reloadModel.AvailableEmployees;
            }

            return View(model);
        }

        // POST: /Admin/Project/ChangeStatus (Admin chuyển trạng thái vòng đời dự án)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeStatus(int projectId, ProjectStatus status)
        {
            var project = await _context.Projects.FindAsync(projectId);
            if (project != null)
            {
                var oldStatus = project.Status;
                project.Status = status;
                project.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                var (adminId, adminName) = GetCurrentAdminInfo();
                await _auditLogService.LogActionAsync(
                    adminId,
                    adminName,
                    projectId,
                    "CHANGE_STATUS",
                    "Project",
                    $"Admin chuyển trạng thái dự án [{project.ProjectCode}] từ {oldStatus} sang {status}"
                );

                TempData["SuccessMessage"] = $"Đã cập nhật trạng thái dự án sang [{status}].";
            }

            return RedirectToAction(nameof(Details), new { id = projectId });
        }

        // POST: /Admin/Project/AssignManager (Admin chỉ định / đổi Trưởng dự án)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignManager(int projectId, int managerId)
        {
            var (adminId, adminName) = GetCurrentAdminInfo();
            var success = await _projectService.AssignManagerAsync(projectId, managerId, adminId, adminName);

            if (success)
            {
                TempData["SuccessMessage"] = "Admin đã bổ nhiệm Trưởng dự án mới thành công.";
            }
            else
            {
                TempData["ErrorMessage"] = "Không thể phân công Trưởng dự án.";
            }

            return RedirectToAction(nameof(Details), new { id = projectId });
        }

        // POST: /Admin/Project/AddMember
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddMember(int projectId, int userId, string role)
        {
            var (adminId, adminName) = GetCurrentAdminInfo();
            var success = await _projectService.AddMemberAsync(projectId, userId, role, adminId, adminName);

            if (success)
            {
                TempData["SuccessMessage"] = "Đã phân công thành viên mới vào dự án.";
            }
            else
            {
                TempData["ErrorMessage"] = "Nhân sự này đã tồn tại trong dự án.";
            }

            return RedirectToAction(nameof(Details), new { id = projectId });
        }

        // POST: /Admin/Project/RemoveMember
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveMember(int projectId, int userId)
        {
            var (adminId, adminName) = GetCurrentAdminInfo();
            var success = await _projectService.RemoveMemberAsync(projectId, userId, adminId, adminName);

            if (success)
            {
                TempData["SuccessMessage"] = "Đã rút nhân sự khỏi dự án.";
            }
            return RedirectToAction(nameof(Details), new { id = projectId });
        }

        // POST: /Admin/Project/Archive/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Archive(int id)
        {
            var (adminId, adminName) = GetCurrentAdminInfo();
            await _projectService.ArchiveProjectAsync(id, adminId, adminName);
            TempData["SuccessMessage"] = "Dự án đã được chuyển vào kho lưu trữ đóng băng (Archived).";
            return RedirectToAction(nameof(Details), new { id });
        }

        // POST: /Admin/Project/Unarchive/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Unarchive(int id)
        {
            var (adminId, adminName) = GetCurrentAdminInfo();
            await _projectService.UnarchiveProjectAsync(id, adminId, adminName);
            TempData["SuccessMessage"] = "Dự án đã được mở khóa và phục hồi.";
            return RedirectToAction(nameof(Details), new { id });
        }

        // POST: /Admin/Project/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var (adminId, adminName) = GetCurrentAdminInfo();
            await _projectService.DeleteProjectAsync(id, adminId, adminName);
            TempData["SuccessMessage"] = "Admin đã xóa dự án thành công.";
            return RedirectToAction(nameof(Index));
        }

        // GET: /Admin/Project/Kanban (Thẻ kéo thả theo mức độ ưu tiên)
        public async Task<IActionResult> Kanban(string? search = null)
        {
            ViewData["ActivePage"] = "ProjectsKanban";
            ViewBag.SearchQuery = search;
            var projects = await _projectService.GetAllProjectsAsync(null, false, search, null, true);
            return View(projects);
        }

        // POST: /Admin/Project/UpdatePriority (AJAX Drag & Drop)
        [HttpPost]
        public async Task<IActionResult> UpdatePriority(int id, ProjectPriority priority)
        {
            var (adminId, adminName) = GetCurrentAdminInfo();
            var success = await _projectService.UpdateProjectPriorityAsync(id, priority, adminId, adminName);
            if (!success) return BadRequest(new { success = false, message = "Không tìm thấy dự án." });
            return Json(new { success = true, message = $"Đã chuyển ưu tiên sang [{priority}]." });
        }

        // GET: /Admin/Project/Reports (Báo cáo & Phân tích tổng thể tiến độ các dự án)
        public async Task<IActionResult> Reports()
        {
            ViewData["ActivePage"] = "Projects";

            var projects = await _context.Projects
                .Include(p => p.ManagerUser)
                .Include(p => p.CreatedByUser)
                .Include(p => p.ProjectMembers)
                .Include(p => p.Tasks)
                    .ThenInclude(t => t.TimeLogs)
                .Where(p => !p.IsArchived)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            return View(projects);
        }
    }
}
