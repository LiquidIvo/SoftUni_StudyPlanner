using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudyPlanner.Data.Models;
using StudyPlanner.Services.Contracts;
using StudyPlanner.Services.Core.Contracts;
using StudyPlanner.ViewModels.StudySession;

namespace StudyPlanner.Controllers
{
    [Authorize]
    public class StudySessionController : Controller
    {
        private readonly IStudySessionService _sessionService;
    
        private readonly UserManager<IdentityUser> _userManager;

        public StudySessionController(IStudySessionService sessionService, IStudyTaskService studyTaskService, UserManager<IdentityUser> userManager)
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
                var session = await _sessionService.GetStudySessionByIdAsync(id, userId);

                return View(session);
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
                
                await _sessionService.CreateStudySessionAsync(input, input.StudyTaskId, userId);
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
                var session = await _sessionService.GetStudySessionByIdAsyncForEdit(id, userId);
                return View(session);
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
                await _sessionService.UpdateStudySessionAsync(input, userId);
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
                var session = await _sessionService.GetStudySessionByIdAsync(id, userId);
                return View(session);
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
