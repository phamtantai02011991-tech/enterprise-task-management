using Microsoft.EntityFrameworkCore;
using PretestWDA.Models;

namespace PretestWDA.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<tbEmployee> tbEmployees { get; set; }
    }
}
