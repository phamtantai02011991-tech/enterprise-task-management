using System.ComponentModel.DataAnnotations;

namespace day02.Models
{
    public class Student
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public int Age { get; set; }

        [Required]
        [EmailAddress]
        public string Gmail { get; set; }

        public string Phone { get; set; }

        public double Mark { get; set; }
    }
}