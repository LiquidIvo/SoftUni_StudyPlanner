using StudyPlanner.ViewModels.StudySession;
using StudyPlanner.ViewModels.StudyTask;


namespace StudyPlanner.Services.Contracts
{
    public interface IStudyTaskService
    {
        Task CreateTaskAsync(StudyTaskCreateInputModel input, string userId);
        Task<List<StudyTaskViewModel>> GetAllTasksAsync(string userId);  
        Task<StudyTaskViewModel> GetTaskByIdAsync(int id, string userId);
        Task<StudyTaskDetailsViewModel> GetDetailedStudyTaskByIdAsync(int id, string userId);
        Task<StudyTaskEditInputModel> GetStudyTaskForEditByIdAsync(int id, string userId);
        Task UpdateTaskAsync(StudyTaskEditInputModel input, string userId);
        Task DeleteTaskAsync(int id, string userId);

        
    }
}
