using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudyPlanner.Data.Seeding.Contracts
{
    public interface IIdentitySeeder
    {
        Task SeedRolesAsync();

        Task SeedAdminUserAsync();
    }
}
