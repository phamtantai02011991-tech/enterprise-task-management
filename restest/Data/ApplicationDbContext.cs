using Microsoft.EntityFrameworkCore;
using restest.Models;

namespace restest.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Pet> Pets { get; set; }

        public static void Initialize(ApplicationDbContext context)
        {
            context.Database.EnsureCreated();

            if (context.Pets.Any())
            {
                return; // DB already seeded
            }

            var pets = new List<Pet>
            {
                new Pet
                {
                    Name = "Golden Retriever Puppy",
                    Category = "Dog",
                    Price = 1200.00m,
                    Age = 1,
                    Status = PetStatus.Healthy,
                    IsActive = true,
                    Description = "Friendly, energetic, and great with families with children.",
                    ImagePath = null
                },
                new Pet
                {
                    Name = "British Shorthair",
                    Category = "Cat",
                    Price = 950.00m,
                    Age = 2,
                    Status = PetStatus.Vaccinated,
                    IsActive = true,
                    Description = "Plush grey coat with copper eyes, calm and quiet temperament.",
                    ImagePath = null
                },
                new Pet
                {
                    Name = "Siberian Husky",
                    Category = "Dog",
                    Price = 1450.00m,
                    Age = 3,
                    Status = PetStatus.Healthy,
                    IsActive = true,
                    Description = "Striking blue eyes, high stamina, friendly and outgoing.",
                    ImagePath = null
                },
                new Pet
                {
                    Name = "Blue & Gold Macaw",
                    Category = "Bird",
                    Price = 2200.00m,
                    Age = 4,
                    Status = PetStatus.Healthy,
                    IsActive = true,
                    Description = "Vibrant feathers, intelligent and capable of mimicking words.",
                    ImagePath = null
                },
                new Pet
                {
                    Name = "Holland Lop Rabbit",
                    Category = "Rabbit",
                    Price = 250.00m,
                    Age = 1,
                    Status = PetStatus.Recovering,
                    IsActive = true,
                    Description = "Adorable lop ears, sweet disposition, recovering from minor ear checkup.",
                    ImagePath = null
                },
                new Pet
                {
                    Name = "Persian Cat",
                    Category = "Cat",
                    Price = 800.00m,
                    Age = 5,
                    Status = PetStatus.UnderTreatment,
                    IsActive = false,
                    Description = "Long luxurious fur, currently under routine dental treatment.",
                    ImagePath = null
                },
                new Pet
                {
                    Name = "Syrian Hamster",
                    Category = "Hamster",
                    Price = 45.00m,
                    Age = 1,
                    Status = PetStatus.Healthy,
                    IsActive = true,
                    Description = "Cute golden coat, active nocturnal runner.",
                    ImagePath = null
                }
            };

            context.Pets.AddRange(pets);
            context.SaveChanges();
        }
    }
}
