using System.ComponentModel.DataAnnotations;

namespace TaskManagementWeb.Models.ViewModels.Admin
{
    public class DepartmentListViewModel
    {
        public List<DepartmentItemViewModel> Departments { get; set; } = new();
        public string? SearchKey { get; set; }
    }

    public class DepartmentItemViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int MemberCount { get; set; }
    }

    public class UserSelectionDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
        public bool IsSelected { get; set; }
    }

    public class DepartmentFormViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên phòng ban")]
        [StringLength(150)]
        [Display(Name = "Tên phòng ban")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Mã phòng ban")]
        [StringLength(50)]
        public string? Code { get; set; }

        [Display(Name = "Mô tả")]
        public string? Description { get; set; }

        public int MemberCount { get; set; }

        [Display(Name = "Nhân sự thuộc phòng ban")]
        public List<int> SelectedUserIds { get; set; } = new();

        public List<UserSelectionDto> AvailableUsers { get; set; } = new();
    }
}
