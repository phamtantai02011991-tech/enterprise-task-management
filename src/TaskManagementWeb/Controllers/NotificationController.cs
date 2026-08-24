using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagementWeb.Models.Entities;
using TaskManagementWeb.Services.Common;

namespace TaskManagementWeb.Controllers
{
    [Authorize]
    public class NotificationController : Controller
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return (userIdClaim != null && int.TryParse(userIdClaim.Value, out int id)) ? id : 0;
        }

        // GET: /Notification/Index (Trung tâm thông báo)
        public async Task<IActionResult> Index()
        {
            int userId = GetCurrentUserId();
            var notifications = await _notificationService.GetUserNotificationsAsync(userId, 50);
            return View(notifications);
        }

        // GET: /Notification/GetUnreadCount (JSON API cho chuông thông báo Header)
        [HttpGet]
        public async Task<IActionResult> GetUnreadSummary()
        {
            int userId = GetCurrentUserId();
            int count = await _notificationService.GetUnreadCountAsync(userId);
            var list = await _notificationService.GetUserNotificationsAsync(userId, 5);

            return Json(new
            {
                unreadCount = count,
                items = list.Select(n => new
                {
                    id = n.Id,
                    title = n.Title,
                    message = n.Message,
                    isRead = n.IsRead,
                    timeAgo = GetTimeAgo(n.CreatedAt)
                })
            });
        }

        // POST: /Notification/MarkAsRead
        [HttpPost]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            int userId = GetCurrentUserId();
            var success = await _notificationService.MarkAsReadAsync(id, userId);
            return Json(new { success });
        }

        // POST: /Notification/MarkAllAsRead
        [HttpPost]
        public async Task<IActionResult> MarkAllAsRead()
        {
            int userId = GetCurrentUserId();
            var success = await _notificationService.MarkAllAsReadAsync(userId);
            return Json(new { success });
        }

        // GET: /Notification/Preferences (Tùy chọn nhận thông báo)
        [HttpGet]
        public async Task<IActionResult> Preferences()
        {
            int userId = GetCurrentUserId();
            var pref = await _notificationService.GetOrCreatePreferencesAsync(userId);
            return View(pref);
        }

        // POST: /Notification/Preferences
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Preferences(NotificationPreference model)
        {
            int userId = GetCurrentUserId();
            model.UserId = userId;
            await _notificationService.UpdatePreferencesAsync(model);

            TempData["SuccessMessage"] = "Cập nhật tùy chọn thông báo thành công!";
            return RedirectToAction(nameof(Preferences));
        }

        private static string GetTimeAgo(DateTime dt)
        {
            var span = DateTime.UtcNow - dt;
            if (span.TotalMinutes < 1) return "Vừa xong";
            if (span.TotalMinutes < 60) return $"{(int)span.TotalMinutes} phút trước";
            if (span.TotalHours < 24) return $"{(int)span.TotalHours} giờ trước";
            return dt.ToString("dd/MM/yyyy");
        }
    }
}
