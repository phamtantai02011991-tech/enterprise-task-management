using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManagementWeb.Models.Entities
{
    public class ProjectFile
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int ProjectId { get; set; }
        [Required]
        public int UploadedByUserId { get; set; }
        [Required]
        [MaxLength(255)]
        public string FileName { get; set; } = string.Empty;
        [Required]
        [MaxLength(500)]
        public string FilePath { get; set; } = string.Empty;
        [Required]
        public long FileSize { get; set; }
        public DateTime UploadedAt { get; set; } = DateTime.Now;
        // Navigation
        [ForeignKey(nameof(ProjectId))]
        public Project Project { get; set; } = null!;
        [ForeignKey(nameof(UploadedByUserId))]
        public User UploadedByUser { get; set; } = null!;
    }
}