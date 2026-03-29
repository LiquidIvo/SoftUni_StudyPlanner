using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudyPlanner.Services.Core.Models.Resource
{
    public class ResourceCreateDTO
    {
        public string Title { get; set; } = null!;
        public string Url { get; set; } = null!;
        public string? Description { get; set; }
    }
}
