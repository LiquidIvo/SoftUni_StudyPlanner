using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using StudyPlanner.Services.Contracts;
using StudyPlanner.ViewModels.StudyTask;


namespace StudyPlanner.Controllers
{
    [Authorize]
    public class StudyTaskController : Controller
    {
        private readonly IStudyTaskService _studyTaskService;
        private readonly ICategoryService _categoryService;
        private readonly ISubjectService _subjectService;
        private readonly UserManager<IdentityUser> _userManager;

        public StudyTaskController(
            IStudyTaskService studyTaskService,
            ICategoryService categoryService,
            ISubjectService subjectService,
            UserManager<IdentityUser> userManager)
        {
            _studyTaskService = studyTaskService;
            _categoryService = categoryService;
            _subjectService = subjectService;
            _userManager = userManager;
        }

        // GET: StudyTask
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            try
            {
                var tasks = await _studyTaskService.GetAllTasksAsync(userId);
                return View(tasks);
            }
            catch (Exception)
            {
                ModelState.AddModelError("","An error occurred while displaying the study tasks");
                return View(new List<StudyTaskViewModel>());
            }
        }

        // GET: StudyTask/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var userId = _userManager.GetUserId(User);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            try
            {
                var task = await _studyTaskService.GetDetailedStudyTaskByIdAsync(id.Value, userId);

                return View(task);
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

        // GET: StudyTask/Create
        public async Task<IActionResult> Create()
        {
            await LoadDropdowns();
            return View(new StudyTaskCreateInputModel());
        }

        // POST: StudyTask/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(StudyTaskCreateInputModel model)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            if (!ModelState.IsValid)
            {
                await LoadDropdowns();
                return View(model);
            }

            try
            {
                await _studyTaskService.CreateTaskAsync(model, userId);
                return RedirectToAction(nameof(Index));
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError("", ex.Message);
                await LoadDropdowns();
                return View(model);
            }
        }

        // GET: StudyTask/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            try
            {

                var task = await _studyTaskService.GetStudyTaskForEditByIdAsync(id.Value, userId);
                await LoadDropdowns();
                return View(task);
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

        // POST: StudyTask/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(StudyTaskEditInputModel model)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            if (!ModelState.IsValid)
            {
                await LoadDropdowns();
                return View(model);
            }

            try
            {

                await _studyTaskService.UpdateTaskAsync(model, userId);
                return RedirectToAction(nameof(Index));
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError("", ex.Message);
                await LoadDropdowns();
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

        // GET: StudyTask/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            try
            {
                var task = await _studyTaskService.GetTaskByIdAsync(id, userId);

                return View(task);
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

        // POST: StudyTask/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            try
            {
                
                await _studyTaskService.DeleteTaskAsync(id, userId);
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

        private async Task LoadDropdowns()
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
                return;

            ViewBag.CategoryId = await _categoryService.GetCategoriesForDropdownAsync(userId);
            ViewBag.SubjectId = await _subjectService.GetSubjectsForDropdownAsync(userId);
        }
    }
}