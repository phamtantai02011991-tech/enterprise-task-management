using day02.Models;
using Microsoft.EntityFrameworkCore;

namespace day02.Data
{
    public class ApplicationDbConnection : DbContext
    {
        public ApplicationDbConnection(
            DbContextOptions<ApplicationDbConnection> options)
            : base(options)
        {
        }

        public DbSet<Student> Students { get; set; }
    }
}