using Microsoft.AspNetCore.Mvc;
using TaskManagementWeb.Models.ViewModels.Admin;
using TaskManagementWeb.Services.Admin;

namespace TaskManagementWeb.Areas.Admin.Controllers
{
    public class DepartmentController : AdminBaseController
    {
        private readonly IDepartmentService _departmentService;

        public DepartmentController(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        public async Task<IActionResult> Index(string? searchKey)
        {
            var model = await _departmentService.GetDepartmentsAsync(searchKey);
            return View(model);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new DepartmentFormViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DepartmentFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _departmentService.CreateDepartmentAsync(model, CurrentUserId, CurrentUserName);
            if (!result.Success)
            {
                ModelState.AddModelError("", result.Message);
                return View(model);
            }

            TempData["SuccessMessage"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var model = await _departmentService.GetDepartmentFormViewModelAsync(id);
            if (model == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy phòng ban.";
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(DepartmentFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _departmentService.UpdateDepartmentAsync(model, CurrentUserId, CurrentUserName);
            if (!result.Success)
            {
                ModelState.AddModelError("", result.Message);
                return View(model);
            }

            TempData["SuccessMessage"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _departmentService.DeleteDepartmentAsync(id, CurrentUserId, CurrentUserName);
            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message;
            }
            else
            {
                TempData["ErrorMessage"] = result.Message;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
