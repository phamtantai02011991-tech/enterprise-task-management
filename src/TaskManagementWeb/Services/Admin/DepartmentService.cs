using Microsoft.EntityFrameworkCore;
using TaskManagementWeb.Data;
using TaskManagementWeb.Models.Entities;
using TaskManagementWeb.Models.ViewModels.Admin;

namespace TaskManagementWeb.Services.Admin
{
    public class DepartmentService : IDepartmentService
    {
        private readonly ApplicationDbContext _context;
        private readonly IAuditLogService _auditLogService;

        public DepartmentService(ApplicationDbContext context, IAuditLogService auditLogService)
        {
            _context = context;
            _auditLogService = auditLogService;
        }

        public async Task<DepartmentListViewModel> GetDepartmentsAsync(string? searchKey)
        {
            var query = _context.Departments
                .Include(d => d.UserDepartments)
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(searchKey))
            {
                var key = searchKey.Trim().ToLower();
                query = query.Where(d => d.Name.ToLower().Contains(key) || d.Code.ToLower().Contains(key));
            }

            var departments = await query
                .OrderBy(d => d.Name)
                .Select(d => new DepartmentItemViewModel
                {
                    Id = d.Id,
                    Name = d.Name,
                    Code = d.Code,
                    Description = d.Description,
                    MemberCount = d.UserDepartments.Count
                })
                .ToListAsync();

            return new DepartmentListViewModel
            {
                Departments = departments,
                SearchKey = searchKey
            };
        }

        public async Task<Department?> GetDepartmentByIdAsync(int id)
        {
            return await _context.Departments
                .Include(d => d.UserDepartments)
                    .ThenInclude(ud => ud.User)
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<DepartmentFormViewModel?> GetDepartmentFormViewModelAsync(int id)
        {
            var dept = await GetDepartmentByIdAsync(id);
            if (dept == null) return null;

            var existingUserIds = dept.UserDepartments.Select(ud => ud.UserId).ToHashSet();

            var availableUsers = await _context.Users
                .Include(u => u.Role)
                .OrderBy(u => u.FullName)
                .Select(u => new UserSelectionDto
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    Email = u.Email,
                    RoleName = u.Role != null ? u.Role.RoleName : "N/A",
                    IsSelected = existingUserIds.Contains(u.Id)
                })
                .ToListAsync();

            return new DepartmentFormViewModel
            {
                Id = dept.Id,
                Name = dept.Name,
                Code = dept.Code,
                Description = dept.Description,
                MemberCount = dept.UserDepartments.Count,
                SelectedUserIds = existingUserIds.ToList(),
                AvailableUsers = availableUsers
            };
        }

        private async Task<string> EnsureUniqueCodeAsync(string? providedCode, string name)
        {
            string baseCode = string.IsNullOrWhiteSpace(providedCode) 
                ? GenerateCodeFromName(name) 
                : providedCode.Trim().ToUpper();

            string code = baseCode;
            int counter = 1;
            while (await _context.Departments.AnyAsync(d => d.Code.ToLower() == code.ToLower()))
            {
                code = $"{baseCode}{counter:D2}";
                counter++;
            }
            return code;
        }

        private static string GenerateCodeFromName(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return "DEPT";

            string normalized = name.Normalize(System.Text.NormalizationForm.FormD);
            var sb = new System.Text.StringBuilder();
            foreach (char c in normalized)
            {
                if (System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c) != System.Globalization.UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(c);
                }
            }
            string cleanStr = System.Text.RegularExpressions.Regex.Replace(sb.ToString(), @"[^a-zA-Z0-9\s]", "");
            var words = cleanStr.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (words.Length == 0) return "DEPT";

            string code = string.Concat(words.Select(w => char.ToUpper(w[0])));
            if (code.Length == 1 && words[0].Length >= 3)
            {
                code = words[0].Substring(0, 3).ToUpper();
            }
            return code;
        }

        public async Task<(bool Success, string Message)> CreateDepartmentAsync(DepartmentFormViewModel model, int actionByUserId, string actionByUserName)
        {
            string finalCode = await EnsureUniqueCodeAsync(model.Code, model.Name);

            var department = new Department
            {
                Name = model.Name.Trim(),
                Code = finalCode,
                Description = model.Description?.Trim()
            };

            if (model.SelectedUserIds != null && model.SelectedUserIds.Any())
            {
                foreach (var userId in model.SelectedUserIds.Distinct())
                {
                    department.UserDepartments.Add(new UserDepartment
                    {
                        UserId = userId,
                        IsPrimary = false,
                        AssignedAt = DateTime.UtcNow
                    });
                }
            }

            _context.Departments.Add(department);
            await _context.SaveChangesAsync();

            await _auditLogService.LogActionAsync(
                actionByUserId,
                actionByUserName,
                null,
                "CREATE",
                "Department",
                $"Tạo mới phòng ban: [{department.Code}] {department.Name}"
            );

            return (true, $"Tạo mới phòng ban thành công với mã [{department.Code}].");
        }

        public async Task<(bool Success, string Message)> UpdateDepartmentAsync(DepartmentFormViewModel model, int actionByUserId, string actionByUserName)
        {
            var dept = await _context.Departments
                .Include(d => d.UserDepartments)
                .FirstOrDefaultAsync(d => d.Id == model.Id);

            if (dept == null)
            {
                return (false, "Không tìm thấy phòng ban.");
            }

            string finalCode = string.IsNullOrWhiteSpace(model.Code) 
                ? GenerateCodeFromName(model.Name) 
                : model.Code.Trim().ToUpper();

            bool codeExists = await _context.Departments.AnyAsync(d => d.Id != model.Id && d.Code.ToLower() == finalCode.ToLower());
            if (codeExists)
            {
                return (false, $"Mã phòng ban [{finalCode}] đã trùng với phòng ban khác.");
            }

            string oldName = dept.Name;
            string oldCode = dept.Code;
            bool isNameOrCodeChanged = (oldName != model.Name.Trim()) || (oldCode != finalCode);

            dept.Name = model.Name.Trim();
            dept.Code = finalCode;
            dept.Description = model.Description?.Trim();

            // Cập nhật lại danh sách nhân sự thuộc phòng ban
            if (model.SelectedUserIds != null)
            {
                _context.UserDepartments.RemoveRange(dept.UserDepartments);
                foreach (var userId in model.SelectedUserIds.Distinct())
                {
                    dept.UserDepartments.Add(new UserDepartment
                    {
                        UserId = userId,
                        DepartmentId = dept.Id,
                        IsPrimary = false,
                        AssignedAt = DateTime.UtcNow
                    });
                }
            }

            _context.Departments.Update(dept);

            int notifiedCount = 0;
            if (isNameOrCodeChanged && dept.UserDepartments.Any())
            {
                notifiedCount = dept.UserDepartments.Count;
                foreach (var ud in dept.UserDepartments)
                {
                    _context.Notifications.Add(new Notification
                    {
                        UserId = ud.UserId,
                        Title = "Thông báo cập nhật thông tin phòng ban",
                        Message = $"Phòng ban của bạn đã được Admin cập nhật từ [{oldCode} - {oldName}] thành [{dept.Code} - {dept.Name}].",
                        CreatedAt = DateTime.UtcNow,
                        IsRead = false
                    });
                }
            }

            await _context.SaveChangesAsync();

            string auditDetails = isNameOrCodeChanged && notifiedCount > 0
                ? $"Cập nhật phòng ban ID {dept.Id}: Cũ [{oldCode} - {oldName}] -> Mới [{dept.Code} - {dept.Name}]. Đã phát thông báo đến {notifiedCount} nhân viên trong phòng."
                : $"Cập nhật mô tả/thông tin phòng ban ID {dept.Id} [{dept.Code} - {dept.Name}]";

            await _auditLogService.LogActionAsync(
                actionByUserId,
                actionByUserName,
                null,
                "UPDATE",
                "Department",
                auditDetails
            );

            string successMsg = notifiedCount > 0
                ? $"Cập nhật phòng ban thành công. Đã tự động gửi thông báo đến {notifiedCount} nhân viên thuộc phòng ban này."
                : "Cập nhật phòng ban thành công.";

            return (true, successMsg);
        }

        public async Task<(bool Success, string Message)> DeleteDepartmentAsync(int id, int actionByUserId, string actionByUserName)
        {
            var dept = await _context.Departments
                .Include(d => d.UserDepartments)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (dept == null)
            {
                return (false, "Không tìm thấy phòng ban.");
            }

            if (dept.UserDepartments.Any())
            {
                return (false, $"Không thể xóa phòng ban [{dept.Name}] vì hiện đang có {dept.UserDepartments.Count} nhân viên thuộc phòng ban này. Hãy chuyển nhân viên sang phòng ban khác trước.");
            }

            string deptName = dept.Name;
            string deptCode = dept.Code;

            // Xóa mềm: Chuyển IsActive = false để bảo toàn 100% dữ liệu lịch sử phòng ban
            dept.IsActive = false;
            _context.Departments.Update(dept);
            await _context.SaveChangesAsync();

            await _auditLogService.LogActionAsync(
                actionByUserId,
                actionByUserName,
                null,
                "SOFT_DELETE",
                "Department",
                $"Ngừng hoạt động / Vô hiệu hóa phòng ban [{deptCode}] {deptName} (Bảo toàn dữ liệu phân bổ lịch sử)"
            );

            return (true, $"Đã chuyển phòng ban [{deptName}] sang trạng thái ngừng hoạt động.");
        }
    }
}
