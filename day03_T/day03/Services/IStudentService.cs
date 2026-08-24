using day03.DTOs;
using day03.Models;

namespace day03.Services
{
    public interface IStudentService
    {
        Task<List<StudentScoreDto>> GetFilteredScoresAsync(StudentFilterDto filter);
        Task<StudentScoreDto?> GetScoreForEditAsync(int scoreId);
        Task<bool> UpdateScoreAsync(StudentScoreDto dto);
        Task<List<Subject>> GetAllSubjectsAsync();
    }
}
