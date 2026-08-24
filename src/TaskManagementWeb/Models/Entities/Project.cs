using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TaskManagementWeb.Models.Enums;

namespace TaskManagementWeb.Models.Entities
{
    public class Project
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string ProjectCode { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public ProjectStatus Status { get; set; } = ProjectStatus.Planning;

        public ProjectPriority Priority { get; set; } = ProjectPriority.Medium;

        public int? ManagerId { get; set; }

        public int Progress { get; set; } = 0; // % tiến độ (0 - 100)

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; } = DateTime.Now;

        public bool IsArchived { get; set; } = false;

        public DateTime? ArchivedAt { get; set; }

        [Required]
        public int CreatedByUserId { get; set; }

        // Navigation
        [ForeignKey(nameof(CreatedByUserId))]
        public User CreatedByUser { get; set; } = null!;

        [ForeignKey(nameof(ManagerId))]
        public User? ManagerUser { get; set; }

        public ICollection<ProjectMember> ProjectMembers { get; set; }
            = new List<ProjectMember>();

        public ICollection<ProjectFile> ProjectFiles { get; set; }
            = new List<ProjectFile>();

        public ICollection<TaskItem> Tasks { get; set; }
            = new List<TaskItem>();

        public ICollection<AuditLog> AuditLogs { get; set; }
            = new List<AuditLog>();
    }
}