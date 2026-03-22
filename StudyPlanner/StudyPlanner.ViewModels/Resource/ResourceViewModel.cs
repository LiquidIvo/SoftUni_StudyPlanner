using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudyPlanner.ViewModels.Resource
{
    public class ResourceViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Url { get; set; } = null!;
        public string? Description { get; set; }
    }
}
