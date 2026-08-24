using TaskManagementWeb.Models.Entities;

namespace TaskManagementWeb.Services.Admin
{
    public interface IAdminAnnouncementService
    {
        Task<List<Announcement>> GetAllAnnouncementsAsync();
        Task<List<Announcement>> GetActiveAnnouncementsAsync();
        Task<Announcement?> GetAnnouncementByIdAsync(int id);
        Task<bool> CreateAnnouncementAsync(Announcement announcement, int adminUserId);
        Task<bool> UpdateAnnouncementAsync(Announcement announcement);
        Task<bool> DeleteAnnouncementAsync(int id);
        Task<bool> ToggleActiveAsync(int id);
    }
}
