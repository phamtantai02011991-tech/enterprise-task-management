using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace day03.DTOs
{
    public class StudentScoreDto
    {
        public int ScoreId { get; set; }
        public string? StudentId { get; set; }
        public string? StudentName { get; set; }
        public string? SubjectId { get; set; }
        public string? SubjectName { get; set; }

        [Required(ErrorMessage = "Score is required")]
        [Range(0, 10, ErrorMessage = "Score from 0 to 10")]
        public decimal Score { get; set; }


    }
}
