using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManagementWeb.Models.Entities
{
    public class ChatMessage
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int SenderId { get; set; }
        public int? ReceiverId { get; set; }
        [Required]
        public string MessageText { get; set; } = string.Empty;
        public DateTime SentAt { get; set; } = DateTime.Now;
        public bool IsRead { get; set; } = false;
        // Navigation
        [ForeignKey(nameof(SenderId))]
        public User Sender { get; set; } = null!;
        [ForeignKey(nameof(ReceiverId))]
        public User? Receiver { get; set; }
    }
}