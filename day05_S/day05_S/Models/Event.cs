using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace day05_S.Models
{
    [Table("Events")]
    public class Event
    {
        [key]
        public string EventId { get; set; } = null;

        [Required]

        public string EventName { get; set; }
        public int MaxCapacity { get; set; }
        public int CurrentCapacity { get; set; }
        public DateTime EventDate { get; set; }


    }
}
