using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static StudyPlanner.GCommon.EntityValidation;
namespace StudyPlanner.Data.Models
{
    public class Subject
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(SubjectNameMaxLength)]
        public string Name { get; set; } = null!;

        [Required]
        [MaxLength(CategoryColorLength)]
        public string Color { get; set; } = null!;

        [Required]
        [ForeignKey(nameof(User))]
        public string UserId { get; set; } = null!;
        public virtual IdentityUser User { get; set; } = null!;
        public virtual ICollection<StudyTask> StudyTasks { get; set; } = new HashSet<StudyTask>();
    }

}

