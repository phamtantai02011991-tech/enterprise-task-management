using Microsoft.AspNetCore.Mvc.Rendering;

namespace day03.DTOs
{
    public class StudentViewModel
    {
        public StudentFilterDto Filter { get; set; } = new();
        public SelectList Subjects { get; set; } = null!;
        public List<StudentScoreDto> Scores { get; set; } = new();
    }
}
