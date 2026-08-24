using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using TaskManagementWeb.Models.Entities;
using TaskManagementWeb.Models.Enums;

namespace TaskManagementWeb.ViewModels.Manager
{
    public class ProjectCreateUpdateViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Mã dự án không được để trống")]
        [StringLength(50, ErrorMessage = "Mã dự án không vượt quá 50 ký tự")]
        [Display(Name = "Mã dự án")]
        public string ProjectCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tên dự án không được để trống")]
        [StringLength(200, ErrorMessage = "Tên dự án không được vượt quá 200 ký tự")]
        [Display(Name = "Tên dự án")]
        public string Title { get; set; } = string.Empty;

        [Display(Name = "Mô tả dự án")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày bắt đầu")]
        [DataType(DataType.Date)]
        [Display(Name = "Ngày bắt đầu")]
        public DateTime StartDate { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Vui lòng chọn ngày kết thúc")]
        [DataType(DataType.Date)]
        [Display(Name = "Ngày kết thúc")]
        public DateTime EndDate { get; set; } = DateTime.Today.AddMonths(1);

        [Display(Name = "Trạng thái")]
        public ProjectStatus Status { get; set; } = ProjectStatus.Planning;

        [Display(Name = "Mức độ ưu tiên")]
        public ProjectPriority Priority { get; set; } = ProjectPriority.Medium;

        [Display(Name = "Trưởng dự án (Project Manager)")]
        public int? ManagerId { get; set; }

        [Display(Name = "Tiến độ hoàn thành (%)")]
        [Range(0, 100, ErrorMessage = "Tiến độ từ 0% đến 100%")]
        public int Progress { get; set; } = 0;

        public int CreatedByUserId { get; set; }

        [Display(Name = "Thành viên ban đầu")]
        public List<int> SelectedMemberIds { get; set; } = new List<int>();

        // Lists for dropdown / select
        public List<SelectListItem> AvailableManagers { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> AvailableEmployees { get; set; } = new List<SelectListItem>();
    }
}