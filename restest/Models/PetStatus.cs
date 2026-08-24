using System.ComponentModel.DataAnnotations;

namespace restest.Models
{
    public enum PetStatus
    {
        [Display(Name = "Healthy")]
        Healthy = 1,

        [Display(Name = "Sick")]
        Sick = 2,

        [Display(Name = "Recovering")]
        Recovering = 3,

        [Display(Name = "Vaccinated")]
        Vaccinated = 4,

        [Display(Name = "Under Treatment")]
        UnderTreatment = 5
    }
}
