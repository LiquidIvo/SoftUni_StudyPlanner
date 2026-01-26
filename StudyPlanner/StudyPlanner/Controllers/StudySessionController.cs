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
    public class StudySessionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StudySessionController(ApplicationDbContext context)
        {
            _context = context;
        }

      

       

        // GET: StudySession/Create
        public IActionResult Create(int studyTaskId)
        {

            var task = _context.StudyTasks.Find(studyTaskId);
            if (task == null) return NotFound();

            
            ViewBag.StudyTaskId = studyTaskId;
            ViewBag.StudyTaskTitle = task.Title; 
            return View();
        }

        // POST: StudySession/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int studyTaskId,[Bind("StartTime,EndTime,Notes")] StudySession studySession)
        {
            studySession.StudyTaskId = studyTaskId;
            ModelState.Remove("StudyTask");

            if (ModelState.IsValid)
            {
                _context.Add(studySession);
                await _context.SaveChangesAsync();

                return RedirectToAction(
                    "Details",
                    "StudyTask",
                    new { id = studyTaskId });
            }

            ViewBag.StudyTaskId = studyTaskId;
            return View(studySession);
        }

        // GET
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var studySession = await _context.StudySessions.FindAsync(id);
            if (studySession == null) return NotFound();

            ViewBag.StudyTaskId = studySession.StudyTaskId;
            var task = await _context.StudyTasks.FindAsync(studySession.StudyTaskId);
            ViewBag.StudyTaskTitle = task?.Title;

            return View(studySession);
        }

        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,StartTime,EndTime,Notes")] StudySession studySession)
        {
            if (id != studySession.Id) return NotFound();
            ModelState.Remove("StudyTask");
            var existingSession = await _context.StudySessions.FindAsync(id);
            if (existingSession == null) return NotFound();

            existingSession.StartTime = studySession.StartTime;
            existingSession.EndTime = studySession.EndTime;
            existingSession.Notes = studySession.Notes;

            if (ModelState.IsValid)
            {
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "StudyTask", new { id = existingSession.StudyTaskId });
            }

            ViewBag.StudyTaskId = existingSession.StudyTaskId;
            return View(existingSession);
        }


        // GET: StudySession/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var studySession = await _context.StudySessions
                .Include(s => s.StudyTask)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (studySession == null)
            {
                return NotFound();
            }

            return View(studySession);
        }

        // POST: StudySession/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var studySession = await _context.StudySessions.FindAsync(id);
            if (studySession == null)
            {
                return NotFound();
            }

            int studyTaskId = studySession.StudyTaskId;

            _context.StudySessions.Remove(studySession);
            await _context.SaveChangesAsync();

            return RedirectToAction(
        "Details",
        "StudyTask",
         new { id = studyTaskId });
        }

        private bool StudySessionExists(int id)
        {
            return _context.StudySessions.Any(e => e.Id == id);
        }
    }
}
