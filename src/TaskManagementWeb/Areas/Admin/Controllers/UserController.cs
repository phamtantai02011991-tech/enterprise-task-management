using Microsoft.AspNetCore.Mvc;
using TaskManagementWeb.Models.ViewModels.Admin;
using TaskManagementWeb.Services.Admin;

namespace TaskManagementWeb.Areas.Admin.Controllers
{
    public class UserController : AdminBaseController
    {
        private readonly IAdminUserService _userService;

        public UserController(IAdminUserService userService)
        {
            _userService = userService;
        }

        public async Task<IActionResult> Index(string? searchKey, int? roleIdFilter, int? departmentIdFilter, int page = 1)
        {
            var model = await _userService.GetUsersAsync(searchKey, roleIdFilter, departmentIdFilter, page);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = await _userService.PrepareCreateUserViewModelAsync();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.AvailableRoles = await _userService.GetRolesAsync();
                model.AvailableDepartments = await _userService.GetDepartmentsAsync();
                return View(model);
            }

            var result = await _userService.CreateUserAsync(model, CurrentUserId, CurrentUserName);
            if (!result.Success)
            {
                ModelState.AddModelError("", result.Message);
                model.AvailableRoles = await _userService.GetRolesAsync();
                model.AvailableDepartments = await _userService.GetDepartmentsAsync();
                return View(model);
            }

            TempData["SuccessMessage"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var model = await _userService.GetEditUserViewModelAsync(id);
            if (model == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy người dùng.";
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.AvailableRoles = await _userService.GetRolesAsync();
                model.AvailableDepartments = await _userService.GetDepartmentsAsync();
                return View(model);
            }

            var result = await _userService.UpdateUserAsync(model, CurrentUserId, CurrentUserName);
            if (!result.Success)
            {
                ModelState.AddModelError("", result.Message);
                model.AvailableRoles = await _userService.GetRolesAsync();
                model.AvailableDepartments = await _userService.GetDepartmentsAsync();
                return View(model);
            }

            TempData["SuccessMessage"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(int id, string newPassword)
        {
            if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 6)
            {
                TempData["ErrorMessage"] = "Mật khẩu mới phải từ 6 ký tự trở lên.";
                return RedirectToAction(nameof(Index));
            }

            var result = await _userService.ResetPasswordAsync(id, newPassword, CurrentUserId, CurrentUserName);
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var result = await _userService.ToggleUserStatusAsync(id, CurrentUserId, CurrentUserName);
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _userService.DeleteUserAsync(id, CurrentUserId, CurrentUserName);
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
