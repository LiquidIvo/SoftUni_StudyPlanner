using System.ComponentModel.DataAnnotations;
namespace StudyPlanner.ViewModels.Category
{
    public class CategoryEditInputModel : CategoryInputModelBase
    {
        [Required]
        public int Id { get; set; }



    }
}
