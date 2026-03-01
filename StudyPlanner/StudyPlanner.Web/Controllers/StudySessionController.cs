using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StudyPlanner.Services.Core.Contracts;
using StudyPlanner.Services.Core.Models.StudySession;
using StudyPlanner.ViewModels.StudySession;

namespace StudyPlanner.Web.Controllers
{
    [Authorize]
    public class StudySessionController : Controller
    {
        private readonly IStudySessionService _sessionService;
    
        private readonly UserManager<IdentityUser> _userManager;

        public StudySessionController(IStudySessionService sessionService, UserManager<IdentityUser> userManager)
        {
           
            _sessionService = sessionService;
            _userManager = userManager;
        }

        private string? GetCurrentUserId()
        {
            return _userManager.GetUserId(User);
        }
        public async Task<IActionResult> Details(int id)
        {

            var userId = GetCurrentUserId();

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            try
            {
                var dto = await _sessionService.GetStudySessionByIdAsync(id, userId);

                var viewModel = new StudySessionViewModel
                {
                    Id = dto.Id,
                    StudyTaskId = dto.StudyTaskId,
                    StartTime = dto.StartTime,
                    EndTime = dto.EndTime,
                    Notes = dto.Notes
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


        // GET: StudySession/Create
        public async Task<IActionResult> Create(int studyTaskId)
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            try
            {
                await _sessionService.CheckTaskOwnershipAsync(studyTaskId,userId);
                var model = new StudySessionCreateInputModel
                {
                    StudyTaskId = studyTaskId
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

        // POST: StudySession/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(StudySessionCreateInputModel input)
        {
            if (!ModelState.IsValid)
            {
                return View(input);
            }

            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            try
            {
                var dto = new StudySessionCreateDTO
                {
                    StartTime = input.StartTime,
                    EndTime = input.EndTime,
                    Notes = input.Notes,
                    StudyTaskId = input.StudyTaskId
                };
                await _sessionService.CreateStudySessionAsync(dto, input.StudyTaskId, userId);
                return RedirectToAction("Details", "StudyTask", new { id = input.StudyTaskId });
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

        // GET
        public async Task<IActionResult> Edit(int id)
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            try
            {
                var dto = await _sessionService.GetStudySessionByIdAsyncForEdit(id, userId);

                var viewModel = new StudySessionEditInputModel
                {
                    Id = dto.Id,
                    StudyTaskId = dto.StudyTaskId,
                    StartTime = dto.StartTime,
                    EndTime = dto.EndTime,
                    Notes = dto.Notes
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

        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(StudySessionEditInputModel input)
        {
            if (!ModelState.IsValid)
            {
                return View(input);
            }

            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            try
            {
                var dto = new StudySessionEditDTO
                {
                    Id = input.Id,
                    StudyTaskId = input.StudyTaskId,
                    StartTime = input.StartTime,
                    EndTime = input.EndTime,
                    Notes = input.Notes
                };

                await _sessionService.UpdateStudySessionAsync(dto, userId);
                return RedirectToAction("Details", "StudyTask", new { id = input.StudyTaskId });
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


        // GET: StudySession/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            try
            {
                var dto = await _sessionService.GetStudySessionByIdAsync(id, userId);

                var viewModel = new StudySessionViewModel
                {
                    Id = dto.Id,
                    StudyTaskId = dto.StudyTaskId,
                    StartTime = dto.StartTime,
                    EndTime = dto.EndTime,
                    Notes = dto.Notes
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

        // POST: StudySession/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            try
            {
                var studyTaskId = await _sessionService.DeleteStudySessionAsync(id, userId);
                return RedirectToAction("Details", "StudyTask", new { id = studyTaskId });
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
