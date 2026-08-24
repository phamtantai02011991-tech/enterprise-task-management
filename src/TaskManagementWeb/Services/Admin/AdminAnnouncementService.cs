using Microsoft.EntityFrameworkCore;
using TaskManagementWeb.Data;
using TaskManagementWeb.Models.Entities;

namespace TaskManagementWeb.Services.Admin
{
    public class AdminAnnouncementService : IAdminAnnouncementService
    {
        private readonly ApplicationDbContext _context;

        public AdminAnnouncementService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Announcement>> GetAllAnnouncementsAsync()
        {
            return await _context.Announcements
                .Include(a => a.CreatedByUser)
                .OrderByDescending(a => a.IsPinned)
                .ThenByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Announcement>> GetActiveAnnouncementsAsync()
        {
            var today = DateTime.Today;
            return await _context.Announcements
                .Include(a => a.CreatedByUser)
                .Where(a => a.IsActive && (!a.ExpiryDate.HasValue || a.ExpiryDate.Value.Date >= today))
                .OrderByDescending(a => a.IsPinned)
                .ThenByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

        public async Task<Announcement?> GetAnnouncementByIdAsync(int id)
        {
            return await _context.Announcements
                .Include(a => a.CreatedByUser)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<bool> CreateAnnouncementAsync(Announcement announcement, int adminUserId)
        {
            announcement.CreatedByUserId = adminUserId;
            announcement.CreatedAt = DateTime.UtcNow;
            _context.Announcements.Add(announcement);
            await _context.SaveChangesAsync();

            // Tự động phát thông báo tới toàn bộ nhân sự (All Users: Admins, Managers, Employees)
            var allUsers = await _context.Users.ToListAsync();
            foreach (var user in allUsers)
            {
                _context.Notifications.Add(new Notification
                {
                    UserId = user.Id,
                    Title = $"[Thông Báo Hệ Thống] {announcement.Title}",
                    Message = announcement.Content.Length > 200 ? announcement.Content.Substring(0, 197) + "..." : announcement.Content,
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow
                });
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateAnnouncementAsync(Announcement announcement)
        {
            var existing = await _context.Announcements.FindAsync(announcement.Id);
            if (existing == null) return false;

            existing.Title = announcement.Title;
            existing.Content = announcement.Content;
            existing.Type = announcement.Type;
            existing.IsActive = announcement.IsActive;
            existing.IsPinned = announcement.IsPinned;
            existing.ExpiryDate = announcement.ExpiryDate;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAnnouncementAsync(int id)
        {
            var announcement = await _context.Announcements.FindAsync(id);
            if (announcement == null) return false;

            _context.Announcements.Remove(announcement);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ToggleActiveAsync(int id)
        {
            var announcement = await _context.Announcements.FindAsync(id);
            if (announcement == null) return false;

            announcement.IsActive = !announcement.IsActive;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
