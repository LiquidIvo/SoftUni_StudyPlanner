using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StudyPlanner.Data.Models;
namespace StudyPlanner.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public virtual DbSet<Category> Categories { get; set; } = null!;
        public virtual DbSet<Subject> Subjects { get; set; } = null!;
        public virtual DbSet<StudyTask> StudyTasks { get; set; } = null!;
        public virtual DbSet<StudySession> StudySessions { get; set; } = null!;


        override protected void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);


            builder.Entity<StudyTask>()
            .HasOne(t => t.Category)
            .WithMany(c => c.StudyTasks)
            .HasForeignKey(t => t.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<StudyTask>()
                .HasOne(t => t.Subject)
                .WithMany(s => s.StudyTasks)
                .HasForeignKey(t => t.SubjectId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<StudySession>()
                .HasOne(s => s.StudyTask)
                .WithMany(t => t.StudySessions)
                .HasForeignKey(s => s.StudyTaskId)
                .OnDelete(DeleteBehavior.Cascade);


            builder.Entity<StudySession>()
                .HasOne(s => s.User)
                .WithMany()
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Restrict);


            var admin = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                UserName = "admin@gmail.com",
                NormalizedUserName = "ADMIN@GMAIL.COM",
                Email = "admin@gmail.com",
                NormalizedEmail = "ADMIN@GMAIL.COM",
                SecurityStamp = Guid.NewGuid().ToString(), 
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                EmailConfirmed = true,
                PasswordHash = new PasswordHasher<ApplicationUser>().HashPassword(new ApplicationUser { UserName = "admin@gmail.com" }, "Admin12345!")

            };
            builder.Entity<ApplicationUser>().HasData(admin);
        }
    }
}
