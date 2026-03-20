using Microsoft.AspNetCore.Identity;
using StudyPlanner.Data.Models;
using StudyPlanner.Data.Seeding.Contracts;
using static StudyPlanner.GCommon.ApplicationConstants;
namespace StudyPlanner.Data.Seeding
{
    public class IdentitySeeder : IIdentitySeeder
    {
        
        private const string adminEmail = "admin@gmail.com";
        private const string adminPassword = "Admin12345!";
        private const string fullname = "Admin";

        public static string[] ApplicationRoles = new[]
        {
            AdminRoleName,
            UserRoleName
        };

        private readonly RoleManager<ApplicationRole> roleManager;
        private readonly UserManager<ApplicationUser> userManager;

        public IdentitySeeder(
            RoleManager<ApplicationRole> roleManager,
            UserManager<ApplicationUser> userManager)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
        }

        public async Task SeedRolesAsync()
        {
            foreach (string role in ApplicationRoles)
            {
                bool roleExists = await roleManager.RoleExistsAsync(role);
                if (!roleExists)
                {
                    var newRole = new ApplicationRole
                    {
                        Name = role
                    };

                    var result = await roleManager.CreateAsync(newRole);

                    if (!result.Succeeded)
                    {
                        throw new InvalidOperationException($"Failed to create role: {role}");
                    }
                }
            }
        }

        public async Task SeedAdminUserAsync()
        {
            

            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true,
                    FullName = fullname,
                    DateOfBirth = DateTime.Now.AddYears(-18)
                };

                var result = await userManager.CreateAsync(adminUser, adminPassword);

                if (!result.Succeeded)
                {
                    throw new InvalidOperationException("Failed to create admin user");
                }
            }

            bool isInRole = await userManager.IsInRoleAsync(adminUser, AdminRoleName);

            if (!isInRole)
            {
                var result = await userManager.AddToRoleAsync(adminUser, AdminRoleName);

                if (!result.Succeeded)
                {
                    throw new InvalidOperationException("Failed to assign Admin role");
                }
            }
        }
    }
}