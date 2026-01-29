using System.ComponentModel.DataAnnotations;
using static StudyPlanner.Common.EntityValidation;
namespace StudyPlanner.ViewModels.Category
{
    public class CategoryEditInputModel : CategoryInputModelBase
    {
        [Required]
        public int Id { get; set; }

       
       
    }
}
