using StudyPlanner.Services.Core.Models.StudySession;

namespace StudyPlanner.Services.Core.Contracts
{
    public interface IStudySessionService
    {
        Task CreateStudySessionAsync(StudySessionCreateDTO input, int studyTaskId, Guid userId);
        Task<List<StudySessionDTO>> GetAllStudySessionsAsync(Guid userId);
        Task<StudySessionDTO> GetStudySessionByIdAsync(int id, Guid userId);
        Task UpdateStudySessionAsync(StudySessionEditDTO input, Guid userId);
        Task<int> DeleteStudySessionAsync(int id, Guid userId);
        Task<StudySessionEditDTO> GetStudySessionByIdAsyncForEdit(int id, Guid userId);

        Task CheckTaskOwnershipAsync(int taskId, Guid userId);
    }
}
