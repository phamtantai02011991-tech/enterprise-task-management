using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace day05_S.Models
{
    [Table("EventRegistrations")]
    public class EventRegistraion
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RegId { get; set; }

        [Required]
        [Column(TypeName = "varchar(50)")]
        public string? UserId { get; set; }

        public string? EventId { get; set; }

        public DateTime RegTime { get; set; }

        [ForeignKey("EventId")]
        public virtual Event? Event { get; set; }
    }
}
