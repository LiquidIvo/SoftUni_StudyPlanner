using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StudyPlanner.Data;
using StudyPlanner.Data.Models;
using StudyPlanner.Data.Repositories;
using StudyPlanner.Data.Repositories.Interfaces;
using StudyPlanner.Data.Seeding;
using StudyPlanner.Data.Seeding.Contracts;
using StudyPlanner.Services.Core.Contracts;
using StudyPlanner.Services.Core.Services;
using StudyPlanner.Web.Infrastructure.Extensions;

namespace StudyPlanner.Web
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


            
            builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
            {
                ConfigureIdentity(options, builder.Configuration);
            }
            )
                .AddRoles<ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();
            builder.Services.AddControllersWithViews();

            builder.Services.AddTransient<IIdentitySeeder, IdentitySeeder>();
            builder.Services.AddScoped<IAdminService, AdminService>();
            builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            builder.Services.AddScoped<ICategoryService, CategoryService>();
            builder.Services.AddScoped<ISubjectService, SubjectService>();
            builder.Services.AddScoped<IStudyTaskService, StudyTaskService>();
            builder.Services.AddScoped<IStudySessionService, StudySessionService>();
            builder.Services.AddScoped<IPdfService, PdfService>();
            builder.Services.AddHttpClient<IQuoteService, QuoteService>();

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

            app.UseRolesSeeder();
            app.UseAdminUserSeeder();
            //
            app.UseStatusCodePagesWithReExecute("/Home/Error/{0}");

            app.MapControllerRoute(
                 name: "areas",
                 pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");


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

        private static void ConfigureIdentity(IdentityOptions options, ConfigurationManager configuration)
        {
            // SignIn
            options.SignIn.RequireConfirmedAccount = configuration.GetValue<bool>("IdentityOptions:SignIn:RequireConfirmedAccount");
            options.SignIn.RequireConfirmedEmail = configuration.GetValue<bool>("IdentityOptions:SignIn:RequireConfirmedEmail");
            options.SignIn.RequireConfirmedPhoneNumber = configuration.GetValue<bool>("IdentityOptions:SignIn:RequireConfirmedPhoneNumber");

            // User
            options.User.RequireUniqueEmail = configuration.GetValue<bool>("IdentityOptions:User:RequireUniqueEmail");

            // Lockout
            options.Lockout.MaxFailedAccessAttempts = configuration.GetValue<int>("IdentityOptions:Lockout:MaxFailedAccessAttempts");
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(
                configuration.GetValue<int>("IdentityOptions:Lockout:DefaultLockoutTimeSpanMinutes"));

            // Password
            options.Password.RequireDigit = configuration.GetValue<bool>("IdentityOptions:Password:RequireDigit");
            options.Password.RequireLowercase = configuration.GetValue<bool>("IdentityOptions:Password:RequireLowercase");
            options.Password.RequireUppercase = configuration.GetValue<bool>("IdentityOptions:Password:RequireUppercase");
            options.Password.RequireNonAlphanumeric = configuration.GetValue<bool>("IdentityOptions:Password:RequireNonAlphanumeric");
            options.Password.RequiredLength = configuration.GetValue<int>("IdentityOptions:Password:RequiredLength");
            options.Password.RequiredUniqueChars = configuration.GetValue<int>("IdentityOptions:Password:RequiredUniqueChars");
        }
    }
}
