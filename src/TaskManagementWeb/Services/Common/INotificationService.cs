using TaskManagementWeb.Models.Entities;

namespace TaskManagementWeb.Services.Common
{
    public interface INotificationService
    {
        Task<List<Notification>> GetUserNotificationsAsync(int userId, int limit = 20);
        Task<int> GetUnreadCountAsync(int userId);
        Task<bool> MarkAsReadAsync(int notificationId, int userId);
        Task<bool> MarkAllAsReadAsync(int userId);
        Task CreateNotificationAsync(int userId, string title, string message);
        Task NotifyTaskAssignedAsync(int taskId, int assignedUserId, string taskTitle, string projectName, string assignedByName);
        Task NotifyTaskCompletedAsync(int taskId, string taskTitle, string projectName, string completedByName, int projectManagerId);
        Task NotifyProjectAddedAsync(int projectId, int memberUserId, string projectCode, string projectTitle, string managerName);
        Task<NotificationPreference> GetOrCreatePreferencesAsync(int userId);
        Task<bool> UpdatePreferencesAsync(NotificationPreference preference);
    }
}
