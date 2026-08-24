using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManagementWeb.Models.Entities
{
    public class AuditLog
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        [MaxLength(150)]
        public string UserName { get; set; } = string.Empty;
        public int? ProjectId { get; set; }
        [Required]
        [MaxLength(50)]
        public string Action { get; set; } = string.Empty;
        [Required]
        [MaxLength(50)]
        public string EntityName { get; set; } = string.Empty;
        [Required]
        public string Details { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.Now;

        // Navigation
        [ForeignKey(nameof(UserId))]
        public User User { get; set; } = null!;
        [ForeignKey(nameof(ProjectId))]
        public Project? Project { get; set; }
    }
}