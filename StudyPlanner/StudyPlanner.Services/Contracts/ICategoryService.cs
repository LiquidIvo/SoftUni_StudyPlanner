using Microsoft.AspNetCore.Mvc.Rendering;
using StudyPlanner.Services.Core.Models.Category;


namespace StudyPlanner.Services.Core.Contracts
{
    public interface ICategoryService
    {
        Task CreateCategoryAsync(CategoryCreateDTO input, string userId);
        Task<List<CategoryDTO>> GetAllCategoriesAsync(string userId); 
        Task<CategoryDTO> GetCategoryByIdAsync(int id, string userId);  

        Task<CategoryEditDTO> GetCategoryByIdAsyncForEdit(int id, string userId);    
        Task UpdateCategoryAsync(CategoryEditDTO input, string userId);  
        Task DeleteCategoryAsync(int id, string userId);  
        Task<List<SelectListItem>> GetCategoriesForDropdownAsync(string userId);
        Task<bool> CategoryExistsAsync(int categoryId, string userId);
    }
}
