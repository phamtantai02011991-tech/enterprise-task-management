using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManagementWeb.Models.Entities
{
    public enum MemberReportLevel
    {
        Reprimand = 1, // Khiển trách / Cảnh cáo (Không hoàn thành task)
        Expulsion = 2  // Khai trừ khỏi dự án (Không hoàn thành nhiều lần / vi phạm nặng)
    }

    public enum MemberReportStatus
    {
        Pending = 0,   // Chờ Admin duyệt
        Approved = 1,  // Admin đã phê duyệt
        Rejected = 2   // Admin đã bác bỏ
    }

    public class ProjectMemberReport
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ProjectId { get; set; }

        [Required]
        public int ReporterManagerId { get; set; } // Manager gửi báo cáo

        [Required]
        public int TargetUserId { get; set; }      // Nhân sự bị báo cáo

        [Required]
        public MemberReportLevel Level { get; set; } = MemberReportLevel.Reprimand;

        [Required]
        [MaxLength(1000)]
        public string Reason { get; set; } = string.Empty; // Lý do chi tiết

        public MemberReportStatus Status { get; set; } = MemberReportStatus.Pending;

        [MaxLength(500)]
        public string? AdminComment { get; set; } // Phản hồi của Admin khi duyệt/bác bỏ

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? ResolvedAt { get; set; }

        // Navigation
        [ForeignKey(nameof(ProjectId))]
        public Project Project { get; set; } = null!;

        [ForeignKey(nameof(ReporterManagerId))]
        public User ReporterManager { get; set; } = null!;

        [ForeignKey(nameof(TargetUserId))]
        public User TargetUser { get; set; } = null!;
    }
}
