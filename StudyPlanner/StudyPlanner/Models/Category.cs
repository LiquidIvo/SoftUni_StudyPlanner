
using System.ComponentModel.DataAnnotations;
using static StudyPlanner.Common.EntityValidation;
namespace StudyPlanner.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MinLength(CategoryNameMinLength)]
        [MaxLength(CategoryNameMaxLength)]
        public string Name { get; set; } = null!;

        [Required]
        [MinLength(CategoryColorMinLength)]
        [MaxLength(CategoryColorMaxLength)]
        public string Color { get; set; } = null!;
    }
}
