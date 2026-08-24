using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagementWeb.Models.Entities;
using TaskManagementWeb.Services.Admin;

namespace TaskManagementWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AnnouncementController : Controller
    {
        private readonly IAdminAnnouncementService _announcementService;

        public AnnouncementController(IAdminAnnouncementService announcementService)
        {
            _announcementService = announcementService;
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return (userIdClaim != null && int.TryParse(userIdClaim.Value, out int id)) ? id : 1;
        }

        // GET: /Admin/Announcement (Danh sách thông báo toàn hệ thống)
        public async Task<IActionResult> Index()
        {
            ViewData["ActivePage"] = "Announcement";
            var announcements = await _announcementService.GetAllAnnouncementsAsync();
            return View(announcements);
        }

        // GET: /Admin/Announcement/Create
        [HttpGet]
        public IActionResult Create()
        {
            ViewData["ActivePage"] = "Announcement";
            var model = new Announcement
            {
                Type = "Info",
                IsActive = true,
                ExpiryDate = DateTime.Today.AddDays(14)
            };
            return View(model);
        }

        // POST: /Admin/Announcement/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Announcement model)
        {
            ViewData["ActivePage"] = "Announcement";
            int adminId = GetCurrentUserId();

            ModelState.Remove(nameof(model.CreatedByUser));

            if (ModelState.IsValid)
            {
                await _announcementService.CreateAnnouncementAsync(model, adminId);
                TempData["SuccessMessage"] = "Đã phát thông báo toàn hệ thống thành công!";
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        // GET: /Admin/Announcement/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            ViewData["ActivePage"] = "Announcement";

            var model = await _announcementService.GetAnnouncementByIdAsync(id.Value);
            if (model == null) return NotFound();

            return View(model);
        }

        // POST: /Admin/Announcement/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Announcement model)
        {
            if (id != model.Id) return NotFound();
            ViewData["ActivePage"] = "Announcement";

            ModelState.Remove(nameof(model.CreatedByUser));

            if (ModelState.IsValid)
            {
                var success = await _announcementService.UpdateAnnouncementAsync(model);
                if (success)
                {
                    TempData["SuccessMessage"] = "Cập nhật thông báo thành công.";
                    return RedirectToAction(nameof(Index));
                }
            }

            return View(model);
        }

        // POST: /Admin/Announcement/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _announcementService.DeleteAnnouncementAsync(id);
            if (success)
            {
                TempData["SuccessMessage"] = "Đã xóa thông báo.";
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: /Admin/Announcement/ToggleActive/5
        [HttpPost]
        public async Task<IActionResult> ToggleActive(int id)
        {
            var success = await _announcementService.ToggleActiveAsync(id);
            if (success)
            {
                TempData["SuccessMessage"] = "Đã cập nhật trạng thái phát sóng thông báo thành công.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
