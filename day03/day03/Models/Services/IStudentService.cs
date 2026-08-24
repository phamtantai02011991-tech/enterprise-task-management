using day03.Models.DTOs;

namespace day03.Models.Services
{
    public interface IStudentService
    {
        // Danh sách điểm
        Task<List<StudentScoreDto>> GetScoresAsync(StudentFilterDto filter);

        // Lấy điểm theo Id
        Task<StudentScoreDto?> GetScoreByIdAsync(int scoreId);

        // Danh sách môn học
        Task<List<SubjectDto>> GetSubjectsAsync();

        // Thêm điểm
        Task AddScoreAsync(StudentScoreDto dto);

        // Lấy dữ liệu để Edit
        Task<StudentScoreDto?> GetStudentForEditAsync(int scoreId);

        // Cập nhật
        Task<bool> UpdateScoreAsync(StudentScoreDto dto);
    }
}