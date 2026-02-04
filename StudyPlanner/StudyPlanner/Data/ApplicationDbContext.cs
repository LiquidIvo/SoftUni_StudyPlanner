using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Build.Execution;
using Microsoft.EntityFrameworkCore;
using StudyPlanner.Models;
using System.Reflection.Emit;
namespace StudyPlanner.Data
{
    public class ApplicationDbContext : IdentityDbContext
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
        }
    }
}
