using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace TaskManagementWeb.Models.Entities
{
    public class NotificationPreference
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        public bool InAppOnTaskAssign { get; set; } = true;

        public bool InAppOnDeadline { get; set; } = true;

        public bool InAppOnTaskCompleted { get; set; } = true;

        public bool InAppOnProjectAdded { get; set; } = true;

        public bool EmailAlertsEnabled { get; set; } = true;

        // Navigation
        [ForeignKey(nameof(UserId))]
        [ValidateNever]
        public User? User { get; set; }
    }
}
