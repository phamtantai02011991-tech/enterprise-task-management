using System.ComponentModel.DataAnnotations;

namespace TaskManagementWeb.Models.Entities
{
    public class Department
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(150)]
        public string Name { get; set; } = string.Empty;
        [Required]
        [MaxLength(50)]
        public string Code { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
        // Navigation
        public ICollection<UserDepartment> UserDepartments { get; set; } = new List<UserDepartment>();
    }
}