using System.ComponentModel.DataAnnotations;

namespace TaskManagementWeb.Models.Entities
{
    public class Role
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string RoleName { get; set; } = string.Empty;

        // Navigation
        public ICollection<User> Users { get; set; } = new List<User>();
    }
}