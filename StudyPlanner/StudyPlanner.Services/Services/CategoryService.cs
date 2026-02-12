using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StudyPlanner.Data.Models;
using StudyPlanner.Data.Repositories.Interfaces;
using StudyPlanner.Services.Contracts;
using StudyPlanner.ViewModels.Category;

namespace StudyPlanner.Services.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IRepository<Category> _categoryRepo;

        public CategoryService(IRepository<Category> categoryRepo)
        {
            _categoryRepo = categoryRepo;
        }

        public async Task CreateCategoryAsync(CategoryCreateInputModel input, string userId)
        {

            var category = new Category
            {
                Name = input.Name,
                Color = input.Color,
                UserId = userId
            };

            await _categoryRepo.AddAsync(category);
            await _categoryRepo.SaveChangesAsync();
        }

        public async Task<List<CategoryViewModel>> GetAllCategoriesAsync(string userId)
        {
            return await _categoryRepo.All()
              .Where(s => s.UserId == userId)
              .Select(s => new CategoryViewModel
              {
                  Id = s.Id,
                  Name = s.Name,
                  Color = s.Color
              })
              .ToListAsync();
        }

        public async Task<CategoryViewModel> GetCategoryByIdAsync(int id, string userId)
        {
            var category = await _categoryRepo.All()
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
                throw new KeyNotFoundException("Category not found.");

            if (category.UserId != userId)
                throw new UnauthorizedAccessException("Access denied.");

            return new CategoryViewModel
            {
                Id = category.Id,
                Name = category.Name,
                Color = category.Color
            };
        }

        public async Task UpdateCategoryAsync(CategoryEditInputModel input, string userId)
        {
            var category = await _categoryRepo.All()
                .FirstOrDefaultAsync(c => c.Id == input.Id);

            if (category == null)
                throw new KeyNotFoundException("Category not found.");

            if (category.UserId != userId)
                throw new UnauthorizedAccessException("Access denied.");

            category.Name = input.Name;
            category.Color = input.Color;

            _categoryRepo.Update(category);
            await _categoryRepo.SaveChangesAsync();
        }

        public async Task DeleteCategoryAsync(int id, string userId)
        {
            var category = await _categoryRepo.All()
                .Include(c => c.StudyTasks)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
                throw new KeyNotFoundException("Category not found.");

            if (category.UserId != userId)
                throw new UnauthorizedAccessException("Access denied.");

           
            if (category.StudyTasks != null && category.StudyTasks.Any())
                throw new InvalidOperationException("Cannot delete this category because it has associated study tasks.");

            _categoryRepo.Delete(category);
            await _categoryRepo.SaveChangesAsync();
        }

        public async Task<List<SelectListItem>> GetCategoriesForDropdownAsync(string userId)
        {
            return await _categoryRepo.All()
                .Where(c => c.UserId == userId)
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                })
                .ToListAsync();
        }

        public async Task<bool> CategoryExistsAsync(int categoryId, string userId)
        {
            return await _categoryRepo.All()
                .AnyAsync(c => c.Id == categoryId && c.UserId == userId);
        }
    }
}