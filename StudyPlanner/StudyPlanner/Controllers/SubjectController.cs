using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StudyPlanner.Services.Contracts;
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

        // GET: Subject
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var subjects = await _subjectService.GetAllSubjectsAsync(userId);
            return View(subjects);
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

            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            try
            {
                await _subjectService.CreateSubjectAsync(input, userId);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while creating the subject.");
                return View(input);
            }
        }

        // GET: Subject/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            try
            {
                var subject = await _subjectService.GetSubjectByIdAsync(id, userId);

                var model = new SubjectEditInputModel
                {
                    Id = subject.Id,
                    Name = subject.Name,
                    Color = subject.Color
                };

                return View(model);
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


            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            try
            {
                await _subjectService.UpdateSubjectAsync(input, userId);  
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
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            try
            {
                var subject = await _subjectService.GetSubjectByIdAsync(id, userId);
                return View(subject);
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
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            try
            {
                await _subjectService.DeleteSubjectAsync(id, userId);  
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {

                var subject = await _subjectService.GetSubjectByIdAsync(id, userId);
                ViewData["ErrorMessage"] = ex.Message;
                return View("Delete", subject);
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