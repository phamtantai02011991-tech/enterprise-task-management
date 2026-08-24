using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PretestWDA.Models
{
    [Table("tbEmployee")]
    public class tbEmployee
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "EmployeeId")]
        public int EmpID { get; set; }

        [Required(ErrorMessage = "Date of birth is required.")]
        [Display(Name = "DateOfBirth")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime EmpDoB { get; set; } = new DateTime(1989, 1, 1);

        [Required(ErrorMessage = "Employee name is required.")]
        [StringLength(30, ErrorMessage = "Employee name cannot exceed 30 characters.")]
        [Display(Name = "EmployeeName")]
        public string EmpName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Address is required.")]
        [StringLength(30, ErrorMessage = "Address cannot exceed 30 characters.")]
        [Display(Name = "Address")]
        public string EmpAddress { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required.")]
        [StringLength(30, ErrorMessage = "Email cannot exceed 30 characters.")]
        [EmailAddress(ErrorMessage = "Invalid Email Address.")]
        [Display(Name = "Email")]
        public string EmpEmail { get; set; } = string.Empty;
    }
}
