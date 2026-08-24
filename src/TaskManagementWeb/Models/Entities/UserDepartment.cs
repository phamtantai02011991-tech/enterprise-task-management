using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManagementWeb.Models.Entities
{
    public class UserDepartment
    {
        public int UserId { get; set; }
        public int DepartmentId { get; set; }

        public bool IsPrimary { get; set; } = false;
        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey(nameof(UserId))]
        public User User { get; set; } = null!;

        [ForeignKey(nameof(DepartmentId))]
        public Department Department { get; set; } = null!;
    }
}
