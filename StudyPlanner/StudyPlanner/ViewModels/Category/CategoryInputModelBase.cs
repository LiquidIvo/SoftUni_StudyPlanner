using System.ComponentModel.DataAnnotations;
using static StudyPlanner.Common.EntityValidation;
namespace StudyPlanner.ViewModels.Category
{
    public abstract class CategoryInputModelBase
    {
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
