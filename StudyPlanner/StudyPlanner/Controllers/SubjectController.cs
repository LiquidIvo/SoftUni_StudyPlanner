using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StudyPlanner.Data;
using StudyPlanner.Models;
using StudyPlanner.ViewModels.Subject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudyPlanner.Controllers
{
    public class SubjectController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SubjectController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Subject
        public async Task<IActionResult> Index()
        {
            var subjects = await _context.Subjects
               .AsNoTracking()
               .Select(s => new SubjectViewModel
               {
                   Id = s.Id,
                   Name = s.Name
               })
               .ToListAsync();

            return View(subjects);
        }

        // GET: Subject/Details/5
       

        // GET: Subject/Create
        public IActionResult Create()
        {
            return View(new SubjectCreateInputModel());
        }

        // POST: Subject/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SubjectCreateInputModel input)
        {
            if (!ModelState.IsValid)
                return View(input);

            var subject = new Subject
            {
                Name = input.Name
            };

            _context.Subjects.Add(subject);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Subject/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var subject = await _context.Subjects.FindAsync(id);
            if (subject == null) return NotFound();

            var model = new SubjectEditInputModel
            {
                Id = subject.Id,
                Name = subject.Name
            };

            return View(model);
        }

        // POST: Subject/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SubjectEditInputModel input)
        {
            if (!ModelState.IsValid) return View(input);

            var subject = await _context.Subjects.FindAsync(input.Id);
            if (subject == null) return NotFound();

            subject.Name = input.Name;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Subject/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var model = await _context.Subjects
                .AsNoTracking()
                .Where(s => s.Id == id)
                .Select(s => new SubjectViewModel
                {
                    Id = s.Id,
                    Name = s.Name
                })
                .FirstOrDefaultAsync();

            if (model == null) return NotFound();

            return View(model);
        }

        // POST: Subject/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var subject = await _context.Subjects.FindAsync(id);
            if (subject != null)
            {
                _context.Subjects.Remove(subject);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

       
    }
}
