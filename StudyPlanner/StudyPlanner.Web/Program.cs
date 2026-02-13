using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StudyPlanner.Data;
using StudyPlanner.Data.Repositories;
using StudyPlanner.Data.Repositories.Interfaces;
using StudyPlanner.Services.Contracts;
using StudyPlanner.Services.Core.Contracts;
using StudyPlanner.Services.Core.Services;
using StudyPlanner.Services.Services;

namespace StudyPlanner
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            string? connectionString = GetConnection(builder.Configuration);
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();


            
            builder.Services.AddDefaultIdentity<IdentityUser>(options =>
            {
                ConfigureIdentity(options, builder.Configuration);
            }
            )
                .AddEntityFrameworkStores<ApplicationDbContext>();
            builder.Services.AddControllersWithViews();

            builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            builder.Services.AddScoped<ICategoryService, CategoryService>();
            builder.Services.AddScoped<ISubjectService, SubjectService>();
            builder.Services.AddScoped<IStudyTaskService, StudyTaskService>();
            builder.Services.AddScoped<IStudySessionService, StudySessionService>();

            var app = builder.Build();


            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.MapRazorPages();

            app.Run();
        }
        private static string GetConnection(IConfiguration configuration)
        {
            
            string? connection = configuration.GetConnectionString("DevConnection");
            if (!string.IsNullOrWhiteSpace(connection))
            {
                return connection;
            }

            return configuration.GetConnectionString("DefaultConnection") ?? "";
        }

        private static void ConfigureIdentity(IdentityOptions options,ConfigurationManager configuration)
        {
            //Account options
            bool requireConfirmedAccount = configuration.GetValue<bool>("IdentityOptions:SignIn:RequireConfirmedAccount");
            bool requireConfirmedEmail = configuration.GetValue<bool>("IdentityOptions:SignIn:RequireConfirmedEmail");
            bool requireConfirmedPhoneNumber = configuration.GetValue<bool>("IdentityOptions:SignIn:RequireConfirmedPhoneNumber");

            // User options
            bool requireUniqueEmail = configuration.GetValue<bool>("IdentityOptions:User:RequireUniqueEmail");

            // Lockout options
            int maxFailedAccessAttempts = configuration.GetValue<int>("IdentityOptions:Lockout:MaxFailedAccessAttempts");
            int defaultLockoutTimeSpanMinutes = configuration.GetValue<int>("IdentityOptions:Lockout:DefaultLockoutTimeSpanMinutes");

            // Password options
            bool requireDigit = configuration.GetValue<bool>("IdentityOptions:Password:RequireDigit");
            bool requireLowercase = configuration.GetValue<bool>("IdentityOptions:Password:RequireLowercase");
            bool requireUppercase = configuration.GetValue<bool>("IdentityOptions:Password:RequireUppercase");
            bool requireNonAlphanumeric = configuration.GetValue<bool>("IdentityOptions:Password:RequireNonAlphanumeric");
            int requiredLength = configuration.GetValue<int>("IdentityOptions:Password:RequiredLength");
            int requiredUniqueChars = configuration.GetValue<int>("IdentityOptions:Password:RequiredUniqueChars");
        }
    }
}
