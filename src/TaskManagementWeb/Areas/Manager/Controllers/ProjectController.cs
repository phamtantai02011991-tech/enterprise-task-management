using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TaskManagementWeb.Data;
using TaskManagementWeb.Models.Entities;
using TaskManagementWeb.Models.Enums;
using TaskManagementWeb.Services.Manager;
using TaskManagementWeb.ViewModels.Manager;

namespace TaskManagementWeb.Areas.Manager.Controllers
{
    [Area("Manager")]
    [Authorize(Roles = "Manager,Admin")]
    public class ProjectController : Controller
    {
        private readonly IProjectManagementService _projectService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ApplicationDbContext _context;

        public ProjectController(
            IProjectManagementService projectService,
            IWebHostEnvironment webHostEnvironment,
            ApplicationDbContext context)
        {
            _projectService = projectService;
            _webHostEnvironment = webHostEnvironment;
            _context = context;
        }

        private (int UserId, string UserName, bool IsAdmin) GetCurrentUserInfo()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            int userId = (userIdClaim != null && int.TryParse(userIdClaim.Value, out int id)) ? id : 1;
            string userName = User.Identity?.Name ?? "Manager";
            bool isAdmin = User.IsInRole("Admin");
            return (userId, userName, isAdmin);
        }

        // GET: Manager/Project (Danh sách dự án với bộ lọc Status, Archive, Search)
        public async Task<IActionResult> Index(ProjectStatus? status = null, bool showArchived = false, string? search = null)
        {
            var (userId, _, isAdmin) = GetCurrentUserInfo();
            var projects = await _projectService.GetAllProjectsAsync(status, showArchived, search, userId, isAdmin);

            ViewBag.CurrentStatus = status;
            ViewBag.ShowArchived = showArchived;
            ViewBag.SearchQuery = search;

            return View(projects);
        }

        // GET: Manager/Project/Details/5 (Xem chi tiết dự án)
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var viewModel = await _projectService.GetProjectDetailsViewModelAsync(id.Value);
            if (viewModel == null) return NotFound();

            return View(viewModel);
        }

        // GET: Manager/Project/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var (userId, _, _) = GetCurrentUserInfo();
            var model = await _projectService.GetCreateViewModelAsync(userId);
            return View(model);
        }

        // POST: Manager/Project/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProjectCreateUpdateViewModel model)
        {
            var (userId, userName, _) = GetCurrentUserInfo();

            if (ModelState.IsValid)
            {
                try
                {
                    var project = await _projectService.CreateProjectAsync(model, userId, userName);
                    TempData["SuccessMessage"] = $"Khởi tạo dự án [{project.ProjectCode}] - {project.Title} thành công!";
                    return RedirectToAction(nameof(Details), new { id = project.Id });
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Lỗi khi lưu dữ liệu: {ex.Message}";
                }
            }

            // Reload dropdowns on error
            var reloadModel = await _projectService.GetCreateViewModelAsync(userId);
            model.AvailableManagers = reloadModel.AvailableManagers;
            model.AvailableEmployees = reloadModel.AvailableEmployees;

            return View(model);
        }

        // GET: Manager/Project/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var model = await _projectService.GetEditViewModelAsync(id.Value);
            if (model == null) return NotFound();

            return View(model);
        }

        // POST: Manager/Project/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProjectCreateUpdateViewModel model)
        {
            if (id != model.Id) return NotFound();
            var (userId, userName, _) = GetCurrentUserInfo();

            if (ModelState.IsValid)
            {
                var success = await _projectService.UpdateProjectAsync(model, userId, userName);
                if (success)
                {
                    TempData["SuccessMessage"] = $"Cập nhật dự án [{model.ProjectCode}] thành công.";
                    return RedirectToAction(nameof(Details), new { id = model.Id });
                }
                else
                {
                    TempData["ErrorMessage"] = "Không tìm thấy dự án để cập nhật.";
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

        // POST: Manager/Project/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var (userId, userName, _) = GetCurrentUserInfo();
            var success = await _projectService.DeleteProjectAsync(id, userId, userName);

            if (success)
            {
                TempData["SuccessMessage"] = "Đã xóa dự án thành công.";
            }
            else
            {
                TempData["ErrorMessage"] = "Không thể xóa dự án này.";
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Manager/Project/Archive/5 (Lưu trữ / Đóng băng dự án)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Archive(int id)
        {
            var (userId, userName, _) = GetCurrentUserInfo();
            var success = await _projectService.ArchiveProjectAsync(id, userId, userName);

            if (success)
            {
                TempData["SuccessMessage"] = "Dự án đã được chuyển vào mục lưu trữ (Archive).";
            }
            else
            {
                TempData["ErrorMessage"] = "Không thể lưu trữ dự án này.";
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        // POST: Manager/Project/Unarchive/5 (Phục hồi dự án)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Unarchive(int id)
        {
            var (userId, userName, _) = GetCurrentUserInfo();
            var success = await _projectService.UnarchiveProjectAsync(id, userId, userName);

            if (success)
            {
                TempData["SuccessMessage"] = "Dự án đã được mở khóa và phục hồi thành công.";
            }
            else
            {
                TempData["ErrorMessage"] = "Không thể phục hồi dự án này.";
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        // POST: Manager/Project/AddMember
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddMember(int projectId, int userId, string role)
        {
            var (currentUserId, currentUserName, _) = GetCurrentUserInfo();
            var success = await _projectService.AddMemberAsync(projectId, userId, role, currentUserId, currentUserName);

            if (success)
            {
                TempData["SuccessMessage"] = "Đã thêm thành viên mới vào dự án thành công.";
            }
            else
            {
                TempData["ErrorMessage"] = "Thành viên này đã tồn tại trong dự án hoặc không hợp lệ.";
            }

            return RedirectToAction(nameof(Details), new { id = projectId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReportMember(int projectId, int targetUserId, MemberReportLevel level, string reason)
        {
            var (currentUserId, currentUserName, _) = GetCurrentUserInfo();

            if (string.IsNullOrWhiteSpace(reason))
            {
                TempData["ErrorMessage"] = "Vui lòng nhập lý do báo cáo cụ thể.";
                return RedirectToAction(nameof(Details), new { id = projectId });
            }

            var report = new ProjectMemberReport
            {
                ProjectId = projectId,
                ReporterManagerId = currentUserId,
                TargetUserId = targetUserId,
                Level = level,
                Reason = reason.Trim(),
                Status = MemberReportStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            _context.ProjectMemberReports.Add(report);
            await _context.SaveChangesAsync();

            // Gửi thông báo đến Admin
            var admins = await _context.Users.Where(u => u.RoleId == 1).ToListAsync();
            var targetUser = await _context.Users.FindAsync(targetUserId);
            var project = await _context.Projects.FindAsync(projectId);
            string levelText = level == MemberReportLevel.Expulsion ? "Đề nghị khai trừ khỏi dự án" : "Báo cáo khiển trách";

            foreach (var admin in admins)
            {
                _context.Notifications.Add(new Notification
                {
                    UserId = admin.Id,
                    Title = $"Báo cáo nhân sự mới: {levelText}",
                    Message = $"Manager {currentUserName} vừa gửi {levelText} đối với nhân sự {targetUser?.FullName} trong dự án [{project?.ProjectCode}].",
                    CreatedAt = DateTime.UtcNow,
                    IsRead = false
                });
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Đã gửi {levelText} đối với nhân sự [{targetUser?.FullName}] lên Admin để xem xét và phê duyệt.";
            return RedirectToAction(nameof(Details), new { id = projectId });
        }

        // GET: /Manager/Project/Kanban (Thẻ kéo thả mức độ ưu tiên dự án)
        public async Task<IActionResult> Kanban(string? search = null)
        {
            ViewData["ActivePage"] = "ProjectsKanban";
            ViewBag.SearchQuery = search;
            var (userId, _, isAdmin) = GetCurrentUserInfo();
            var projects = await _projectService.GetAllProjectsAsync(null, false, search, userId, isAdmin);
            return View(projects);
        }

        // POST: /Manager/Project/UpdatePriority (AJAX Drag & Drop)
        [HttpPost]
        public async Task<IActionResult> UpdatePriority(int id, ProjectPriority priority)
        {
            var (userId, userName, _) = GetCurrentUserInfo();
            var success = await _projectService.UpdateProjectPriorityAsync(id, priority, userId, userName);
            if (!success) return BadRequest(new { success = false, message = "Không tìm thấy dự án." });
            return Json(new { success = true, message = $"Đã cập nhật mức ưu tiên sang [{priority}]." });
        }

        // POST: Manager/Project/AssignManager
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignManager(int projectId, int managerId)
        {
            var (currentUserId, currentUserName, _) = GetCurrentUserInfo();
            var success = await _projectService.AssignManagerAsync(projectId, managerId, currentUserId, currentUserName);

            if (success)
            {
                TempData["SuccessMessage"] = "Đã chỉ định Trưởng dự án (Project Manager) thành công.";
            }
            else
            {
                TempData["ErrorMessage"] = "Không thể phân công trưởng dự án.";
            }

            return RedirectToAction(nameof(Details), new { id = projectId });
        }

        // POST: Manager/Project/UploadFile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadFile(int projectId, IFormFile file)
        {
            var (userId, userName, _) = GetCurrentUserInfo();

            if (file != null && file.Length > 0)
            {
                var success = await _projectService.UploadFileAsync(projectId, file, userId, userName, _webHostEnvironment.WebRootPath);
                if (success)
                {
                    TempData["SuccessMessage"] = $"Tải lên tài liệu [{file.FileName}] thành công.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Tải tài liệu thất bại.";
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Vui lòng chọn một tập tin hợp lệ.";
            }

            return RedirectToAction(nameof(Details), new { id = projectId });
        }
    }
}
