using Microsoft.EntityFrameworkCore;
using TaskManagementWeb.Data;
using TaskManagementWeb.Models.Entities;
using TaskManagementWeb.Models.ViewModels.Admin;

namespace TaskManagementWeb.Services.Admin
{
    public class AuditLogService : IAuditLogService
    {
        private readonly ApplicationDbContext _context;

        public AuditLogService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task LogActionAsync(int userId, string userName, int? projectId, string action, string entityName, string details)
        {
            var auditLog = new AuditLog
            {
                UserId = userId,
                UserName = userName,
                ProjectId = projectId,
                Action = action.ToUpper(),
                EntityName = entityName,
                Details = details,
                Timestamp = DateTime.UtcNow
            };

            _context.AuditLogs.Add(auditLog);
            await _context.SaveChangesAsync();
        }

        public async Task<AuditLogListViewModel> GetAuditLogsAsync(string? searchKey, string? actionFilter, DateTime? startDate, DateTime? endDate, int page = 1, int pageSize = 15)
        {
            var query = _context.AuditLogs
                .Include(a => a.Project)
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(searchKey))
            {
                var key = searchKey.Trim().ToLower();
                query = query.Where(a => a.UserName.ToLower().Contains(key) ||
                                         a.EntityName.ToLower().Contains(key) ||
                                         (a.Details != null && a.Details.ToLower().Contains(key)));
            }

            if (!string.IsNullOrWhiteSpace(actionFilter))
            {
                query = query.Where(a => a.Action.ToUpper() == actionFilter.Trim().ToUpper());
            }

            if (startDate.HasValue)
            {
                query = query.Where(a => a.Timestamp >= startDate.Value.Date);
            }

            if (endDate.HasValue)
            {
                var endOfDay = endDate.Value.Date.AddDays(1).AddTicks(-1);
                query = query.Where(a => a.Timestamp <= endOfDay);
            }

            int totalItems = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            if (totalPages < 1) totalPages = 1;
            if (page < 1) page = 1;
            if (page > totalPages) page = totalPages;

            var logs = await query
                .OrderByDescending(a => a.Timestamp)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(a => new AuditLogItemViewModel
                {
                    Id = a.Id,
                    UserId = a.UserId,
                    UserName = a.UserName,
                    ProjectId = a.ProjectId,
                    ProjectTitle = a.Project != null ? a.Project.Title : null,
                    Action = a.Action,
                    EntityName = a.EntityName,
                    Details = a.Details,
                    Timestamp = a.Timestamp
                })
                .ToListAsync();

            return new AuditLogListViewModel
            {
                Logs = logs,
                SearchKey = searchKey,
                ActionFilter = actionFilter,
                StartDate = startDate,
                EndDate = endDate,
                AvailableActions = await GetDistinctActionsAsync(),
                CurrentPage = page,
                TotalPages = totalPages,
                TotalItems = totalItems
            };
        }

        public async Task<List<string>> GetDistinctActionsAsync()
        {
            return await _context.AuditLogs
                .Select(a => a.Action.ToUpper())
                .Distinct()
                .OrderBy(a => a)
                .ToListAsync();
        }
    }
}
