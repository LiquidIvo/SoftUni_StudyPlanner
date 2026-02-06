using Microsoft.AspNetCore.Identity;
using StudyPlanner.GCommon.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static StudyPlanner.GCommon.EntityValidation;

namespace StudyPlanner.Data.Models
{
    public class StudyTask
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(StudyTaskTitleMaxLength)]
        public string Title { get; set; } = null!;


        [MaxLength(StudyTaskDescriptionMaxLength)]
        public string? Description { get; set; }

        [Required]
        public DateTime DueDate { get; set; }

        [Required]
        public TaskPriority Priority { get; set; }

        [Required]
        public GCommon.Enums.TaskStatus Status { get; set; }


        [ForeignKey(nameof(Category))]
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; } = null!;


        
        [ForeignKey(nameof(Subject))]
        public int SubjectId { get; set; }
        public virtual Subject Subject { get; set; } = null!;

        [Required]
        [ForeignKey(nameof(User))]
        public string UserId { get; set; } = null!;
        public virtual IdentityUser User { get; set; } = null!;

        public virtual ICollection<StudySession> StudySessions { get; set; } = new HashSet<StudySession>();
    }
}

