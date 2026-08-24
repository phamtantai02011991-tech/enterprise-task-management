using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace TaskManagementWeb.ViewModels.Manager
{
    public class TaskCreateUpdateViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Tiêu đề công việc không được để trống")]
        [StringLength(150)]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn độ ưu tiên")]
        public string Priority { get; set; } = "Medium"; // Low, Medium, High

        [Required(ErrorMessage = "Vui lòng chọn trạng thái")]
        public string Status { get; set; } = "Todo"; // Todo, InProgress, Completed

        [Required(ErrorMessage = "Vui lòng chọn hạn chót")]
        [DataType(DataType.DateTime)]
        public DateTime Deadline { get; set; }

        public int ProjectId { get; set; }

        [Display(Name = "Phân công cho nhân sự")]
        public int? AssignedUserId { get; set; }

        // Danh sách nhân sự trong dự án phục vụ cho thẻ <select> trên giao diện
        public SelectList? EmployeeList { get; set; }
    }
}