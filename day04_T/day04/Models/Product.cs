using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace day04.Models
{
    [Table("Product")]
    public class Product
    {
        public int Id { get; set; }
        [Required(ErrorMessage ="Name is required")]
        [StringLength(100,MinimumLength =2, ErrorMessage ="Name from 2 to 100")]
        public string? Name { get; set; }
        [Required(ErrorMessage = "Category is required")]
        [StringLength(50,ErrorMessage = "Category max 50 characters")]
        public string? Category { get; set; }

        [Range(1000,100000000,ErrorMessage = "Price from 1,000 to 100,000,000 VND")]
        public decimal Price { get; set; }
        [Range(1,100,ErrorMessage = "Quantity from 1 to 100")]
        public int Quantity { get; set; }

        public string? ImagePath { get; set; }
    }
}
