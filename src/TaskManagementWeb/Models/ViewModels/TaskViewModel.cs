using System.ComponentModel.DataAnnotations;
using TaskManagementWeb.Models.Enums;

namespace TaskManagementWeb.Models.ViewModels
{
    public class TaskViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public TaskPriority Priority { get; set; } = TaskPriority.Medium;
        public TaskStatusEnum Status { get; set; } = TaskStatusEnum.Pending;
        [Required(ErrorMessage = "Deadline is required")]
        public DateTime Deadline { get; set; } = DateTime.Today.AddDays(7);

        [Required(ErrorMessage = "Please choose project")]
        public int ProjectId { get; set; }
        public string? ProjectTitle { get; set; }
        public int? AssignedUserId { get; set; }
        public string? AssignedUserName { get; set; }
    }
}
