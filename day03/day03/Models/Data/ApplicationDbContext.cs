using day03.Models;
using Microsoft.EntityFrameworkCore;

namespace day03.Models.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Đổi thành số nhiều "Subjects" để map đúng chuẩn với bảng trong DB
        public DbSet<Subject> Subjects { get; set; }

        public DbSet<StudentScore> StudentScores { get; set; }
        public object Subject { get; internal set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed Subject
            modelBuilder.Entity<Subject>().HasData(
                new Subject { SubjectId = "SUB01", SubjectName = "C#" },
                new Subject { SubjectId = "SUB02", SubjectName = "ASP.NET Core" },
                new Subject { SubjectId = "SUB03", SubjectName = "SQL Server" }
            );

            // Seed StudentScore - Sửa SubjectId thành "SUB01", "SUB02", "SUB03" cho khớp phía trên
            modelBuilder.Entity<StudentScore>().HasData(
                new StudentScore
                {
                    ScoreId = 1,
                    StudentId = "SV001",
                    StudentName = "Nguyễn Văn A",
                    SubjectId = "SUB01", // Sửa từ SUB001 thành SUB01
                    Score = 8.5m
                },
                new StudentScore
                {
                    ScoreId = 2,
                    StudentId = "SV002",
                    StudentName = "Trần Thị B",
                    SubjectId = "SUB02", // Sửa từ SUB002 thành SUB02
                    Score = 9.0m
                },
                new StudentScore
                {
                    ScoreId = 3,
                    StudentId = "SV003",
                    StudentName = "Lê Văn C",
                    SubjectId = "SUB03", // Sửa từ SUB003 thành SUB03
                    Score = 7.5m
                }
            );
        }
    }
}