using StudyPlanner.Services.Core.Models.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudyPlanner.Services.Core.Contracts
{
    public interface IAdminService
    {
        
        Task<List<AdminUserDTO>> GetAllUsersAsync();

        Task<AdminUserDTO> GetUserByIdAsync(Guid id);
        Task DeleteUserAsync(Guid id);
    }
}
