using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManagementWeb.Models.Entities
{
    public class TimeLog
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int TaskItemId { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        public float HoursSpent { get; set; }
        public DateTime DateLogged { get; set; } = DateTime.Now;
        public string? Description { get; set; }
        // Navigation
        [ForeignKey(nameof(TaskItemId))]
        public TaskItem TaskItem { get; set; } = null!;
        [ForeignKey(nameof(UserId))]
        public User User { get; set; } = null!;
    }
}