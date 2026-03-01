using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StudyPlanner.Services.Core.Contracts;
using StudyPlanner.Services.Core.Models.Category;
using StudyPlanner.ViewModels.Category;

namespace StudyPlanner.Web.Controllers
{
    [Authorize]
    public class CategoryController : Controller
    {
       
        private readonly ICategoryService _categoryService;
        private readonly UserManager<IdentityUser> _userManager;

        public CategoryController(ICategoryService categoryService, UserManager<IdentityUser> userManager)
        {
            _categoryService = categoryService;
            _userManager = userManager;
        }

            private string? GetCurrentUserId()
            {
                return _userManager.GetUserId(User);
            }

        // GET: Category
        public async Task<IActionResult> Index()
        {
            var userId = GetCurrentUserId();

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();


            var dtos = await _categoryService.GetAllCategoriesAsync(userId);

            var viewModels = dtos.Select(d => new CategoryViewModel
            {
                Id = d.Id,
                Name = d.Name,
                Color = d.Color
            }).ToList();

            return View(viewModels);
        }

        // GET: Category/Create
        public IActionResult Create()
        {
            return View(new CategoryCreateInputModel());
        }

        // POST: Category/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryCreateInputModel input)
        {
            if (!ModelState.IsValid)
                return View(input);

            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

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

        // GET: Category/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

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

        // POST: Category/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CategoryEditInputModel input)
        {
            if (!ModelState.IsValid)
                return View(input);

            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

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

        // GET: Category/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

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

        // POST: Category/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

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
