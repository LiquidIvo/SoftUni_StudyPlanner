using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StudyPlanner.Data;
using StudyPlanner.Models;
using StudyPlanner.ViewModels.StudyTask;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            var tasks = await _context.StudyTasks
           .AsNoTracking()
           .Select(t => new StudyTaskViewModel
           {
               Id = t.Id,
               Description = t.Description,
               Title = t.Title,
               DueDate = t.DueDate,
               Priority = t.Priority.ToString(),
               Status = t.Status.ToString(),
               Category = t.Category.Name,
               CategoryColor = t.Category.Color,
               SubjectColor = t.Subject.Color,
               Subject = t.Subject.Name
           })
           .ToListAsync();

            return View(tasks);
        }

        // GET: StudyTask/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var model = await _context.StudyTasks
             .AsNoTracking()
             .Where(t => t.Id == id)
             .Select(t => new StudyTaskDetailsViewModel
             {
                 Id = t.Id,
                 Title = t.Title,
                 Description = t.Description,
                 DueDate = t.DueDate,
                 Priority = t.Priority.ToString(),
                 Status = t.Status.ToString(),
                 Category = t.Category.Name,
                 CategoryColor = t.Category.Color,
                 Subject = t.Subject.Name,
                 SubjectColor = t.Subject.Color,
                 StudySessions = t.StudySessions
                     .Select(s => new StudySessionItemViewModel
                     {
                         Id = s.Id,
                         StartTime = s.StartTime,
                         EndTime = s.EndTime,
                         Notes = s.Notes
                     })
                     .ToList()
             })
             .FirstOrDefaultAsync();

            if (model == null) return NotFound();

            return View(model);
        }

        // GET: StudyTask/Create
        public IActionResult Create()
        {
            LoadDropdowns();
            return View(new StudyTaskCreateModel());
        }

        // POST: StudyTask/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(StudyTaskCreateModel model)
        {
            if (!ModelState.IsValid)
            {
                LoadDropdowns();
                return View(model);
            }

            var task = new StudyTask
            {
                Title = model.Title,
                Description = model.Description,
                DueDate = model.DueDate,
                Priority = model.Priority,
                Status = model.Status,
                CategoryId = model.CategoryId,
                SubjectId = model.SubjectId
            };

            _context.StudyTasks.Add(task);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: StudyTask/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var model = await _context.StudyTasks
            .AsNoTracking()
            .Where(t => t.Id == id)
            .Select(t => new StudyTaskEditModel
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                DueDate = t.DueDate,
                Priority = t.Priority,
                Status = t.Status,
                CategoryId = t.CategoryId,
                SubjectId = t.SubjectId
            })
            .FirstOrDefaultAsync();

            if (model == null) return NotFound();

            LoadDropdowns();
            return View(model);
        }

        // POST: StudyTask/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(StudyTaskEditModel model)
        {
            if (!ModelState.IsValid)
            {
                LoadDropdowns();
                return View(model);
            }

            var task = await _context.StudyTasks.FindAsync(model.Id);
            if (task == null) return NotFound();

            task.Title = model.Title;
            task.Description = model.Description;
            task.DueDate = model.DueDate;
            task.Priority = model.Priority;
            task.Status = model.Status;
            task.CategoryId = model.CategoryId;
            task.SubjectId = model.SubjectId;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: StudyTask/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var model = await _context.StudyTasks
                .AsNoTracking()
                .Where(t => t.Id == id)
                .Select(t => new StudyTaskViewModel
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    Priority = t.Priority.ToString(),
                    Status = t.Status.ToString(),
                    Category = t.Category.Name,
                    Subject = t.Subject.Name
                })
                .FirstOrDefaultAsync();

            if (model == null) return NotFound();

            return View(model);
        }

        // POST: StudyTask/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var task = await _context.StudyTasks.FindAsync(id);
            if (task == null) return NotFound();

            _context.StudyTasks.Remove(task);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private void LoadDropdowns()
        {
            ViewBag.CategoryId = new SelectList(_context.Categories.AsNoTracking(), "Id", "Name");
            ViewBag.SubjectId = new SelectList(_context.Subjects.AsNoTracking(), "Id", "Name");
        }
    }
}