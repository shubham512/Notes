using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NotesApp.Data;
using NotesApp.Models;

namespace NotesApp.Controllers
{
    public class NotesController : Controller
    {
        private readonly NotesAppDbContext _context;

        public NotesController(NotesAppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var notes = await _context.Notes
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
            
            return View(notes);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Content,Priority")] Note note)
        {
            if (!ModelState.IsValid)
            {
                var notes = await _context.Notes.OrderByDescending(n => n.CreatedAt).ToListAsync();
                return View("Index", notes);
            }
            
            note.CreatedAt = DateTime.UtcNow;
            note.UpdatedAt = null;
            _context.Add(note);
            await _context.SaveChangesAsync();
            
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();
            
            var note = await _context.Notes.FindAsync(id.Value);
            
            if (note == null) 
                return NotFound();
            
            return View(note);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Content,Priority")] Note input)
        {
            if (id != input.Id) 
                return BadRequest();

            if (!ModelState.IsValid)
            {
                return View(input);
            }

            var note = await _context.Notes.FindAsync(id);
            if (note == null) 
                return NotFound();

            note.Title = input.Title;
            note.Content = input.Content;
            note.Priority = input.Priority;
            note.UpdatedAt = DateTime.UtcNow;

            try
            {
                _context.Update(note);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Notes.AnyAsync(n => n.Id == id))
                    return NotFound();
                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var note = await _context.Notes.FindAsync(id);
            if (note != null)
            {
                _context.Notes.Remove(note);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}