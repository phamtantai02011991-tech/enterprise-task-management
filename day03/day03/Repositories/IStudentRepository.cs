using day03.Models;

namespace day03.Repositories
{
    public interface IStudentRepository
    {
        IQueryable<StudentScore> GetScores();
        Task<StudentScore?> GetScoreByIdAsync(int scoreId);
        Task<List<Subject>> GetAllSubjectsAsync();
        Task UpdateScoreAsync(StudentScore entity);
        Task SaveChangesAsync();
    }
}
