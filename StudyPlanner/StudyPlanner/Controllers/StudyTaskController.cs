using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StudyPlanner.Data;
using StudyPlanner.Models;

namespace StudyPlanner.Controllers
{
    public class StudyTaskController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StudyTaskController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: StudyTask
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.StudyTasks.Include(s => s.Category).Include(s => s.Subject);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: StudyTask/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var studyTask = await _context.StudyTasks
                .Include(s => s.Category)
                .Include(s => s.Subject)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (studyTask == null)
            {
                return NotFound();
            }

            return View(studyTask);
        }

        // GET: StudyTask/Create
        public IActionResult Create()
        {
            ViewBag.CategoryId = new SelectList(_context.Categories, "Id", "Name");
            ViewBag.SubjectId = new SelectList(_context.Subjects, "Id", "Name");
            return View();
        }

        // POST: StudyTask/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,DueDate,Priority,Status,CategoryId,SubjectId")] StudyTask studyTask)
        {
            // Remove navigation properties from ModelState validation
            ModelState.Remove("Category");
            ModelState.Remove("Subject");
            ModelState.Remove("StudySessions");

            if (ModelState.IsValid)
            {
                _context.Add(studyTask);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.CategoryId = new SelectList(_context.Categories, "Id", "Name", studyTask.CategoryId);
            ViewBag.SubjectId = new SelectList(_context.Subjects, "Id", "Name", studyTask.SubjectId);
            return View(studyTask);
        }

        // GET: StudyTask/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var studyTask = await _context.StudyTasks.FindAsync(id);
            if (studyTask == null)
            {
                return NotFound();
            }
            ViewBag.CategoryId = new SelectList(_context.Categories, "Id", "Name", studyTask.CategoryId);
            ViewBag.SubjectId = new SelectList(_context.Subjects, "Id", "Name", studyTask.SubjectId);
            return View(studyTask);
        }

        // POST: StudyTask/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,DueDate,Priority,Status,CategoryId,SubjectId")] StudyTask studyTask)
        {
            if (id != studyTask.Id)
            {
                return NotFound();
            }

            // Remove navigation properties from ModelState validation
            ModelState.Remove("Category");
            ModelState.Remove("Subject");
            ModelState.Remove("StudySessions");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(studyTask);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudyTaskExists(studyTask.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.CategoryId = new SelectList(_context.Categories, "Id", "Name", studyTask.CategoryId);
            ViewBag.SubjectId = new SelectList(_context.Subjects, "Id", "Name", studyTask.SubjectId);
            return View(studyTask);
        }

        // GET: StudyTask/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var studyTask = await _context.StudyTasks
                .Include(s => s.Category)
                .Include(s => s.Subject)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (studyTask == null)
            {
                return NotFound();
            }

            return View(studyTask);
        }

        // POST: StudyTask/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var studyTask = await _context.StudyTasks.FindAsync(id);
            if (studyTask != null)
            {
                _context.StudyTasks.Remove(studyTask);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StudyTaskExists(int id)
        {
            return _context.StudyTasks.Any(e => e.Id == id);
        }
    }
}