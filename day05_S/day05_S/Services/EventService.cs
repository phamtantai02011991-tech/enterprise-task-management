using day05_S.Data;
using day05_S.Models;
using Microsoft.EntityFrameworkCore;

namespace day05_S.Services
{
    public class EventService : IEventService
    {
        private readonly EventDbContext context;

        public EventService(EventDbContext context)
        {
            this.context = context;
        }

        public Task<bool> CancelRegistrationAsync(string userId, string regId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Event>> GetAllEventsAsync()
        {
            return await context.Events
                .AsNoTracking()
                .ToListAsync();
        }

        public Task<object> GetStatisticsAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<List<EventRegistration>> GetUserRegistrationsAsync(string userId)
        {
            return await context.EventRegistrations
                .Include(r => r.Event)                  // lấy thông tin sự kiện
                .AsNoTracking()
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.RegTime)
                .ToListAsync();
        }

        public async Task<string> RegisterEventAsync(string userId, string eventId)
        {
            // Kiểm tra tham số
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(eventId))
            {
                return "UserId and EventId cannot be null or empty.";
            }

            // Tìm sự kiện
            var ev = await context.Events
                .FirstOrDefaultAsync(e => e.EventId == eventId.Trim());

            if (ev == null)
            {
                return "Event not found.";
            }

            // Kiểm tra đã đăng ký chưa
            bool exists = await context.EventRegistrations
                .AnyAsync(r => r.UserId == userId && r.EventId == eventId);

            if (exists)
            {
                return "You have already registered for this event.";
            }

            // Tạo đăng ký mới
            var registration = new EventRegistration
            {
                UserId = userId,
                EventId = eventId,
                RegTime = DateTime.Now
                // Thêm các thuộc tính khác nếu cần (Status, ...)
            };

            context.EventRegistrations.Add(registration);
            await context.SaveChangesAsync();

            return "Registration successful.";

            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                var reg = new EventRegistration
                {
                    UserId = userId,
                    EventId = eventId,
                    RegTime = DateTime.Now
                };
                context.EventRegistrations.Add(reg);
                ev.CurrentCapacity += 1;
                context.Events.Update(ev);
                await context.SaveChangesAsync();
                await transaction.CommitAsync();
                return "Registration successful.";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return "Fail";
            } 
        }
    }
}