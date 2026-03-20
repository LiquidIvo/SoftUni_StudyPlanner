using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StudyPlanner.Data.Models;
using StudyPlanner.Services.Core.Contracts;
using StudyPlanner.Services.Core.Models.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudyPlanner.Services.Core.Services
{
    public class AdminService : IAdminService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<List<AdminUserDTO>> GetAllUsersAsync()
        {
            return await _userManager.Users
                .Select(u => new AdminUserDTO
                {
                    Id = u.Id,
                    Email = u.Email!,
                    FullName = u.FullName
                })
                .ToListAsync();
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
