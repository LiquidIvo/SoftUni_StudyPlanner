using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudyPlanner.ViewModels.Resource
{
    public class ResourceEditInputModel : ResourceInputModelBase
    {
        [Required]
        public int Id { get; set; }
    }
}
