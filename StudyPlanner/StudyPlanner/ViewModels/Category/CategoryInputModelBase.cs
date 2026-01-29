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
        [StringLength(CategoryColorLength)]
        [RegularExpression("^#([0-9A-Fa-f]{6})$", ErrorMessage = "Invalid hex color.")]
        public string Color { get; set; } = null!;
    }
}
