using Microsoft.AspNetCore.Mvc.Rendering;
using StudyPlanner.Data.Models;
using StudyPlanner.ViewModels.Subject;


namespace StudyPlanner.Services.Contracts
{
    public interface ISubjectService
    {
        Task CreateSubjectAsync(SubjectCreateInputModel input, string userId);
        Task<List<SubjectViewModel>> GetAllSubjectsAsync(string userId);  
        Task<SubjectViewModel> GetSubjectByIdAsync(int id, string userId);
        Task UpdateSubjectAsync(SubjectEditInputModel input, string userId); 
        Task DeleteSubjectAsync(int id, string userId);
        Task<List<SelectListItem>> GetSubjectsForDropdownAsync(string userId);

       Task<SubjectEditInputModel> GetSubjectByIdAsyncForEdit(int id, string userId);

        Task<bool> SubjectExistsAsync(int subjectId, string userId);
    }
}
