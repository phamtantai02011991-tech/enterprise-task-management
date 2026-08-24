using day03.Models;

namespace day03.Repositories
{
    public interface IStudentRepository
    {
        // Lấy danh sách điểm
        IQueryable<StudentScore> GetScores();

        // Lấy điểm theo ID
        Task<StudentScore?> GetScoreByIdAsync(int scoreId);

        // Lấy danh sách môn học
        Task<List<Subject>> GetSubjectsAsync();

        // Thêm điểm
        Task AddScoreAsync(StudentScore score);

        // Cập nhật điểm
        void Update(StudentScore entity);

        // Lưu thay đổi
        Task SaveChangesAsync();
    }
}