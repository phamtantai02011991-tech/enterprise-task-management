using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace day03.Models
{
    [Table("Subject")]
    public class Subject
    {
        [Key]
        [StringLength(10)]
        public string SubjectId { get; set; } = null!;
        [Required]
        [StringLength(100)]
        public string SubjectName { get; set; } = null!;
        public ICollection<StudentScore> StudentScores { get; set; } = new List<StudentScore>();
    }
}
