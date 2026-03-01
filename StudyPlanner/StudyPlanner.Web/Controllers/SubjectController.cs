using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StudyPlanner.Services.Contracts;
using StudyPlanner.Services.Core.Models.Subject;
using StudyPlanner.Services.Services;
using StudyPlanner.ViewModels.Subject;


namespace StudyPlanner.Controllers
{
    [Authorize]
    public class SubjectController : Controller
    {
        private readonly ISubjectService _subjectService;
        private readonly UserManager<IdentityUser> _userManager;

        public SubjectController(ISubjectService subjectService, UserManager<IdentityUser> userManager)
        {
            _subjectService = subjectService;
            _userManager = userManager;
        }

        private string? GetCurrentUserId()
        {
            return _userManager.GetUserId(User);
        }

        // GET: Subject
        public async Task<IActionResult> Index()
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var subjects = await _subjectService.GetAllSubjectsAsync(userId);


            var viewModels = subjects.Select(d => new SubjectViewModel
            {
                Id = d.Id,
                Name = d.Name,
                Color = d.Color
            }).ToList();


            return View(viewModels);
        }

        // GET: Subject/Create
        public IActionResult Create()
        {
            return View(new SubjectCreateInputModel());
        }

        // POST: Subject/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SubjectCreateInputModel input)
        {
            if (!ModelState.IsValid)
                return View(input);

            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

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

        // GET: Subject/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

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

        // POST: Subject/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SubjectEditInputModel input)
        {
            if (!ModelState.IsValid)
                return View(input);


            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

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

        // GET: Subject/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

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

        // POST: Subject/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

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