using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StudyPlanner.Data;
using StudyPlanner.Models;
using StudyPlanner.ViewModels.Category;


namespace StudyPlanner.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CategoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Category
        public async Task<IActionResult> Index()
        {
           
            var categories = await _context.Categories
                .AsNoTracking()
                .Select(c => new CategoryViewModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    Color = c.Color
                })
                .ToListAsync();

            return View(categories);
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

            var category = new Category
            {
                Name = input.Name,
                Color = input.Color
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Category/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var category = await _context.Categories
                .AsNoTracking()
                .Where(c => c.Id == id)
                .Select(c => new CategoryEditInputModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    Color = c.Color
                })
                .FirstOrDefaultAsync();

            if (category == null)
                return NotFound();

            return View(category);
        }

        // POST: Category/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CategoryEditInputModel input)
        {
            if (!ModelState.IsValid)
                return View(input);

            var category = await _context.Categories.FindAsync(input.Id);

            if (category == null)
                return NotFound();

            
            category.Name = input.Name;
            category.Color = input.Color;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Category/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _context.Categories
                .AsNoTracking()
                .Where(c => c.Id == id)
                .Select(c => new CategoryViewModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    Color = c.Color
                })
                .FirstOrDefaultAsync();

            if (category == null)
                return NotFound();

            return View(category);
        }

        // POST: Category/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            
            var category = new Category { Id = id };
            _context.Categories.Attach(category);
            _context.Categories.Remove(category);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


    }
}
