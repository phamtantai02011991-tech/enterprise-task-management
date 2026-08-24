using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TaskManagementWeb.Data;
using TaskManagementWeb.Models.Entities;
using TaskManagementWeb.Models.Enums;
using TaskManagementWeb.Services.Admin;
using TaskManagementWeb.ViewModels.Manager;

namespace TaskManagementWeb.Services.Manager
{
    public class ProjectManagementService : IProjectManagementService
    {
        private readonly ApplicationDbContext _context;
        private readonly IAuditLogService _auditLogService;

        public ProjectManagementService(ApplicationDbContext context, IAuditLogService auditLogService)
        {
            _context = context;
            _auditLogService = auditLogService;
        }

        public async Task<List<Project>> GetAllProjectsAsync(ProjectStatus? status = null, bool showArchived = false, string? search = null, int? currentUserId = null, bool isAdmin = false)
        {
            var query = _context.Projects
                .Include(p => p.CreatedByUser)
                .Include(p => p.ManagerUser)
                .Include(p => p.ProjectMembers)
                    .ThenInclude(pm => pm.User)
                .Include(p => p.Tasks)
                .AsQueryable();

            // Lọc lưu trữ
            if (showArchived)
            {
                query = query.Where(p => p.IsArchived);
            }
            else
            {
                query = query.Where(p => !p.IsArchived);
            }

            // Lọc theo quyền (Manager chỉ xem dự án của mình nếu không phải Admin)
            if (!isAdmin && currentUserId.HasValue)
            {
                int uid = currentUserId.Value;
                query = query.Where(p => p.CreatedByUserId == uid || p.ManagerId == uid || p.ProjectMembers.Any(pm => pm.UserId == uid));
            }

            // Lọc theo trạng thái
            if (status.HasValue)
            {
                query = query.Where(p => p.Status == status.Value);
            }

            // Tìm kiếm
            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim().ToLower();
                query = query.Where(p => p.Title.ToLower().Contains(search) || 
                                         p.ProjectCode.ToLower().Contains(search) || 
                                         (p.Description != null && p.Description.ToLower().Contains(search)));
            }

            return await query.OrderByDescending(p => p.CreatedAt).ToListAsync();
        }

        public async Task<Project?> GetProjectByIdAsync(int id)
        {
            return await _context.Projects
                .Include(p => p.CreatedByUser)
                .Include(p => p.ManagerUser)
                .Include(p => p.ProjectMembers)
                    .ThenInclude(pm => pm.User)
                .Include(p => p.Tasks)
                    .ThenInclude(t => t.AssignedUser)
                .Include(p => p.ProjectFiles)
                    .ThenInclude(f => f.UploadedByUser)
                .Include(p => p.AuditLogs)
                    .ThenInclude(a => a.User)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<ProjectDetailsViewModel?> GetProjectDetailsViewModelAsync(int id)
        {
            var project = await GetProjectByIdAsync(id);
            if (project == null) return null;

            // Recalculate progress dynamically
            int calculatedProgress = await RecalculateProgressAsync(id);
            project.Progress = calculatedProgress;

            // Danh sách user chưa có trong dự án để thêm vào
            var existingMemberIds = project.ProjectMembers.Select(pm => pm.UserId).ToList();
            var availableUsers = await _context.Users
                .Where(u => !existingMemberIds.Contains(u.Id))
                .Select(u => new SelectListItem
                {
                    Value = u.Id.ToString(),
                    Text = $"{u.FullName} ({u.Email}) - {u.Role.RoleName}"
                })
                .ToListAsync();

            var auditLogs = await _context.AuditLogs
                .Include(a => a.User)
                .Where(a => a.ProjectId == id)
                .OrderByDescending(a => a.Timestamp)
                .Take(30)
                .ToListAsync();

            return new ProjectDetailsViewModel
            {
                Project = project,
                Tasks = project.Tasks.ToList(),
                ProjectFiles = project.ProjectFiles.ToList(),
                ProjectMembers = project.ProjectMembers.ToList(),
                AuditLogs = auditLogs,
                AvailableUsersToAdd = availableUsers
            };
        }

        public async Task<ProjectCreateUpdateViewModel> GetCreateViewModelAsync(int currentUserId)
        {
            // Tự sinh mã dự án gợi ý
            int projectCount = await _context.Projects.CountAsync() + 1;
            string suggestedCode = $"PRJ-{DateTime.Now.Year}-{projectCount:D3}";

            var managers = await _context.Users
                .Where(u => u.Role.RoleName == "Manager" || u.Role.RoleName == "Admin")
                .Select(u => new SelectListItem
                {
                    Value = u.Id.ToString(),
                    Text = $"{u.FullName} ({u.Role.RoleName})"
                })
                .ToListAsync();

            var employees = await _context.Users
                .Select(u => new SelectListItem
                {
                    Value = u.Id.ToString(),
                    Text = $"{u.FullName} ({u.Email}) - {u.Role.RoleName}"
                })
                .ToListAsync();

            return new ProjectCreateUpdateViewModel
            {
                ProjectCode = suggestedCode,
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddMonths(1),
                Status = ProjectStatus.Planning,
                Priority = ProjectPriority.Medium,
                ManagerId = currentUserId,
                CreatedByUserId = currentUserId,
                AvailableManagers = managers,
                AvailableEmployees = employees
            };
        }

        public async Task<ProjectCreateUpdateViewModel?> GetEditViewModelAsync(int id)
        {
            var project = await _context.Projects
                .Include(p => p.ProjectMembers)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (project == null) return null;

            var managers = await _context.Users
                .Where(u => u.Role.RoleName == "Manager" || u.Role.RoleName == "Admin")
                .Select(u => new SelectListItem
                {
                    Value = u.Id.ToString(),
                    Text = $"{u.FullName} ({u.Role.RoleName})"
                })
                .ToListAsync();

            var employees = await _context.Users
                .Select(u => new SelectListItem
                {
                    Value = u.Id.ToString(),
                    Text = $"{u.FullName} ({u.Email})"
                })
                .ToListAsync();

            return new ProjectCreateUpdateViewModel
            {
                Id = project.Id,
                ProjectCode = project.ProjectCode,
                Title = project.Title,
                Description = project.Description,
                StartDate = project.StartDate,
                EndDate = project.EndDate,
                Status = project.Status,
                Priority = project.Priority,
                ManagerId = project.ManagerId,
                Progress = project.Progress,
                CreatedByUserId = project.CreatedByUserId,
                SelectedMemberIds = project.ProjectMembers.Select(pm => pm.UserId).ToList(),
                AvailableManagers = managers,
                AvailableEmployees = employees
            };
        }

        public async Task<Project> CreateProjectAsync(ProjectCreateUpdateViewModel model, int creatorUserId, string creatorName)
        {
            var project = new Project
            {
                ProjectCode = model.ProjectCode.Trim().ToUpper(),
                Title = model.Title.Trim(),
                Description = model.Description,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                Status = model.Status,
                Priority = model.Priority,
                ManagerId = model.ManagerId ?? creatorUserId,
                Progress = model.Progress,
                CreatedByUserId = creatorUserId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsArchived = false
            };

            // Thêm các thành viên ban đầu nếu có chọn
            if (model.SelectedMemberIds != null && model.SelectedMemberIds.Any())
            {
                foreach (var userId in model.SelectedMemberIds.Distinct())
                {
                    project.ProjectMembers.Add(new ProjectMember
                    {
                        UserId = userId,
                        RoleInProject = (userId == project.ManagerId) ? "Project Manager" : "Member",
                        AssignedAt = DateTime.Now
                    });
                }
            }

            // Đảm bảo Manager có trong ProjectMembers
            if (project.ManagerId.HasValue && !project.ProjectMembers.Any(pm => pm.UserId == project.ManagerId.Value))
            {
                project.ProjectMembers.Add(new ProjectMember
                {
                    UserId = project.ManagerId.Value,
                    RoleInProject = "Project Manager",
                    AssignedAt = DateTime.Now
                });
            }

            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            await _auditLogService.LogActionAsync(
                creatorUserId,
                creatorName,
                project.Id,
                "CREATE",
                "Project",
                $"Khởi tạo dự án mới: [{project.ProjectCode}] - {project.Title}"
            );

            return project;
        }

        public async Task<bool> UpdateProjectAsync(ProjectCreateUpdateViewModel model, int editorUserId, string editorName)
        {
            var project = await _context.Projects
                .Include(p => p.ProjectMembers)
                .FirstOrDefaultAsync(p => p.Id == model.Id);

            if (project == null) return false;

            project.ProjectCode = model.ProjectCode.Trim().ToUpper();
            project.Title = model.Title.Trim();
            project.Description = model.Description;
            project.StartDate = model.StartDate;
            project.EndDate = model.EndDate;
            project.Status = model.Status;
            project.Priority = model.Priority;
            project.ManagerId = model.ManagerId;
            project.Progress = model.Progress;
            project.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            await _auditLogService.LogActionAsync(
                editorUserId,
                editorName,
                project.Id,
                "UPDATE",
                "Project",
                $"Cập nhật thông tin dự án [{project.ProjectCode}] - {project.Title}"
            );

            return true;
        }

        public async Task<bool> DeleteProjectAsync(int id, int userId, string userName)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null) return false;

            string info = $"[{project.ProjectCode}] {project.Title}";
            
            // Xóa mềm: Chuyển sang trạng thái Lưu trữ (Archived) để bảo toàn 100% dữ liệu Tasks, Files, TimeLogs
            project.IsArchived = true;
            project.ArchivedAt = DateTime.UtcNow;
            project.UpdatedAt = DateTime.UtcNow;
            _context.Projects.Update(project);
            await _context.SaveChangesAsync();

            await _auditLogService.LogActionAsync(
                userId,
                userName,
                id,
                "SOFT_DELETE",
                "Project",
                $"Đóng / Lưu trữ (Soft Delete) dự án ID {id} {info} - Bảo toàn toàn bộ công việc và dữ liệu lịch sử"
            );

            return true;
        }

        public async Task<bool> ArchiveProjectAsync(int id, int userId, string userName)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null) return false;

            project.IsArchived = true;
            project.ArchivedAt = DateTime.UtcNow;
            project.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            await _auditLogService.LogActionAsync(
                userId,
                userName,
                id,
                "ARCHIVE",
                "Project",
                $"Lưu trữ (đóng băng) dự án [{project.ProjectCode}] {project.Title}"
            );

            return true;
        }

        public async Task<bool> UnarchiveProjectAsync(int id, int userId, string userName)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null) return false;

            project.IsArchived = false;
            project.ArchivedAt = null;
            project.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            await _auditLogService.LogActionAsync(
                userId,
                userName,
                id,
                "UNARCHIVE",
                "Project",
                $"Phục hồi dự án từ kho lưu trữ: [{project.ProjectCode}] {project.Title}"
            );

            return true;
        }

        public async Task<bool> AddMemberAsync(int projectId, int userId, string role, int actionUserId, string actionUserName)
        {
            var exists = await _context.ProjectMembers.AnyAsync(pm => pm.ProjectId == projectId && pm.UserId == userId);
            if (exists) return false;

            var member = new ProjectMember
            {
                ProjectId = projectId,
                UserId = userId,
                RoleInProject = string.IsNullOrWhiteSpace(role) ? "Member" : role.Trim(),
                AssignedAt = DateTime.Now
            };

            _context.ProjectMembers.Add(member);
            await _context.SaveChangesAsync();

            var user = await _context.Users.FindAsync(userId);
            await _auditLogService.LogActionAsync(
                actionUserId,
                actionUserName,
                projectId,
                "ADD_MEMBER",
                "ProjectMember",
                $"Thêm thành viên {user?.FullName ?? userId.ToString()} (Vai trò: {member.RoleInProject}) vào dự án"
            );

            return true;
        }

        public async Task<bool> RemoveMemberAsync(int projectId, int userId, int actionUserId, string actionUserName)
        {
            var member = await _context.ProjectMembers
                .Include(pm => pm.User)
                .FirstOrDefaultAsync(pm => pm.ProjectId == projectId && pm.UserId == userId);

            if (member == null) return false;

            string memberName = member.User?.FullName ?? userId.ToString();
            _context.ProjectMembers.Remove(member);
            await _context.SaveChangesAsync();

            await _auditLogService.LogActionAsync(
                actionUserId,
                actionUserName,
                projectId,
                "REMOVE_MEMBER",
                "ProjectMember",
                $"Xóa thành viên {memberName} khỏi dự án"
            );

            return true;
        }

        public async Task<bool> AssignManagerAsync(int projectId, int managerId, int actionUserId, string actionUserName)
        {
            var project = await _context.Projects.FindAsync(projectId);
            if (project == null) return false;

            var newManager = await _context.Users.FindAsync(managerId);
            if (newManager == null) return false;

            project.ManagerId = managerId;
            project.UpdatedAt = DateTime.UtcNow;

            // Ensure is in members as Project Manager
            var member = await _context.ProjectMembers.FirstOrDefaultAsync(pm => pm.ProjectId == projectId && pm.UserId == managerId);
            if (member == null)
            {
                _context.ProjectMembers.Add(new ProjectMember
                {
                    ProjectId = projectId,
                    UserId = managerId,
                    RoleInProject = "Project Manager",
                    AssignedAt = DateTime.Now
                });
            }
            else
            {
                member.RoleInProject = "Project Manager";
            }

            await _context.SaveChangesAsync();

            await _auditLogService.LogActionAsync(
                actionUserId,
                actionUserName,
                projectId,
                "ASSIGN_MANAGER",
                "Project",
                $"Bổ nhiệm {newManager.FullName} làm Trưởng dự án (Project Manager)"
            );

            return true;
        }

        public async Task<bool> UploadFileAsync(int projectId, IFormFile file, int userId, string userName, string webRootPath)
        {
            if (file == null || file.Length == 0) return false;

            string uploadsFolder = Path.Combine(webRootPath, "uploads");
            if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

            string uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            var projectFile = new ProjectFile
            {
                ProjectId = projectId,
                FileName = file.FileName,
                FilePath = $"/uploads/{uniqueFileName}",
                FileSize = file.Length,
                UploadedByUserId = userId,
                UploadedAt = DateTime.Now
            };

            _context.ProjectFiles.Add(projectFile);
            await _context.SaveChangesAsync();

            await _auditLogService.LogActionAsync(
                userId,
                userName,
                projectId,
                "UPLOAD_FILE",
                "ProjectFile",
                $"Tải lên tài liệu: {file.FileName} ({(file.Length / 1024):N0} KB)"
            );

            return true;
        }

        public async Task<int> RecalculateProgressAsync(int projectId)
        {
            var project = await _context.Projects
                .Include(p => p.Tasks)
                .FirstOrDefaultAsync(p => p.Id == projectId);

            if (project == null) return 0;

            if (project.Tasks.Any())
            {
                int total = project.Tasks.Count;
                int completed = project.Tasks.Count(t => t.Status.ToString().Equals("Completed", StringComparison.OrdinalIgnoreCase));
                int progress = (int)Math.Round((double)completed / total * 100);

                if (project.Progress != progress)
                {
                    project.Progress = progress;
                    await _context.SaveChangesAsync();
                }
                return progress;
            }

            return project.Progress;
        }

        public async Task<bool> UpdateProjectPriorityAsync(int id, ProjectPriority priority, int userId, string userName)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null) return false;

            var oldPriority = project.Priority;
            project.Priority = priority;
            project.UpdatedAt = DateTime.UtcNow;

            _context.Projects.Update(project);
            await _context.SaveChangesAsync();

            await _auditLogService.LogActionAsync(
                userId,
                userName,
                id,
                "UPDATE_PRIORITY",
                "Project",
                $"Thay đổi mức độ ưu tiên dự án [{project.ProjectCode}] từ {oldPriority} sang {priority}"
            );

            return true;
        }
    }
}