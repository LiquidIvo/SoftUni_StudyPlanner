using StudyPlanner.Services.Core.Models.StudyTask;


namespace StudyPlanner.Services.Core.Contracts
{
    public interface IStudyTaskService
    {
        Task CreateTaskAsync(StudyTaskCreateDTO input, Guid userId);
        Task<(List<StudyTaskDTO> Items, int TotalCount)> GetAllTasksAsync(Guid userId, string? searchTerm, string? priority, int pageNumber, int pageSize);
        Task<StudyTaskDTO> GetTaskByIdAsync(int id, Guid userId);
        Task<(StudyTaskDetailsDTO Task, int TotalSessions)> GetDetailedStudyTaskByIdAsync(int id, Guid userId, int pageNumber, int pageSize);
        Task<StudyTaskDetailsDTO> GetDetailedStudyTaskForPDF (int id, Guid userId);
        Task<StudyTaskEditDTO> GetStudyTaskForEditByIdAsync(int id, Guid userId);
        Task UpdateTaskAsync(StudyTaskEditDTO input, Guid userId);
        Task DeleteTaskAsync(int id, Guid userId);

        
    }
}
