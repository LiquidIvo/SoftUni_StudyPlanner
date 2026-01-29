using System.ComponentModel.DataAnnotations;
using static StudyPlanner.Common.EntityValidation;
namespace StudyPlanner.ViewModels.Subject
{
    public abstract class SubjectInputModelBase
    {
        [Required]
        [MinLength(SubjectNameMinLength)]
        [MaxLength(SubjectNameMaxLength)]
        public string Name { get; set; } = null!;
    }
}
