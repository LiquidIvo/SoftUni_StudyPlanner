using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudyPlanner.ViewModels;

namespace StudyPlanner.Web.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [Route("/Home/Error/{statusCode}")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(int statusCode)
        {
            if(statusCode == StatusCodes.Status400BadRequest)
            {
                return View("BadRequest");
            }
            if (statusCode == StatusCodes.Status403Forbidden)
            {
                return View("Forbidden");
            }
            if (statusCode == StatusCodes.Status404NotFound)
            {
                return View("NotFound");
            }
            if (statusCode == StatusCodes.Status401Unauthorized)
            {
                return View("Unauthorized");
            }

            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
