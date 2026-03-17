using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using StudyPlanner.Data.Seeding.Contracts;

namespace StudyPlanner.Web.Infrastructure.Extensions
{
    public static class WebApplicationExtensions
    {
        public static IApplicationBuilder UseRolesSeeder(this IApplicationBuilder applicationBuilder)
        {
            using IServiceScope scope = applicationBuilder
                .ApplicationServices
                .CreateScope();
            IIdentitySeeder identitySeeder = scope
                .ServiceProvider
                .GetRequiredService<IIdentitySeeder>();

            identitySeeder
                .SeedRolesAsync()
                .GetAwaiter()
                .GetResult();

            return applicationBuilder;
        }

        public static IApplicationBuilder UseAdminUserSeeder(this IApplicationBuilder applicationBuilder)
        {
            using IServiceScope scope = applicationBuilder
                .ApplicationServices
                .CreateScope();
            IIdentitySeeder identitySeeder = scope
                .ServiceProvider
                .GetRequiredService<IIdentitySeeder>();

            identitySeeder
                .SeedAdminUserAsync()
                .GetAwaiter()
                .GetResult();

            return applicationBuilder;
        }
    }
}

