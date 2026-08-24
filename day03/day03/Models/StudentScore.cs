using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace day03.Models
{
    [Table("StudentScore")]
    public class StudentScore
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ScoreId { get; set; }
        [Required]
        [StringLength(10)]
        public string? StudentId { get; set; }
        [Required]
        [StringLength(100)]
        public string? StudentName { get; set; }
        [Required]
        [StringLength(10)]
        public string? SubjectId { get; set; }

        [Column(TypeName = "decimal(14, 2)")]
        public decimal Score { get; set; }

        [ForeignKey(nameof(SubjectId))]
        public Subject Subject { get; set; } = null!;
    }
}
