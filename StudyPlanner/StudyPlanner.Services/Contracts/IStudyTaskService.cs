using StudyPlanner.Services.Core.Models.StudyTask;


namespace StudyPlanner.Services.Core.Contracts
{
    public interface IStudyTaskService
    {
        Task CreateTaskAsync(StudyTaskCreateDTO input, Guid userId);
        Task<List<StudyTaskDTO>> GetAllTasksAsync(Guid userId);  
        Task<StudyTaskDTO> GetTaskByIdAsync(int id, Guid userId);
        Task<StudyTaskDetailsDTO> GetDetailedStudyTaskByIdAsync(int id, Guid userId);
        Task<StudyTaskEditDTO> GetStudyTaskForEditByIdAsync(int id, Guid userId);
        Task UpdateTaskAsync(StudyTaskEditDTO input, Guid userId);
        Task DeleteTaskAsync(int id, Guid userId);

        
    }
}
