using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManagementWeb.Models.Entities
{
    public class ProjectMember
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ProjectId { get; set; }

        [Required]
        public int UserId { get; set; }

        [MaxLength(50)]
        public string RoleInProject { get; set; } = "Member"; // Member, Tech Lead, Developer, QA, Designer,...

        public DateTime AssignedAt { get; set; } = DateTime.Now;

        // Navigation
        [ForeignKey(nameof(ProjectId))]
        public Project Project { get; set; } = null!;

        [ForeignKey(nameof(UserId))]
        public User User { get; set; } = null!;
    }
}