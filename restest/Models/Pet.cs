using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace restest.Models
{
    [Table("Pet")]
    public class Pet
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Pet ID")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Pet name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Pet name must be between 2 and 100 characters")]
        [Display(Name = "Pet Name")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Category / Breed is required")]
        [StringLength(50, ErrorMessage = "Category cannot exceed 50 characters")]
        [Display(Name = "Category / Breed")]
        public string? Category { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0, 100000000, ErrorMessage = "Price must be between 0 and 100,000,000")]
        [Display(Name = "Price ($)")]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Age is required")]
        [Range(0, 50, ErrorMessage = "Age must be between 0 and 50")]
        [Display(Name = "Age (Years)")]
        public int Age { get; set; }

        [Required(ErrorMessage = "Health status is required")]
        [Display(Name = "Health Status")]
        public PetStatus Status { get; set; } = PetStatus.Healthy;

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; } = true;

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        [Display(Name = "Description")]
        public string? Description { get; set; }

        [Display(Name = "Pet Image")]
        public string? ImagePath { get; set; }
    }
}
