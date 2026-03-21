using Microsoft.AspNetCore.Mvc.Rendering;
using StudyPlanner.Services.Core.Models.Subject;


namespace StudyPlanner.Services.Core.Contracts
{
    public interface ISubjectService
    {
        Task CreateSubjectAsync(SubjectCreateDTO input, Guid userId);
        Task<(List<SubjectDTO> Items,int TotalCount)> GetAllSubjectsAsync(Guid userId, string? searchTerm, int pageNumber, int pageSize);  
        Task<SubjectDTO> GetSubjectByIdAsync(int id, Guid userId);
        Task UpdateSubjectAsync(SubjectEditDTO input, Guid userId); 
        Task DeleteSubjectAsync(int id, Guid userId);
        Task<List<SelectListItem>> GetSubjectsForDropdownAsync(Guid userId);

       Task<SubjectEditDTO> GetSubjectByIdAsyncForEdit(int id, Guid userId);

        Task<bool> SubjectExistsAsync(int subjectId, Guid userId);
    }
}
