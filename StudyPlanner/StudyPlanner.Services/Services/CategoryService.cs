using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StudyPlanner.Data.Models;
using StudyPlanner.Data.Repositories.Interfaces;
using StudyPlanner.Services.Core.Contracts;
using StudyPlanner.Services.Core.Models.Category;

namespace StudyPlanner.Services.Core.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IRepository<Category> _categoryRepo;

        public CategoryService(IRepository<Category> categoryRepo)
        {
            _categoryRepo = categoryRepo;
        }

        public async Task CreateCategoryAsync(CategoryCreateDTO input, Guid userId)
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

        public async Task<(List<CategoryDTO> Items, int TotalCount)> GetAllCategoriesAsync(Guid userId,string? searchTerm,int pageNumber ,int pageSize)
        {
            var query = _categoryRepo.All()
              .Where(c => c.UserId == userId);

            if(!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(c => c.Name.Contains(searchTerm));
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderBy(c => c.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new CategoryDTO
              {
                  Id = c.Id,
                  Name = c.Name,
                  Color = c.Color
              })
              .ToListAsync();

            return (items,totalCount);
        }

        public async Task<CategoryDTO> GetCategoryByIdAsync(int id, Guid userId)
        {
            var category = await _categoryRepo.All()
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
                throw new KeyNotFoundException("Category not found.");

            if (category.UserId != userId)
                throw new UnauthorizedAccessException("Access denied.");

            return new CategoryDTO
            {
                Id = category.Id,
                Name = category.Name,
                Color = category.Color
            };
        }

        public async Task UpdateCategoryAsync(CategoryEditDTO input, Guid userId)
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

        public async Task DeleteCategoryAsync(int id, Guid userId)
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

        public async Task<List<SelectListItem>> GetCategoriesForDropdownAsync(Guid userId)
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

        public async Task<bool> CategoryExistsAsync(int categoryId, Guid userId)
        {
            return await _categoryRepo.All()
                .AnyAsync(c => c.Id == categoryId && c.UserId == userId);
        }

        public async Task<CategoryEditDTO> GetCategoryByIdAsyncForEdit(int id, Guid userId)
        {
            var category = await _categoryRepo.All()
               .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
                throw new KeyNotFoundException("Category not found.");

            if (category.UserId != userId)
                throw new UnauthorizedAccessException("Access denied.");

            return new CategoryEditDTO
            {
                Id = category.Id,
                Name = category.Name,
                Color = category.Color
            };
        }
    }
}