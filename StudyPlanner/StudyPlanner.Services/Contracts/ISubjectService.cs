using Microsoft.AspNetCore.Mvc.Rendering;
using StudyPlanner.Services.Core.Models.Subject;
using StudyPlanner.ViewModels.Subject;


namespace StudyPlanner.Services.Contracts
{
    public interface ISubjectService
    {
        Task CreateSubjectAsync(SubjectCreateDTO input, string userId);
        Task<List<SubjectDTO>> GetAllSubjectsAsync(string userId);  
        Task<SubjectDTO> GetSubjectByIdAsync(int id, string userId);
        Task UpdateSubjectAsync(SubjectEditDTO input, string userId); 
        Task DeleteSubjectAsync(int id, string userId);
        Task<List<SelectListItem>> GetSubjectsForDropdownAsync(string userId);

       Task<SubjectEditDTO> GetSubjectByIdAsyncForEdit(int id, string userId);

        Task<bool> SubjectExistsAsync(int subjectId, string userId);
    }
}
