using System.ComponentModel.DataAnnotations;
using TaskManagementWeb.Models.Entities;

namespace TaskManagementWeb.Models.ViewModels.Admin
{
    public class UserListViewModel
    {
        public List<UserItemViewModel> Users { get; set; } = new();
        public string? SearchKey { get; set; }
        public int? RoleIdFilter { get; set; }
        public int? DepartmentIdFilter { get; set; }
        public List<Role> AvailableRoles { get; set; } = new();
        public List<Department> AvailableDepartments { get; set; } = new();
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; } = 1;
        public int TotalItems { get; set; }
    }

    public class UserItemViewModel
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int RoleId { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public List<int> DepartmentIds { get; set; } = new();
        public List<string> DepartmentNames { get; set; } = new();
        public string PrimaryDepartmentName => DepartmentNames.FirstOrDefault() ?? "Chưa phân phòng";
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }
    }

    public class CreateUserViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập họ và tên")]
        [StringLength(150)]
        [Display(Name = "Họ và tên")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ Email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [StringLength(150)]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự")]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng chọn vai trò")]
        [Display(Name = "Vai trò (Role)")]
        public int RoleId { get; set; }

        [Display(Name = "Phòng ban tham gia (Có thể chọn nhiều)")]
        public List<int> DepartmentIds { get; set; } = new();

        public List<Role> AvailableRoles { get; set; } = new();
        public List<Department> AvailableDepartments { get; set; } = new();
    }

    public class EditUserViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập họ và tên")]
        [StringLength(150)]
        [Display(Name = "Họ và tên")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ Email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [StringLength(150)]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng chọn vai trò")]
        [Display(Name = "Vai trò (Role)")]
        public int RoleId { get; set; }

        [Display(Name = "Phòng ban tham gia (Có thể chọn nhiều)")]
        public List<int> DepartmentIds { get; set; } = new();

        [Display(Name = "Trạng thái hoạt động")]
        public bool IsActive { get; set; } = true;

        public List<Role> AvailableRoles { get; set; } = new();
        public List<Department> AvailableDepartments { get; set; } = new();
    }

    public class ResetPasswordViewModel
    {
        public int UserId { get; set; }
        public string UserEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu mới")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự")]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu mới")]
        public string NewPassword { get; set; } = string.Empty;
    }
}
