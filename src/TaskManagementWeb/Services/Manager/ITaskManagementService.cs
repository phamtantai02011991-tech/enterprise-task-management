using TaskManagementWeb.Models.Entities;
using TaskManagementWeb.ViewModels.Manager;

namespace TaskManagementWeb.Services.Manager
{
    public interface ITaskManagementService
    {
        Task<List<TaskItem>> GetTasksByProjectIdAsync(int projectId);
        Task<TaskItem?> GetTaskByIdAsync(int id);
        Task CreateTaskAsync(TaskCreateUpdateViewModel model);
        Task UpdateTaskAsync(TaskCreateUpdateViewModel model);
        Task DeleteTaskAsync(int id);
    }
}