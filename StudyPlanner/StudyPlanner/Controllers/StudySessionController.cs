using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StudyPlanner.Data;
using StudyPlanner.Models;
using StudyPlanner.ViewModels.StudySession;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudyPlanner.Controllers
{
    public class StudySessionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StudySessionController(ApplicationDbContext context)
        {
            _context = context;
        }

      

       

        // GET: StudySession/Create
        public async Task<IActionResult> Create(int studyTaskId)
        {
            var taskExists = await _context.StudyTasks
               .AsNoTracking()
               .AnyAsync(t => t.Id == studyTaskId);

            if (!taskExists) return NotFound();

            var model = new StudySessionCreateInputModel
            {
                StudyTaskId = studyTaskId
            };

            return View(model);
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

            var session = new StudySession
            {
                StudyTaskId = input.StudyTaskId,
                StartTime = input.StartTime,
                EndTime = input.EndTime,
                Notes = input.Notes
            };

            _context.StudySessions.Add(session);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", "StudyTask", new { id = input.StudyTaskId });
        }

        // GET
        public async Task<IActionResult> Edit(int id)
        {
            var session = await _context.StudySessions
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == id);

            if (session == null) return NotFound();

            var model = new StudySessionEditInputModel
            {
                Id = session.Id,
                StudyTaskId = session.StudyTaskId,
                StartTime = session.StartTime,
                EndTime = session.EndTime,
                Notes = session.Notes
            };

            return View(model);
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

            var session = await _context.StudySessions.FindAsync(input.Id);
            if (session == null) return NotFound();

            session.StartTime = input.StartTime;
            session.EndTime = input.EndTime;
            session.Notes = input.Notes;

            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "StudyTask", new { id = session.StudyTaskId });
        }


        // GET: StudySession/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var model = await _context.StudySessions
                .AsNoTracking()
                .Where(s => s.Id == id)
                .Select(s => new StudySessionDeleteViewModel
                {
                    Id = s.Id,
                    StudyTaskId = s.StudyTaskId,
                    StartTime = s.StartTime,
                    EndTime = s.EndTime,
                    Notes = s.Notes
                })
                .FirstOrDefaultAsync();

            if (model == null) return NotFound();
            return View(model);
        }

        // POST: StudySession/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var session = await _context.StudySessions
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == id);

            if (session == null) return NotFound();

            _context.StudySessions.Remove(new StudySession { Id = id });
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", "StudyTask", new { id = session.StudyTaskId });
        }

        
    }
}
