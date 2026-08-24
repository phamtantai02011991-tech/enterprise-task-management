using day03.DTOs;
using day03.Models;
using day03.Repositories;
using Microsoft.EntityFrameworkCore;

namespace day03.Services
{
    public class StudentService : IStudentService
    {
        private readonly IStudentRepository repository;
        public StudentService(IStudentRepository repository)
        {
            this.repository = repository;
        }

        public async Task<List<Subject>> GetAllSubjectsAsync()
        {
            return await repository.GetAllSubjectsAsync();
        }

        public async Task<List<StudentScoreDto>> GetFilteredScoresAsync(StudentFilterDto filter)
        {
            var query = repository.GetScores();
            if (!string.IsNullOrWhiteSpace(filter.SearchKeyword))
            {
                var keyword = filter.SearchKeyword.Trim().ToLower();
                query = query.Where(s => s.StudentId!.ToLower().Contains(keyword) ||
                                    s.StudentName!.ToLower().Contains(keyword));
            }
            if (!string.IsNullOrWhiteSpace(filter.SubjectId))
            {
                query = query.Where(s => s.SubjectId == filter.SubjectId);
            }

            return await query.Select(s => new StudentScoreDto
            {
                ScoreId = s.ScoreId,
                StudentId = s.StudentId,
                StudentName = s.StudentName,
                SubjectId = s.SubjectId,
                SubjectName = s.Subject.SubjectName,
                Score = s.Score,

            }).ToListAsync();
        }

        public async Task<StudentScoreDto?> GetScoreForEditAsync(int scoreId)
        {
            var entity = await repository.GetScoreByIdAsync(scoreId);
            if (entity == null) return null;
            return new StudentScoreDto
            {
                ScoreId = entity.ScoreId,
                StudentId = entity.StudentId,
                StudentName = entity.StudentName,
                SubjectId = entity.SubjectId,
                SubjectName = entity.Subject.SubjectName,
                Score = entity.Score
            };
        }

        public async Task<bool> UpdateScoreAsync(StudentScoreDto dto)
        {
            if (dto.Score < 0 || dto.Score > 10) return false;
            var entity = await repository.GetScoreByIdAsync(dto.ScoreId);
            if (entity == null) return false;
            entity.Score = dto.Score;
            await repository.UpdateScoreAsync(entity);
            await repository.SaveChangesAsync();
            return true;
        }
    }
}
