using StudyPlanner.Services.Core.Models.Resource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudyPlanner.Services.Core.Contracts
{
    public interface IResourceService
    {
        Task<(List<ResourceDTO> Items, int TotalCount)> GetAllResourcesAsync(
            Guid userId, string? searchTerm, int pageNumber, int pageSize);

        Task<ResourceDTO> GetResourceByIdAsync(int id, Guid userId);
        Task<ResourceEditDTO> GetResourceByIdAsyncForEdit(int id, Guid userId);
        Task CreateResourceAsync(ResourceCreateDTO input, Guid userId);
        Task UpdateResourceAsync(ResourceEditDTO input, Guid userId);
        Task DeleteResourceAsync(int id, Guid userId);
    }
}
