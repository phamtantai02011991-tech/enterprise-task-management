using Microsoft.AspNetCore.Mvc;
using TaskManagementWeb.Services.Admin;

namespace TaskManagementWeb.Areas.Admin.Controllers
{
    public class AuditLogController : AdminBaseController
    {
        private readonly IAuditLogService _auditLogService;

        public AuditLogController(IAuditLogService auditLogService)
        {
            _auditLogService = auditLogService;
        }

        public async Task<IActionResult> Index(string? searchKey, string? actionFilter, DateTime? startDate, DateTime? endDate, int page = 1)
        {
            var model = await _auditLogService.GetAuditLogsAsync(searchKey, actionFilter, startDate, endDate, page);
            return View(model);
        }
    }
}
