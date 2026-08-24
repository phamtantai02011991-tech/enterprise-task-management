using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace TaskManagementWeb.Models.Entities
{
    public class Announcement
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Tiêu đề thông báo không được để trống")]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Nội dung thông báo không được để trống")]
        public string Content { get; set; } = string.Empty;

        [MaxLength(30)]
        public string Type { get; set; } = "Info"; // Info, Warning, Urgent, Success

        public bool IsActive { get; set; } = true;

        public bool IsPinned { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? ExpiryDate { get; set; }

        public int CreatedByUserId { get; set; }

        // Navigation
        [ForeignKey(nameof(CreatedByUserId))]
        [ValidateNever]
        public User? CreatedByUser { get; set; }
    }
}
