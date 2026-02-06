using Microsoft.AspNetCore.Mvc.Rendering;
using StudyPlanner.ViewModels.Category;


namespace StudyPlanner.Services.Contracts
{
    public interface ICategoryService
    {
        Task CreateCategoryAsync(CategoryCreateInputModel input, string userId);
        Task<List<CategoryViewModel>> GetAllCategoriesAsync(string userId); 
        Task<CategoryViewModel> GetCategoryByIdAsync(int id, string userId);  
        Task UpdateCategoryAsync(CategoryEditInputModel input, string userId);  
        Task DeleteCategoryAsync(int id, string userId);  
        Task<List<SelectListItem>> GetCategoriesForDropdownAsync(string userId);
        Task<bool> CategoryExistsAsync(int categoryId, string userId);
    }
}
