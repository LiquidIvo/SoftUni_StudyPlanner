using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StudyPlanner.Data.Models;
using StudyPlanner.Services.Core.Contracts;
using StudyPlanner.Services.Core.Models.Category;
using StudyPlanner.ViewModels.Category;
using static StudyPlanner.GCommon.ApplicationConstants;
namespace StudyPlanner.Web.Controllers
{
    [Authorize(Roles = AdminOrUser)]
    public class CategoryController : Controller
    {
       
        private readonly ICategoryService _categoryService;
        private readonly UserManager<ApplicationUser> _userManager;

        public CategoryController(ICategoryService categoryService, UserManager<ApplicationUser> userManager)
        {
            _categoryService = categoryService;
            _userManager = userManager;
        }

            private Guid GetCurrentUserId()
            {
                var userId = _userManager.GetUserId(User);
                return Guid.Parse(userId!);
            }

        [HttpGet]
        public async Task<IActionResult> Index(string? searchTerm,int pageNumber = PageNumber)
        {
            var userId = GetCurrentUserId();
            

            var (dtos, totalCount) = await _categoryService.GetAllCategoriesAsync(userId,searchTerm,pageNumber,PageSize);
            
            var viewModels = dtos.Select(d => new CategoryViewModel
            {
                Id = d.Id,
                Name = d.Name,
                Color = d.Color
            }).ToList();

            ViewData["SearchTerm"] = searchTerm;
            ViewData["PageNumber"] = pageNumber;
            ViewData["TotalPages"] = (int)Math.Ceiling((double)totalCount / PageSize);

            return View(viewModels);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new CategoryCreateInputModel());
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryCreateInputModel input)
        {
            if (!ModelState.IsValid)
                return View(input);

            var userId = GetCurrentUserId();
           

            try
            {
                var dto = new CategoryCreateDTO
                {
                    Name = input.Name,
                    Color = input.Color
                };

                await _categoryService.CreateCategoryAsync(dto, userId);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "An error occurred while creating the category.");
                return View(input);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var userId = GetCurrentUserId();
           

            try
            {
                var dto = await _categoryService.GetCategoryByIdAsyncForEdit(id, userId);

                var viewModel = new CategoryEditInputModel
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
        public async Task<IActionResult> Edit(CategoryEditInputModel input)
        {
            if (!ModelState.IsValid)
                return View(input);

            var userId = GetCurrentUserId();
          

            try
            {
                var dto = new CategoryEditDTO
                {
                    Id = input.Id,
                    Name = input.Name,
                    Color = input.Color
                };

                await _categoryService.UpdateCategoryAsync(dto, userId);
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
                var dto = await _categoryService.GetCategoryByIdAsync(id, userId);

                var viewModel = new CategoryViewModel
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
                
                await _categoryService.DeleteCategoryAsync(id, userId);
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                var dto = await _categoryService.GetCategoryByIdAsync(id, userId);

                var viewModel = new CategoryViewModel
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
