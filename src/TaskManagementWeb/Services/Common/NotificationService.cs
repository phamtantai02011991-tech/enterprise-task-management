using Microsoft.EntityFrameworkCore;
using TaskManagementWeb.Data;
using TaskManagementWeb.Models.Entities;

namespace TaskManagementWeb.Services.Common
{
    public class NotificationService : INotificationService
    {
        private readonly ApplicationDbContext _context;

        public NotificationService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Notification>> GetUserNotificationsAsync(int userId, int limit = 20)
        {
            return await _context.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .Take(limit)
                .ToListAsync();
        }

        public async Task<int> GetUnreadCountAsync(int userId)
        {
            return await _context.Notifications
                .CountAsync(n => n.UserId == userId && !n.IsRead);
        }

        public async Task<bool> MarkAsReadAsync(int notificationId, int userId)
        {
            var notification = await _context.Notifications
                .FirstOrDefaultAsync(n => n.Id == notificationId && n.UserId == userId);

            if (notification == null) return false;

            notification.IsRead = true;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> MarkAllAsReadAsync(int userId)
        {
            var unreadList = await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .ToListAsync();

            if (!unreadList.Any()) return true;

            foreach (var item in unreadList)
            {
                item.IsRead = true;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task CreateNotificationAsync(int userId, string title, string message)
        {
            var notification = new Notification
            {
                UserId = userId,
                Title = title,
                Message = message,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
        }

        public async Task NotifyTaskAssignedAsync(int taskId, int assignedUserId, string taskTitle, string projectName, string assignedByName)
        {
            var pref = await GetOrCreatePreferencesAsync(assignedUserId);
            if (pref.InAppOnTaskAssign)
            {
                await CreateNotificationAsync(
                    assignedUserId,
                    "Phân công Công việc mới",
                    $"{assignedByName} đã giao công việc [{taskTitle}] thuộc dự án [{projectName}] cho bạn."
                );
            }
        }

        public async Task NotifyTaskCompletedAsync(int taskId, string taskTitle, string projectName, string completedByName, int projectManagerId)
        {
            var pref = await GetOrCreatePreferencesAsync(projectManagerId);
            if (pref.InAppOnTaskCompleted)
            {
                await CreateNotificationAsync(
                    projectManagerId,
                    "Công việc đã Hoàn thành",
                    $"{completedByName} vừa đánh dấu hoàn thành công việc [{taskTitle}] trong dự án [{projectName}]."
                );
            }
        }

        public async Task NotifyProjectAddedAsync(int projectId, int memberUserId, string projectCode, string projectTitle, string managerName)
        {
            var pref = await GetOrCreatePreferencesAsync(memberUserId);
            if (pref.InAppOnProjectAdded)
            {
                await CreateNotificationAsync(
                    memberUserId,
                    "Tham gia Dự án mới",
                    $"Bạn đã được thêm vào dự án [{projectCode}] {projectTitle} bởi {managerName}."
                );
            }
        }

        public async Task<NotificationPreference> GetOrCreatePreferencesAsync(int userId)
        {
            var pref = await _context.NotificationPreferences
                .FirstOrDefaultAsync(np => np.UserId == userId);

            if (pref == null)
            {
                pref = new NotificationPreference
                {
                    UserId = userId,
                    InAppOnTaskAssign = true,
                    InAppOnDeadline = true,
                    InAppOnTaskCompleted = true,
                    InAppOnProjectAdded = true,
                    EmailAlertsEnabled = true
                };

                _context.NotificationPreferences.Add(pref);
                await _context.SaveChangesAsync();
            }

            return pref;
        }

        public async Task<bool> UpdatePreferencesAsync(NotificationPreference preference)
        {
            var existing = await _context.NotificationPreferences
                .FirstOrDefaultAsync(np => np.UserId == preference.UserId);

            if (existing == null)
            {
                _context.NotificationPreferences.Add(preference);
            }
            else
            {
                existing.InAppOnTaskAssign = preference.InAppOnTaskAssign;
                existing.InAppOnDeadline = preference.InAppOnDeadline;
                existing.InAppOnTaskCompleted = preference.InAppOnTaskCompleted;
                existing.InAppOnProjectAdded = preference.InAppOnProjectAdded;
                existing.EmailAlertsEnabled = preference.EmailAlertsEnabled;
            }

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
