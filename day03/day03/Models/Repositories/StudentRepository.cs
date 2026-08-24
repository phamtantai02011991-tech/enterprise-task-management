using day03.Models;
using day03.Models.Data;
using day03.Repositories;
using Microsoft.EntityFrameworkCore;

namespace day03.Models.Repositories
{
    public class StudentRepository : IStudentRepository
    {
        private readonly ApplicationDbContext _context;

        public StudentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Lấy danh sách điểm
        public IQueryable<StudentScore> GetScores()
        {
            return _context.StudentScores
                .Include(x => x.Subject)
                .AsNoTracking();
        }

        // Lấy điểm theo ID
        public async Task<StudentScore?> GetScoreByIdAsync(int scoreId)
        {
            return await _context.StudentScores
                .Include(x => x.Subject)
                .FirstOrDefaultAsync(x => x.ScoreId == scoreId);
        }

        // Lấy danh sách môn học
        public async Task<List<Subject>> GetSubjectsAsync()
        {
            return await _context.Subjects.ToListAsync();
        }

        // Thêm điểm
        public async Task AddScoreAsync(StudentScore score)
        {
            await _context.StudentScores.AddAsync(score);
        }

        // Cập nhật
        public void Update(StudentScore entity)
        {
            _context.StudentScores.Update(entity);
        }

        // Lưu thay đổi
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}