using StudyPlanner.Services.Core.Models.StudyTask;


namespace StudyPlanner.Services.Core.Contracts
{
    public interface IStudyTaskService
    {
        Task CreateTaskAsync(StudyTaskCreateDTO input, string userId);
        Task<List<StudyTaskDTO>> GetAllTasksAsync(string userId);  
        Task<StudyTaskDTO> GetTaskByIdAsync(int id, string userId);
        Task<StudyTaskDetailsDTO> GetDetailedStudyTaskByIdAsync(int id, string userId);
        Task<StudyTaskEditDTO> GetStudyTaskForEditByIdAsync(int id, string userId);
        Task UpdateTaskAsync(StudyTaskEditDTO input, string userId);
        Task DeleteTaskAsync(int id, string userId);

        
    }
}
