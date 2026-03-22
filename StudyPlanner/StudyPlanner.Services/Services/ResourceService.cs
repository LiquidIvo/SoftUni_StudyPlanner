using Microsoft.EntityFrameworkCore;
using StudyPlanner.Data.Models;
using StudyPlanner.Data.Repositories.Interfaces;
using StudyPlanner.Services.Core.Models.Resource;
using System.ComponentModel.Design;

namespace StudyPlanner.Services.Core.Services
{
    public class ResourceService : Contracts.IResourceService
    {
        private readonly IRepository<Resource> _resourceRepo;

        public ResourceService(IRepository<Resource> resourceRepo)
        {
            _resourceRepo = resourceRepo;
        }

       

        public async Task<(List<ResourceDTO> Items, int TotalCount)> GetAllResourcesAsync(
            Guid userId, string? searchTerm, int pageNumber, int pageSize)
        {
            var query = _resourceRepo.All()
                .Where(r => r.UserId == userId);

            if (!string.IsNullOrWhiteSpace(searchTerm))
                query = query.Where(r => r.Title.Contains(searchTerm));

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderBy(r => r.Title)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(r => new ResourceDTO
                {
                    Id = r.Id,
                    Title = r.Title,
                    Url = r.Url,
                    Description = r.Description
                })
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<ResourceDTO> GetResourceByIdAsync(int id, Guid userId)
        {
            var resource = await _resourceRepo.All()
                .FirstOrDefaultAsync(r => r.Id == id);

            if (resource == null)
                throw new KeyNotFoundException("Resource not found.");
            if (resource.UserId != userId)
                throw new UnauthorizedAccessException("Access denied.");

            return new ResourceDTO
            {
                Id = resource.Id,
                Title = resource.Title,
                Url = resource.Url,
                Description = resource.Description
            };
        }

        public async Task<ResourceEditDTO> GetResourceByIdAsyncForEdit(int id, Guid userId)
        {
            var resource = await _resourceRepo.All()
                .FirstOrDefaultAsync(r => r.Id == id);

            if (resource == null)
                throw new KeyNotFoundException("Resource not found.");
            if (resource.UserId != userId)
                throw new UnauthorizedAccessException("Access denied.");

            return new ResourceEditDTO
            {
                Id = resource.Id,
                Title = resource.Title,
                Url = resource.Url,
                Description = resource.Description
            };
        }

      

        public async Task CreateResourceAsync(ResourceCreateDTO input, Guid userId)
        {
            var resource = new Resource
            {
                Title = input.Title,
                Url = input.Url,
                Description = input.Description,
                UserId = userId
            };

            await _resourceRepo.AddAsync(resource);
            await _resourceRepo.SaveChangesAsync();
        }

        public async Task UpdateResourceAsync(ResourceEditDTO input, Guid userId)
        {
            var resource = await _resourceRepo.All()
                .FirstOrDefaultAsync(r => r.Id == input.Id);

            if (resource == null)
                throw new KeyNotFoundException("Resource not found.");
            if (resource.UserId != userId)
                throw new UnauthorizedAccessException("Access denied.");

            resource.Title = input.Title;
            resource.Url = input.Url;
            resource.Description = input.Description;

            _resourceRepo.Update(resource);
            await _resourceRepo.SaveChangesAsync();
        }

        public async Task DeleteResourceAsync(int id, Guid userId)
        {
            var resource = await _resourceRepo.All()
                .FirstOrDefaultAsync(r => r.Id == id);

            if (resource == null)
                throw new KeyNotFoundException("Resource not found.");
            if (resource.UserId != userId)
                throw new UnauthorizedAccessException("Access denied.");

            _resourceRepo.Delete(resource);
            await _resourceRepo.SaveChangesAsync();
        }
    }
}
