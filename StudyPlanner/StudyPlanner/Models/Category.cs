
using System.ComponentModel.DataAnnotations;
using static StudyPlanner.Common.EntityValidation;
namespace StudyPlanner.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(CategoryNameMaxLength)]
        public string Name { get; set; } = null!;

        [Required]
        [MaxLength(CategoryColorLength)]
        public string Color { get; set; } = null!;

        public virtual ICollection<StudyTask> StudyTasks { get; set; } = new HashSet<StudyTask>();
    }
}
