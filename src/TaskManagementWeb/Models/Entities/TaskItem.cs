using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TaskManagementWeb.Models.Enums;

namespace TaskManagementWeb.Models.Entities
{
    public class TaskItem
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public TaskPriority Priority { get; set; } = TaskPriority.Medium;
        public TaskStatusEnum Status { get; set; } = TaskStatusEnum.Pending;
        [Required]
        public DateTime Deadline { get; set; }
        [Required]
        public int ProjectId { get; set; }
        public int? AssignedUserId { get; set; }
        public bool IsActive { get; set; } = true;

        // Navigation
        [ForeignKey(nameof(ProjectId))]
        public Project Project { get; set; } = null!;
        [ForeignKey(nameof(AssignedUserId))]
        public User? AssignedUser { get; set; }
        public ICollection<TimeLog> TimeLogs { get; set; }
            = new List<TimeLog>();
    }
}