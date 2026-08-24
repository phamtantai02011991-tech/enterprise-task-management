using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TaskManagementWeb.Data;
using TaskManagementWeb.Models.Entities;
using TaskManagementWeb.ViewModels.Account;
using BCrypt.Net;

namespace TaskManagementWeb.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }
        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                if (User.IsInRole("Admin")) return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
                if (User.IsInRole("Manager")) return RedirectToAction("Index", "Dashboard", new { area = "Manager" });
                if (User.IsInRole("Employee")) return RedirectToAction("Index", "Dashboard", new { area = "Employee" });
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
                return View(model);

            var normalizedEmail = model.Email?.Trim() ?? string.Empty;
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email != null && u.Email.ToLower() == normalizedEmail.ToLower());

            if (user == null)
            {
                ModelState.AddModelError("", "Email hoặc mật khẩu không đúng.");
                return View(model);
            }

            bool isPasswordValid = false;
            try
            {
                isPasswordValid = BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash);
            }
            catch
            {
                isPasswordValid = (user.PasswordHash == model.Password);
            }

            if (!isPasswordValid)
            {
                ModelState.AddModelError("", "Email hoặc mật khẩu không đúng.");
                return View(model);
            }

            if (!user.IsActive)
            {
                ModelState.AddModelError("", "Tài khoản của bạn đã bị tạm khóa hoặc ngừng hoạt động. Vui lòng liên hệ Quản trị viên.");
                return View(model);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role?.RoleName ?? "Employee")
            };

            var identity = new ClaimsIdentity(claims, "CookieAuth");
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync("CookieAuth", principal);

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            if (user.Role?.RoleName == "Admin")
            {
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
            if (user.Role?.RoleName == "Manager")
            {
                return RedirectToAction("Index", "Dashboard", new { area = "Manager" });
            }
            if (user.Role?.RoleName == "Employee")
            {
                return RedirectToAction("Index", "Dashboard", new { area = "Employee" });
            }

            return RedirectToAction("Index", "Home");
        }

        // GET: /Account/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var existedUser = await _context.Users
                .AnyAsync(u => u.Email != null && u.Email.ToLower() == model.Email.ToLower().Trim());

            if (existedUser)
            {
                ModelState.AddModelError("Email", "Địa chỉ Email này đã được đăng ký trong hệ thống.");
                return View(model);
            }

            var employeeRole = await _context.Roles
                .FirstOrDefaultAsync(r => r.RoleName == "Employee");

            if (employeeRole == null)
            {
                employeeRole = await _context.Roles.FirstOrDefaultAsync();
            }

            var user = new User
            {
                FullName = model.FullName.Trim(),
                Email = model.Email.Trim(),
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
                RoleId = employeeRole != null ? employeeRole.Id : 1,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Đăng ký tài khoản thành công! Vui lòng đăng nhập với tài khoản vừa tạo.";
            return RedirectToAction("Login");
        }

        // GET / POST: /Account/Logout
        [HttpGet, HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("CookieAuth");
            return RedirectToAction("Login");
        }
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}