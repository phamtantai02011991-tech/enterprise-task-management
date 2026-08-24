using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace day03.Models.DTOs
{
    public class StudentFilterDto
    {
        public string? SubjectId { get; set; }

        public string? StudentId { get; set; }

        public string? StudentName { get; set; }

        //sreach điểm thiếu SearchKeyward
        public string? SearchKeyword { get; set; }
    }
}
