using Microsoft.AspNetCore.Mvc.Rendering;

namespace day03.Models.DTOs
{
    public class StudentViewModel
    {
        public StudentFilterDto Filter { get; set; } = new();
        public SelectList Subjects { get; set; } = null;
        public List<StudentScore> Students { get; set; }
        public object Scores { get; internal set; }
    }
}
