using Microsoft.AspNetCore.Mvc.Rendering;
using StudyPlanner.Services.Core.Models.Category;


namespace StudyPlanner.Services.Core.Contracts
{
    public interface ICategoryService
    {
        Task CreateCategoryAsync(CategoryCreateDTO input, Guid userId);
        Task<List<CategoryDTO>> GetAllCategoriesAsync(Guid userId); 
        Task<CategoryDTO> GetCategoryByIdAsync(int id, Guid userId);  

        Task<CategoryEditDTO> GetCategoryByIdAsyncForEdit(int id, Guid userId);    
        Task UpdateCategoryAsync(CategoryEditDTO input, Guid userId);  
        Task DeleteCategoryAsync(int id, Guid userId);  
        Task<List<SelectListItem>> GetCategoriesForDropdownAsync(Guid userId);
        Task<bool> CategoryExistsAsync(int categoryId, Guid userId);
    }
}
