using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using StudyPlanner.Data.Models;

namespace StudyPlanner.Data.Configuration
{
    public static class DatabaseSeeder
    {
        public static void SeedRoles(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

            string[] roles = { "Admin", "User" };

            foreach (var role in roles)
            {
                var roleExists = roleManager.RoleExistsAsync(role).GetAwaiter().GetResult();
                if(!roleExists)
                {
                    var result = roleManager.CreateAsync(new ApplicationRole { Name = role }).GetAwaiter().GetResult();
                    if(!result.Succeeded)
                    {
                        throw new Exception($"Failed to create role: {role}");
                    }
                }
            }
        }
    }
}
