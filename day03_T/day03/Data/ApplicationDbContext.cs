using day03.Models;
using Microsoft.EntityFrameworkCore;

namespace day03.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options){  }
        public DbSet<Subject> Subjects => Set<Subject>();
        public DbSet<StudentScore> StudentScores => Set<StudentScore>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Subject>(entity =>
            {
                entity.ToTable("Subject");
                entity.HasKey(e => e.SubjectId);
            });

            modelBuilder.Entity<StudentScore>(entity =>
            {
                entity.ToTable("StudentScore");
                entity.HasKey(e => e.ScoreId);
                entity.HasOne(d => d.Subject)
                      .WithMany(p => p.StudentScores)
                      .HasForeignKey(d => d.SubjectId);
            });

            modelBuilder.Entity<Subject>().HasData(
                new Subject { SubjectId = "SUB001",SubjectName = "C# Programming"},
                 new Subject { SubjectId = "SUB002", SubjectName = "SQL Server Database" },
                  new Subject { SubjectId = "SUB003", SubjectName = "ASP.NET Core MVC Web App" }
             );

            modelBuilder.Entity<StudentScore>().HasData(
                new StudentScore { ScoreId = 1, StudentId = "STD001",StudentName = "Alex Tran", SubjectId = "SUB001", Score = 8.00m },
                new StudentScore { ScoreId = 2, StudentId = "STD001", StudentName = "Alex Tran", SubjectId = "SUB002", Score = 7.50m },
                new StudentScore { ScoreId = 3, StudentId = "STD002", StudentName = "Tai Pham", SubjectId = "SUB001", Score = 9.00m },
                new StudentScore { ScoreId = 4, StudentId = "STD003", StudentName = "Alice Nguyen", SubjectId = "SUB003", Score = 4.00m },
                new StudentScore { ScoreId = 5, StudentId = "STD003", StudentName = "Alice Nguyen", SubjectId = "SUB002", Score = 7.00m }
             );
        }
    }
}
