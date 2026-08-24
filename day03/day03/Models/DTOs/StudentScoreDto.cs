using System.ComponentModel.DataAnnotations;

namespace day03.Models.DTOs
{
    public class StudentScoreDto
    {
        public int ScoreId { get; set; }

        public string? StudentId { get; set; }

        public string? StudentName { get; set; }

        public string? SubjectId { get; set; }

        // 👉 Thêm dòng này để sửa Lỗi 4
        public string? SubjectName { get; set; }

        [Required(ErrorMessage = "Score is required")]
        public decimal Score { get; set; }
    }
}