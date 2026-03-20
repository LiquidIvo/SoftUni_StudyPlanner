using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StudyPlanner.Data.Models;
using StudyPlanner.Services.Core.Contracts;
using StudyPlanner.Services.Core.Models.StudySession;
using StudyPlanner.ViewModels.StudySession;
using static StudyPlanner.GCommon.ApplicationConstants;

namespace StudyPlanner.Web.Controllers
{
    [Authorize(Roles = AdminOrUser)]
    public class StudySessionController : Controller
    {
        private readonly IStudySessionService _sessionService;
    
        private readonly UserManager<ApplicationUser> _userManager;

        public StudySessionController(IStudySessionService sessionService, UserManager<ApplicationUser> userManager)
        {
           
            _sessionService = sessionService;
            _userManager = userManager;
        }

        private Guid GetCurrentUserId()
        {
            var userId = _userManager.GetUserId(User);
            return Guid.Parse(userId!);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {

            var userId = GetCurrentUserId();

           

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


        [HttpGet]
        public async Task<IActionResult> Create(int studyTaskId)
        {
            var userId = GetCurrentUserId();
           

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

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(StudySessionCreateInputModel input)
        {
            if (!ModelState.IsValid)
            {
                return View(input);
            }

            var userId = GetCurrentUserId();
           

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

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var userId = GetCurrentUserId();
           

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

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(StudySessionEditInputModel input)
        {
            if (!ModelState.IsValid)
            {
                return View(input);
            }

            var userId = GetCurrentUserId();
            

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


        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = GetCurrentUserId();
           

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

       
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = GetCurrentUserId();
           

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
