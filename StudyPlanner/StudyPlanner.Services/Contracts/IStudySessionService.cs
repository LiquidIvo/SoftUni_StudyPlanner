using StudyPlanner.ViewModels.StudySession;
using StudyPlanner.ViewModels.Subject;

namespace StudyPlanner.Services.Core.Contracts
{
    public interface IStudySessionService
    {
        Task CreateStudySessionAsync(StudySessionCreateInputModel input, int studyTaskId, string userId);
        Task<List<StudySessionViewModel>> GetAllStudySessionsAsync(string userId);
        Task<StudySessionViewModel> GetStudySessionByIdAsync(int id, string userId);
        Task UpdateStudySessionAsync(StudySessionEditInputModel input, string userId);
        Task<int> DeleteStudySessionAsync(int id, string userId);
        Task<StudySessionEditInputModel> GetStudySessionByIdAsyncForEdit(int id, string userId);

        Task CheckTaskOwnershipAsync(int taskId, string userId);
    }
}
