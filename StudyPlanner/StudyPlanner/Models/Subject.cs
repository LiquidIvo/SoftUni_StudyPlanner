
using System.ComponentModel.DataAnnotations;
using static StudyPlanner.Common.EntityValidation;
namespace StudyPlanner.Models
{
    public class Subject
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MinLength(SubjectNameMinLength)]
        [MaxLength(SubjectNameMaxLength)]
        public string Name { get; set; } = null!;

        public virtual ICollection<StudyTask> StudyTasks { get; set; } = new HashSet<StudyTask>();
    }

}

