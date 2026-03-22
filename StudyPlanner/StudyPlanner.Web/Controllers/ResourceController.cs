using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StudyPlanner.Data.Models;
using StudyPlanner.Services.Core.Contracts;
using StudyPlanner.Services.Core.Models.Resource;
using StudyPlanner.ViewModels.Resource;
using static StudyPlanner.GCommon.ApplicationConstants;

namespace StudyPlanner.Web.Controllers
{
    [Authorize(Roles = AdminOrUser)]
    public class ResourceController : Controller
    {
        private readonly IResourceService _resourceService;
        private readonly UserManager<ApplicationUser> _userManager;

        public ResourceController(
            IResourceService resourceService,
            UserManager<ApplicationUser> userManager)
        {
            _resourceService = resourceService;
            _userManager = userManager;
        }

        private Guid GetCurrentUserId()
        {
            var userId = _userManager.GetUserId(User);
            return Guid.Parse(userId!);
        }

       
        [HttpGet]
        public async Task<IActionResult> Index(string? searchTerm, int pageNumber = PageNumber)
        {
            var userId = GetCurrentUserId();

            var (dtos, totalCount) = await _resourceService
                .GetAllResourcesAsync(userId, searchTerm, pageNumber, PageSize);

            var viewModels = dtos.Select(d => new ResourceViewModel
            {
                Id = d.Id,
                Title = d.Title,
                Url = d.Url,
                Description = d.Description
            }).ToList();

            ViewData["SearchTerm"] = searchTerm;
            ViewData["PageNumber"] = pageNumber;
            ViewData["TotalPages"] = (int)Math.Ceiling((double)totalCount / PageSize);

            return View(viewModels);
        }

        
        [HttpGet]
        public IActionResult Create()
        {
            return View(new ResourceCreateInputModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ResourceCreateInputModel input)
        {
            if (!ModelState.IsValid)
                return View(input);

            var userId = GetCurrentUserId();

            try
            {
                var dto = new ResourceCreateDTO
                {
                    Title = input.Title,
                    Url = input.Url,
                    Description = input.Description
                };

                await _resourceService.CreateResourceAsync(dto, userId);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "An error occurred while creating the resource.");
                return View(input);
            }
        }

        
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var userId = GetCurrentUserId();

            try
            {
                var dto = await _resourceService.GetResourceByIdAsyncForEdit(id, userId);

                var viewModel = new ResourceEditInputModel
                {
                    Id = dto.Id,
                    Title = dto.Title,
                    Url = dto.Url,
                    Description = dto.Description
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
        public async Task<IActionResult> Edit(ResourceEditInputModel input)
        {
            if (!ModelState.IsValid)
                return View(input);

            var userId = GetCurrentUserId();

            try
            {
                var dto = new ResourceEditDTO
                {
                    Id = input.Id,
                    Title = input.Title,
                    Url = input.Url,
                    Description = input.Description
                };

                await _resourceService.UpdateResourceAsync(dto, userId);
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
                var dto = await _resourceService.GetResourceByIdAsync(id, userId);

                var viewModel = new ResourceViewModel
                {
                    Id = dto.Id,
                    Title = dto.Title,
                    Url = dto.Url,
                    Description = dto.Description
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
                await _resourceService.DeleteResourceAsync(id, userId);
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
    }
}