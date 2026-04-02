using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StudyPlanner.Data.Models;
using StudyPlanner.Services.Core.Contracts;
using StudyPlanner.Services.Core.Models.Admin;

namespace StudyPlanner.Services.Core.Services
{
    public class AdminService : IAdminService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<(List<AdminUserDTO> Items, int TotalCount)> GetAllUsersAsync(string? searchTerm, int pageNumber, int pageSize)
        {
            var query = _userManager.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
                query = query.Where(u => u.Email!.Contains(searchTerm) || u.FullName!.Contains(searchTerm));

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderBy(u => u.Email)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new AdminUserDTO
                {
                    Id = u.Id,
                    Email = u.Email!,
                    FullName = u.FullName
                })
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<AdminUserDTO> GetUserByIdAsync(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());

            if (user == null)
                throw new KeyNotFoundException("User not found.");

            return new AdminUserDTO
            {
                Id = user.Id,
                Email = user.Email!,
                FullName = user.FullName
            };
        }

        public async Task DeleteUserAsync(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());

            if (user == null)
                throw new KeyNotFoundException("User not found.");

            await _userManager.DeleteAsync(user);
        }
    }
}