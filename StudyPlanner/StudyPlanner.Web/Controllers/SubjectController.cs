using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StudyPlanner.Data.Models;
using StudyPlanner.Services.Core.Contracts;
using StudyPlanner.Services.Core.Models.Subject;
using StudyPlanner.ViewModels.Subject;


namespace StudyPlanner.Web.Controllers
{
    [Authorize]
    public class SubjectController : Controller
    {
        private readonly ISubjectService _subjectService;
        private readonly UserManager<ApplicationUser> _userManager;

        public SubjectController(ISubjectService subjectService, UserManager<ApplicationUser> userManager)
        {
            _subjectService = subjectService;
            _userManager = userManager;
        }

        private Guid GetCurrentUserId()
        {
            var userId = _userManager.GetUserId(User);
            return Guid.Parse(userId!);
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = GetCurrentUserId();
          

            var subjects = await _subjectService.GetAllSubjectsAsync(userId);


            var viewModels = subjects.Select(d => new SubjectViewModel
            {
                Id = d.Id,
                Name = d.Name,
                Color = d.Color
            }).ToList();


            return View(viewModels);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new SubjectCreateInputModel());
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SubjectCreateInputModel input)
        {
            if (!ModelState.IsValid)
                return View(input);

            var userId = GetCurrentUserId();
            

            try
            {
                var dto = new SubjectCreateDTO
                {
                    Name = input.Name,
                    Color = input.Color
                };

                await _subjectService.CreateSubjectAsync(dto, userId);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "An error occurred while creating the subject.");
                return View(input);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var userId = GetCurrentUserId();
           

            try
            {
                var dto = await _subjectService.GetSubjectByIdAsyncForEdit(id, userId);

                var viewModel = new SubjectEditInputModel
                {
                    Id = dto.Id,
                    Name = dto.Name,
                    Color = dto.Color
                };

                return View(viewModel);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SubjectEditInputModel input)
        {
            if (!ModelState.IsValid)
                return View(input);


            var userId = GetCurrentUserId();
           

            try
            {
                var dto = new SubjectEditDTO
                {
                    Id = input.Id,
                    Name = input.Name,
                    Color = input.Color
                };
                await _subjectService.UpdateSubjectAsync(dto, userId);  
                return RedirectToAction(nameof(Index));
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = GetCurrentUserId();
           
            try
            {
                var dto = await _subjectService.GetSubjectByIdAsync(id, userId);
                var viewModel = new SubjectViewModel
                {
                    Id = dto.Id,
                    Name = dto.Name,
                    Color = dto.Color
                };  

                return View(viewModel);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

       
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = GetCurrentUserId();
           

            try
            {
                await _subjectService.DeleteSubjectAsync(id, userId);  
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {

                var dto = await _subjectService.GetSubjectByIdAsync(id, userId);

                var viewModel = new SubjectViewModel
                {
                    Id = dto.Id,
                    Name = dto.Name,
                    Color = dto.Color
                };
                ViewData["ErrorMessage"] = ex.Message;
                return View("Delete", viewModel);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }
    }
}