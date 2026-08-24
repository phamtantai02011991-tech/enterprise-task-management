using Microsoft.EntityFrameworkCore;
using TaskManagementWeb.Data;
using TaskManagementWeb.Models.Entities;
using TaskManagementWeb.Models.ViewModels.Admin;

namespace TaskManagementWeb.Services.Admin
{
    public class AdminUserService : IAdminUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly IAuditLogService _auditLogService;

        public AdminUserService(ApplicationDbContext context, IAuditLogService auditLogService)
        {
            _context = context;
            _auditLogService = auditLogService;
        }

        public async Task<UserListViewModel> GetUsersAsync(string? searchKey, int? roleIdFilter, int? departmentIdFilter, int page = 1, int pageSize = 10)
        {
            var query = _context.Users
                .Include(u => u.Role)
                .Include(u => u.UserDepartments)
                    .ThenInclude(ud => ud.Department)
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(searchKey))
            {
                var key = searchKey.Trim().ToLower();
                query = query.Where(u => u.FullName.ToLower().Contains(key) || u.Email.ToLower().Contains(key));
            }

            if (roleIdFilter.HasValue && roleIdFilter > 0)
            {
                query = query.Where(u => u.RoleId == roleIdFilter.Value);
            }

            if (departmentIdFilter.HasValue && departmentIdFilter > 0)
            {
                query = query.Where(u => u.UserDepartments.Any(ud => ud.DepartmentId == departmentIdFilter.Value));
            }

            int totalItems = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            if (totalPages < 1) totalPages = 1;
            if (page < 1) page = 1;
            if (page > totalPages) page = totalPages;

            var users = await query
                .OrderByDescending(u => u.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new UserItemViewModel
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    Email = u.Email,
                    RoleId = u.RoleId,
                    RoleName = u.Role != null ? u.Role.RoleName : "N/A",
                    DepartmentIds = u.UserDepartments.Select(ud => ud.DepartmentId).ToList(),
                    DepartmentNames = u.UserDepartments.Select(ud => ud.Department.Name).ToList(),
                    IsActive = u.IsActive,
                    CreatedAt = u.CreatedAt
                })
                .ToListAsync();

            return new UserListViewModel
            {
                Users = users,
                SearchKey = searchKey,
                RoleIdFilter = roleIdFilter,
                DepartmentIdFilter = departmentIdFilter,
                AvailableRoles = await GetRolesAsync(),
                AvailableDepartments = await GetDepartmentsAsync(),
                CurrentPage = page,
                TotalPages = totalPages,
                TotalItems = totalItems
            };
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _context.Users
                .Include(u => u.Role)
                .Include(u => u.UserDepartments)
                    .ThenInclude(ud => ud.Department)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<EditUserViewModel?> GetEditUserViewModelAsync(int id)
        {
            var user = await GetUserByIdAsync(id);
            if (user == null) return null;

            return new EditUserViewModel
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                RoleId = user.RoleId,
                DepartmentIds = user.UserDepartments.Select(ud => ud.DepartmentId).ToList(),
                IsActive = user.IsActive,
                AvailableRoles = await GetRolesAsync(),
                AvailableDepartments = await GetDepartmentsAsync()
            };
        }

        public async Task<CreateUserViewModel> PrepareCreateUserViewModelAsync()
        {
            return new CreateUserViewModel
            {
                AvailableRoles = await GetRolesAsync(),
                AvailableDepartments = await GetDepartmentsAsync()
            };
        }

        public async Task<(bool Success, string Message)> CreateUserAsync(CreateUserViewModel model, int actionByUserId, string actionByUserName)
        {
            bool emailExists = await _context.Users.AnyAsync(u => u.Email.ToLower() == model.Email.Trim().ToLower());
            if (emailExists)
            {
                return (false, "Email này đã được sử dụng trong hệ thống.");
            }

            var user = new User
            {
                FullName = model.FullName.Trim(),
                Email = model.Email.Trim().ToLower(),
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
                RoleId = model.RoleId,
                CreatedAt = DateTime.UtcNow
            };

            if (model.DepartmentIds != null && model.DepartmentIds.Any())
            {
                bool isFirst = true;
                foreach (var deptId in model.DepartmentIds.Distinct())
                {
                    user.UserDepartments.Add(new UserDepartment
                    {
                        DepartmentId = deptId,
                        IsPrimary = isFirst,
                        AssignedAt = DateTime.UtcNow
                    });
                    isFirst = false;
                }
            }

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            await _auditLogService.LogActionAsync(
                actionByUserId,
                actionByUserName,
                null,
                "CREATE",
                "User",
                $"Tạo tài khoản mới: {user.FullName} ({user.Email}), RoleId={user.RoleId}, Gán {user.UserDepartments.Count} phòng ban"
            );

            return (true, "Tạo tài khoản người dùng thành công.");
        }

        public async Task<(bool Success, string Message)> UpdateUserAsync(EditUserViewModel model, int actionByUserId, string actionByUserName)
        {
            var user = await _context.Users
                .Include(u => u.UserDepartments)
                .FirstOrDefaultAsync(u => u.Id == model.Id);

            if (user == null)
            {
                return (false, "Không tìm thấy thông tin người dùng.");
            }

            bool emailExists = await _context.Users.AnyAsync(u => u.Id != model.Id && u.Email.ToLower() == model.Email.Trim().ToLower());
            if (emailExists)
            {
                return (false, "Email này đã được sử dụng bởi người dùng khác.");
            }

            int oldRoleId = user.RoleId;
            string oldDetails = $"FullName={user.FullName}, RoleId={user.RoleId}, DeptCount={user.UserDepartments.Count}";

            user.FullName = model.FullName.Trim();
            user.Email = model.Email.Trim().ToLower();
            user.RoleId = model.RoleId;
            user.IsActive = model.IsActive;

            // Nếu hạ quyền từ Manager (RoleId=2) xuống Employee (RoleId=3) hoặc vai trò khác:
            // Tự động thu hồi quyền Trưởng dự án ở mọi dự án đang quản lý
            if (oldRoleId == 2 && model.RoleId != 2)
            {
                var managedProjects = await _context.Projects
                    .Where(p => p.ManagerId == user.Id)
                    .ToListAsync();

                foreach (var p in managedProjects)
                {
                    p.ManagerId = null;
                    p.UpdatedAt = DateTime.UtcNow;
                }

                var projectMemberships = await _context.ProjectMembers
                    .Where(pm => pm.UserId == user.Id && pm.RoleInProject == "Manager")
                    .ToListAsync();

                foreach (var pm in projectMemberships)
                {
                    pm.RoleInProject = "Member";
                }
            }

            // Cập nhật lại danh sách phòng ban Many-to-Many
            _context.UserDepartments.RemoveRange(user.UserDepartments);

            if (model.DepartmentIds != null && model.DepartmentIds.Any())
            {
                bool isFirst = true;
                foreach (var deptId in model.DepartmentIds.Distinct())
                {
                    user.UserDepartments.Add(new UserDepartment
                    {
                        UserId = user.Id,
                        DepartmentId = deptId,
                        IsPrimary = isFirst,
                        AssignedAt = DateTime.UtcNow
                    });
                    isFirst = false;
                }
            }

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            await _auditLogService.LogActionAsync(
                actionByUserId,
                actionByUserName,
                null,
                "UPDATE",
                "User",
                $"Cập nhật tài khoản User ID {user.Id}. Cũ: [{oldDetails}] -> Mới: [FullName={user.FullName}, RoleId={user.RoleId}, IsActive={user.IsActive}, DeptCount={user.UserDepartments.Count}]"
            );

            return (true, "Cập nhật thông tin người dùng thành công.");
        }

        public async Task<(bool Success, string Message)> ResetPasswordAsync(int userId, string newPassword, int actionByUserId, string actionByUserName)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return (false, "Không tìm thấy người dùng.");
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            await _auditLogService.LogActionAsync(
                actionByUserId,
                actionByUserName,
                null,
                "RESET_PASSWORD",
                "User",
                $"Đặt lại mật khẩu cho tài khoản {user.FullName} ({user.Email})"
            );

            return (true, $"Đặt lại mật khẩu thành công cho người dùng {user.FullName}.");
        }

        public async Task<(bool Success, string Message)> ToggleUserStatusAsync(int userId, int actionByUserId, string actionByUserName)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return (false, "Không tìm thấy tài khoản người dùng.");
            }

            if (user.Id == actionByUserId)
            {
                return (false, "Bạn không thể tự thay đổi trạng thái tài khoản Admin đang đăng nhập.");
            }

            user.IsActive = !user.IsActive;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            string action = user.IsActive ? "ACTIVATE_USER" : "DEACTIVATE_USER";
            string statusText = user.IsActive ? "kích hoạt hoạt động" : "tạm khóa / vô hiệu hóa";

            await _auditLogService.LogActionAsync(
                actionByUserId,
                actionByUserName,
                null,
                action,
                "User",
                $"Đã {statusText} tài khoản người dùng: {user.FullName} ({user.Email})"
            );

            return (true, $"Đã {statusText} tài khoản {user.FullName} thành công.");
        }

        public async Task<(bool Success, string Message)> DeleteUserAsync(int userId, int actionByUserId, string actionByUserName)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return (false, "Không tìm thấy tài khoản người dùng.");
            }

            if (user.Id == actionByUserId)
            {
                return (false, "Bạn không thể tự vô hiệu hóa tài khoản Admin đang đăng nhập.");
            }

            // Xóa mềm: Chuyển IsActive = false để bảo toàn toàn bộ dữ liệu lịch sử dự án/task
            user.IsActive = false;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            await _auditLogService.LogActionAsync(
                actionByUserId,
                actionByUserName,
                null,
                "SOFT_DELETE",
                "User",
                $"Vô hiệu hóa (Soft Delete) tài khoản {user.FullName} ({user.Email}) - Toàn bộ dữ liệu dự án và lịch sử được giữ nguyên vẹn."
            );

            return (true, $"Đã chuyển tài khoản {user.FullName} sang trạng thái ngừng hoạt động (Bảo toàn dữ liệu).");
        }

        public async Task<List<Role>> GetRolesAsync()
        {
            return await _context.Roles.AsNoTracking().ToListAsync();
        }

        public async Task<List<Department>> GetDepartmentsAsync()
        {
            return await _context.Departments.AsNoTracking().ToListAsync();
        }
    }
}
