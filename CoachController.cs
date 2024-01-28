using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using wmbaApp.Data;
using wmbaApp.Models;

namespace wmbaApp.Controllers
{
    public class CoachController : Controller
    {
        private readonly WmbaContext _context;

        public CoachController(WmbaContext context)
        {
            _context = context;
        }

        // GET: Coach
        public async Task<IActionResult> Index()
        {
              return View(await _context.Coaches.ToListAsync());
        }

        // GET: Coach/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Coaches == null)
            {
                return NotFound();
            }

            var coach = await _context.Coaches
                .FirstOrDefaultAsync(m => m.ID == id);
            if (coach == null)
            {
                return NotFound();
            }

            return View(coach);
        }

        // GET: Coach/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Coach/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,CoachFirstName,CoachLastName")] Coach coach)
        {
            if (ModelState.IsValid)
            {
                _context.Add(coach);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(coach);
        }

        // GET: Coach/Edit/5
       public async Task<IActionResult> Edit(int? id)
 {
     if (id == null || _context.Coaches == null)
     {
         return NotFound();
     }

     var coach = await _context.Coaches.FindAsync(id);
     if (coach == null)
     {
         return NotFound();
     }

     // Use TryUpdateModelAsync to update the model from the request data
     if (await TryUpdateModelAsync(coach, "", c => c.CoachFirstName, c => c.CoachLastName ))
     {
         try
         {
            
             await _context.SaveChangesAsync();
             return View(coach); // Return the edited coach to the same Edit view
         }
         catch (DbUpdateConcurrencyException)
         {
             if (!CoachExists(coach.ID))
             {
                 return NotFound();
             }
             else
             {
                 throw;
             }
         }
         catch (DbUpdateException)
         {
             // Handle database update exception, if needed
             ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
         }
     }

     return View(coach);
 }

        // POST: Coach/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,CoachFirstName,CoachLastName")] Coach coach)
        {
            if (id != coach.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(coach);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CoachExists(coach.ID))
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
            return View(coach);
        }

        // GET: Coach/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Coaches == null)
            {
                return NotFound();
            }

            var coach = await _context.Coaches
                .FirstOrDefaultAsync(m => m.ID == id);
            if (coach == null)
            {
                return NotFound();
            }

            return View(coach);
        }

        // POST: Coach/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Coaches == null)
            {
                return Problem("Entity set 'WmbaContext.Coaches'  is null.");
            }
            var coach = await _context.Coaches.FindAsync(id);
            if (coach != null)
            {
                _context.Coaches.Remove(coach);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CoachExists(int id)
        {
          return _context.Coaches.Any(e => e.ID == id);
        }
    }
}
