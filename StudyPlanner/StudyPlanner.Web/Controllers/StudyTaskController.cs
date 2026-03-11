using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StudyPlanner.Services.Core.Contracts;
using StudyPlanner.Services.Core.Models.StudyTask;
using StudyPlanner.ViewModels.StudyTask;


namespace StudyPlanner.Web.Controllers
{
    [Authorize]
    public class StudyTaskController : Controller
    {
        private readonly IStudyTaskService _studyTaskService;
        private readonly ICategoryService _categoryService;
        private readonly ISubjectService _subjectService;
        private readonly IPdfService _pdfService;
        private readonly UserManager<IdentityUser> _userManager;

        public StudyTaskController(
            IStudyTaskService studyTaskService,
            ICategoryService categoryService,
            ISubjectService subjectService,
            IPdfService pdfService,
            UserManager<IdentityUser> userManager)
        {
            _studyTaskService = studyTaskService;
            _categoryService = categoryService;
            _subjectService = subjectService;
            _pdfService = pdfService;
            _userManager = userManager;
        }

        private string? GetCurrentUserId()
        {
            return _userManager.GetUserId(User);
        }

        // GET: StudyTask
        public async Task<IActionResult> Index()
        {
            var userId = GetCurrentUserId();

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            try
            {
                var dtos = await _studyTaskService.GetAllTasksAsync(userId);

                var viewModels = dtos.Select(d => new StudyTaskViewModel
                {
                    Id = d.Id,
                    Title = d.Title,
                    Description = d.Description,
                    DueDate = d.DueDate,
                    Priority = d.Priority,
                    Status = d.Status,
                    Category = d.Category,
                    CategoryColor = d.CategoryColor,
                    Subject = d.Subject,
                    SubjectColor = d.SubjectColor
                }).ToList();

                return View(viewModels);
            }
            catch (Exception)
            {
                ModelState.AddModelError("","An error occurred while displaying the study tasks");
                return View(new List<StudyTaskViewModel>());
            }
        }

        // GET: StudyTask/Details/5
        public async Task<IActionResult> Details(int id)
        {
            

            var userId = GetCurrentUserId();

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            try
            {
                var dto = await _studyTaskService.GetDetailedStudyTaskByIdAsync(id, userId);

                var viewModel =  new StudyTaskDetailsViewModel
                {
                    Id = dto.Id,
                    Title = dto.Title,
                    Description = dto.Description,
                    DueDate = dto.DueDate,
                    Priority = dto.Priority,
                    Status = dto.Status,
                    Category = dto.Category,
                    CategoryColor = dto.CategoryColor,
                    Subject = dto.Subject,
                    SubjectColor = dto.SubjectColor,
                    StudySessions = dto.StudySessions.Select(s => new StudySessionItemViewModel
                    {
                        Id = s.Id,
                        StartTime = s.StartTime,
                        EndTime = s.EndTime,
                        Notes = s.Notes
                    }).ToList()
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
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            if (!ModelState.IsValid)
            {
                await LoadDropdowns();
                return View(model);
            }

            try
            {
                var dto = new StudyTaskCreateDTO
                {
                    Title = model.Title,
                    Description = model.Description,
                    DueDate = model.DueDate,
                    Priority = model.Priority,
                    Status = model.Status,
                    CategoryId = model.CategoryId,
                    SubjectId = model.SubjectId
                };

                await _studyTaskService.CreateTaskAsync(dto, userId);
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
        public async Task<IActionResult> Edit(int id)
        {
            

            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            try
            {

                var dto = await _studyTaskService.GetStudyTaskForEditByIdAsync(id, userId);

                var viewModel = new StudyTaskEditInputModel
                {
                    Id = dto.Id,
                    Title = dto.Title,
                    Description = dto.Description,
                    DueDate = dto.DueDate,
                    Priority = dto.Priority,
                    Status = dto.Status,
                    CategoryId = dto.CategoryId,
                    SubjectId = dto.SubjectId
                };

                await LoadDropdowns();
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

        // POST: StudyTask/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(StudyTaskEditInputModel model)
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            if (!ModelState.IsValid)
            {
                await LoadDropdowns();
                return View(model);
            }

            try
            {

                var dto = new StudyTaskEditDTO
                {
                    Id = model.Id,
                    Title = model.Title,
                    Description = model.Description,
                    DueDate = model.DueDate,
                    Priority = model.Priority,
                    Status = model.Status,
                    CategoryId = model.CategoryId,
                    SubjectId = model.SubjectId
                };

                await _studyTaskService.UpdateTaskAsync(dto, userId);
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
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            try
            {
                var dto = await _studyTaskService.GetTaskByIdAsync(id, userId);

                var viewModel = new StudyTaskViewModel
                {
                    Id = dto.Id,
                    Title = dto.Title,
                    Description = dto.Description,
                    DueDate = dto.DueDate,
                    Priority = dto.Priority,
                    Status = dto.Status,
                    Category = dto.Category,
                    CategoryColor = dto.CategoryColor,
                    Subject = dto.Subject,
                    SubjectColor = dto.SubjectColor
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

        // POST: StudyTask/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = GetCurrentUserId();
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

        [HttpGet]
        public async Task<IActionResult> DownloadPdf(int id)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            try
            {

                var dto = await _studyTaskService.GetDetailedStudyTaskByIdAsync(id, userId);
                var pdfBytes = _pdfService.GenerateStudyTaskPdf(dto);
                return File(pdfBytes, "application/pdf", $"StudyTask_{dto.Title}_{DateTime.Now:yyyyMMdd}.pdf");
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
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return;

            ViewBag.CategoryId = await _categoryService.GetCategoriesForDropdownAsync(userId);
            ViewBag.SubjectId = await _subjectService.GetSubjectsForDropdownAsync(userId);
        }
    }
}