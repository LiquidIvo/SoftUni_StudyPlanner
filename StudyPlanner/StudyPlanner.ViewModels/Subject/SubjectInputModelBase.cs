using System.ComponentModel.DataAnnotations;
using static StudyPlanner.GCommon.EntityValidation;
namespace StudyPlanner.ViewModels.Subject
{
    public abstract class SubjectInputModelBase
    {
        [Required]
        [MinLength(SubjectNameMinLength)]
        [MaxLength(SubjectNameMaxLength)]
        public string Name { get; set; } = null!;


        [Required]
        [StringLength(CategoryColorLength)]
        [RegularExpression("^#([0-9A-Fa-f]{6})$", ErrorMessage = "Invalid hex color.")]
        public string Color { get; set; } = null!;


    }
}
