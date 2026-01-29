using System.ComponentModel.DataAnnotations;
using static StudyPlanner.Common.EntityValidation;
namespace StudyPlanner.ViewModels.Subject
{
    public class SubjectEditInputModel : SubjectInputModelBase
    {
        [Required]
        public int Id { get; set; }

    
    }
}
