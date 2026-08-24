using day03.Models;
using day03.Models.DTOs;
using day03.Repositories;
using Microsoft.EntityFrameworkCore;

namespace day03.Models.Services
{
    public class StudentService : IStudentService
    {
        private readonly IStudentRepository _repository;

        public StudentService(IStudentRepository repository)
        {
            _repository = repository;
        }

        // Danh sách điểm
        public async Task<List<StudentScoreDto>> GetScoresAsync(StudentFilterDto filter)
        {
            var query = _repository.GetScores();

            if (!string.IsNullOrWhiteSpace(filter.SearchKeyword))
            {
                var keyword = filter.SearchKeyword.Trim();

                query = query.Where(x =>
                    x.StudentId.Contains(keyword) ||
                    x.StudentName.Contains(keyword) ||
                    x.SubjectId.Contains(keyword));
            }

            if (!string.IsNullOrWhiteSpace(filter.SubjectId))
            {
                query = query.Where(x => x.SubjectId == filter.SubjectId);
            }

            return await query.Select(x => new StudentScoreDto
            {
                ScoreId = x.ScoreId,
                StudentId = x.StudentId,
                StudentName = x.StudentName,
                SubjectId = x.SubjectId,
                Score = x.Score
            }).ToListAsync();
        }

        // Lấy điểm theo ID
        public async Task<StudentScoreDto?> GetScoreByIdAsync(int scoreId)
        {
            var entity = await _repository.GetScoreByIdAsync(scoreId);

            if (entity == null)
                return null;

            return new StudentScoreDto
            {
                ScoreId = entity.ScoreId,
                StudentId = entity.StudentId,
                StudentName = entity.StudentName,
                SubjectId = entity.SubjectId,
                Score = entity.Score
            };
        }

        // Danh sách môn học
        public async Task<List<SubjectDto>> GetSubjectsAsync()
        {
            var list = await _repository.GetSubjectsAsync();

            return list.Select(x => new SubjectDto
            {
                SubjectId = x.SubjectId,
                SubjectName = x.SubjectName
            }).ToList();
        }

        // Thêm điểm
        public async Task AddScoreAsync(StudentScoreDto dto)
        {
            var entity = new StudentScore
            {
                StudentId = dto.StudentId,
                StudentName = dto.StudentName,
                SubjectId = dto.SubjectId,
                Score = dto.Score
            };

            await _repository.AddScoreAsync(entity);
            await _repository.SaveChangesAsync();
        }

        // Lấy dữ liệu Edit
        public async Task<StudentScoreDto?> GetStudentForEditAsync(int scoreId)
        {
            return await GetScoreByIdAsync(scoreId);
        }

        // Cập nhật điểm
        public async Task<bool> UpdateScoreAsync(StudentScoreDto dto)
        {
            if (dto.Score < 0 || dto.Score > 10)
                return false;

            var entity = await _repository.GetScoreByIdAsync(dto.ScoreId);

            if (entity == null)
                return false;

            entity.StudentId = dto.StudentId;
            entity.StudentName = dto.StudentName;
            entity.SubjectId = dto.SubjectId;
            entity.Score = dto.Score;

            _repository.Update(entity);

            await _repository.SaveChangesAsync();

            return true;
        }
    }
}