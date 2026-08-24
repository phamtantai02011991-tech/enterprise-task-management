using Microsoft.AspNetCore.Http;
using TaskManagementWeb.Models.Entities;
using TaskManagementWeb.Models.Enums;
using TaskManagementWeb.ViewModels.Manager;

namespace TaskManagementWeb.Services.Manager
{
    public interface IProjectManagementService
    {
        Task<List<Project>> GetAllProjectsAsync(ProjectStatus? status = null, bool showArchived = false, string? search = null, int? currentUserId = null, bool isAdmin = false);
        Task<Project?> GetProjectByIdAsync(int id);
        Task<ProjectDetailsViewModel?> GetProjectDetailsViewModelAsync(int id);
        Task<ProjectCreateUpdateViewModel> GetCreateViewModelAsync(int currentUserId);
        Task<ProjectCreateUpdateViewModel?> GetEditViewModelAsync(int id);
        Task<Project> CreateProjectAsync(ProjectCreateUpdateViewModel model, int creatorUserId, string creatorName);
        Task<bool> UpdateProjectAsync(ProjectCreateUpdateViewModel model, int editorUserId, string editorName);
        Task<bool> DeleteProjectAsync(int id, int userId, string userName);
        Task<bool> ArchiveProjectAsync(int id, int userId, string userName);
        Task<bool> UnarchiveProjectAsync(int id, int userId, string userName);
        Task<bool> AddMemberAsync(int projectId, int userId, string role, int actionUserId, string actionUserName);
        Task<bool> RemoveMemberAsync(int projectId, int userId, int actionUserId, string actionUserName);
        Task<bool> AssignManagerAsync(int projectId, int managerId, int actionUserId, string actionUserName);
        Task<bool> UploadFileAsync(int projectId, IFormFile file, int userId, string userName, string webRootPath);
        Task<int> RecalculateProgressAsync(int projectId);
        Task<bool> UpdateProjectPriorityAsync(int id, ProjectPriority priority, int userId, string userName);
    }
}