using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManagementWeb.Models.Entities
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(150)]
        public string FullName { get; set; } = string.Empty;
        [Required]
        [MaxLength(150)]
        public string Email { get; set; } = string.Empty;
        [Required]
        [MaxLength(255)]
        public string PasswordHash { get; set; } = string.Empty;
        public int RoleId { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation
        [ForeignKey(nameof(RoleId))]
        public Role Role { get; set; } = null!;

        public ICollection<UserDepartment> UserDepartments { get; set; }
            = new List<UserDepartment>();
        public ICollection<Project> CreatedProjects { get; set; }
            = new List<Project>();
        public ICollection<ProjectMember> ProjectMembers { get; set; }
            = new List<ProjectMember>();
        public ICollection<TaskItem> AssignedTasks { get; set; }
            = new List<TaskItem>();
        public ICollection<TimeLog> TimeLogs { get; set; }
            = new List<TimeLog>();
        public ICollection<Notification> Notifications { get; set; }
            = new List<Notification>();
        public ICollection<ChatMessage> SentMessages { get; set; }
            = new List<ChatMessage>();
        public ICollection<ChatMessage> ReceivedMessages { get; set; }
            = new List<ChatMessage>();
        public ICollection<AuditLog> AuditLogs { get; set; }
            = new List<AuditLog>();
    }
}