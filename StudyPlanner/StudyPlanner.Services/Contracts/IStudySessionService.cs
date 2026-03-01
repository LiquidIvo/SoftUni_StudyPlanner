using StudyPlanner.Services.Core.Models.StudySession;

namespace StudyPlanner.Services.Core.Contracts
{
    public interface IStudySessionService
    {
        Task CreateStudySessionAsync(StudySessionCreateDTO input, int studyTaskId, string userId);
        Task<List<StudySessionDTO>> GetAllStudySessionsAsync(string userId);
        Task<StudySessionDTO> GetStudySessionByIdAsync(int id, string userId);
        Task UpdateStudySessionAsync(StudySessionEditDTO input, string userId);
        Task<int> DeleteStudySessionAsync(int id, string userId);
        Task<StudySessionEditDTO> GetStudySessionByIdAsyncForEdit(int id, string userId);

        Task CheckTaskOwnershipAsync(int taskId, string userId);
    }
}
