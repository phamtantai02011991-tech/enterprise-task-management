using day03.Data;
using day03.Models;
using Microsoft.EntityFrameworkCore;

namespace day03.Repositories
{
    public class StudentRepository : IStudentRepository
    {
        private readonly ApplicationDbContext context;
        public StudentRepository(ApplicationDbContext context) { 
                this.context = context;
        }
        public async Task<List<Subject>> GetAllSubjectsAsync()
        {
            return await context.Subjects.AsNoTracking().ToListAsync();
        }

        public async Task<StudentScore?> GetScoreByIdAsync(int scoreId)
        {
            return await context.StudentScores.Include(s => s.Subject)
                                 .FirstOrDefaultAsync(s => s.ScoreId == scoreId);
        }


        public IQueryable<StudentScore> GetScores()
        {
            return context.StudentScores.Include(s => s.Subject).AsNoTracking();
        }

        public Task UpdateScoreAsync(StudentScore entity)
        {
            context.StudentScores.Update(entity);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await context.SaveChangesAsync();
        }
    }
}
