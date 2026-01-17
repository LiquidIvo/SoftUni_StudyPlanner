using StudyPlanner.Common;
using StudyPlanner.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static StudyPlanner.Common.EntityValidation;

namespace StudyPlanner.Models
{
    public class StudyTask
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MinLength(StudyTaskTitleMinLength)]
        [MaxLength(StudyTaskTitleMaxLength)]
        public string Title { get; set; } = null!;


        [MaxLength(StudyTaskDescriptionMaxLength)]
        public string? Description { get; set; }

        [Required]
        public DateTime DueDate { get; set; }

        [Required]
        public TaskPriority Priority { get; set; }

        [Required]
        public Enums.TaskStatus Status { get; set; }


        [Required]
        [ForeignKey(nameof(Category))]
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; } = null!;


        [Required]
        [ForeignKey(nameof(Subject))]
        public int SubjectId { get; set; }
        public virtual Subject Subject { get; set; } = null!;

        public virtual ICollection<StudySession> StudySessions { get; set; } = new HashSet<StudySession>();
    }
}

