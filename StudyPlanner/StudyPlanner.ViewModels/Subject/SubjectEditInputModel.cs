using System.ComponentModel.DataAnnotations;
namespace StudyPlanner.ViewModels.Subject
{
    public class SubjectEditInputModel : SubjectInputModelBase
    {
        [Required]
        public int Id { get; set; }


    }
}
