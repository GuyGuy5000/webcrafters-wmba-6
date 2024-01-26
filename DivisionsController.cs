using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using wmbaApp.Data;
using wmbaApp.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace wmbaApp.Controllers
{
    public class DivisionsController : Controller
    {
        private readonly WmbaContext _context;

        public DivisionsController(WmbaContext context)
        {
            _context = context;
        }

        // GET: Divisions
        public async Task<IActionResult> Index()
        {
              return View(await _context.Divisions.ToListAsync());
        }

        // GET: Divisions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Divisions == null)
            {
                return NotFound();
            }

            var division = await _context.Divisions
                .FirstOrDefaultAsync(m => m.ID == id);
            if (division == null)
            {
                return NotFound();
            }

            return View(division);
        }

        // GET: Divisions/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Divisions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,DivName")] Division division)
        {
            if (ModelState.IsValid)
            {
                // Check if the division already exists by name
                if (!_context.Divisions.Any(d => d.DivName == division.DivName))
                {
                    // Division does not exist, add it
                    _context.Add(division);
                    await _context.SaveChangesAsync();
                    TempData["DivisionID"] = division.ID;

                    return RedirectToAction("Create", "Teams");
                }
                else
                {
                    // Division with the same name already exists, add a model error
                    ModelState.AddModelError(string.Empty, "A division with the same name already exists.");
                    return RedirectToAction(nameof(Index));
                }
            }

            return View(division);

        }

        // GET: Divisions/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            if (id == null || _context.Divisions == null)
            {
                return NotFound();
            }

            var division = await _context.Divisions.FindAsync(id);
            if (division == null)
            {
                return NotFound();
            }
            return View(division);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,DivName")] Division division)
        {
            if (id != division.ID)
            {
                return NotFound();
            }

            // Check if the new division name already exists
            var existingDivision = await _context.Divisions.FirstOrDefaultAsync(d => d.DivName == division.DivName && d.ID != division.ID);
            if (existingDivision != null)
            {
                ModelState.AddModelError("division.DivName", "Division name already exists.");
                return View(division);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(division);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DivisionExists(division.ID))
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
            return View(division);
        }

        // GET: Divisions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Divisions == null)
            {
                return NotFound();
            }

            var division = await _context.Divisions
                .FirstOrDefaultAsync(m => m.ID == id);
            if (division == null)
            {
                return NotFound();
            }

            return View(division);
        }

        // POST: Divisions/Delete/5
        [HttpPost, ActionName("Delete")]
[ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Divisions == null)
            {
                return Problem("Entity set 'WmbaContext.Divisions' is null.");
            }

            var division = await _context.Divisions
                .Include(d => d.Teams) // Include the related teams
                .FirstOrDefaultAsync(m => m.ID == id);

            if (division != null)
            {
                if (division.Teams != null && division.Teams.Any())
                {
                    // Division has associated teams, set an error message in ViewBag

                    return RedirectToAction(nameof(Index));
                }

                _context.Divisions.Remove(division);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }


        private bool DivisionExists(int id)
        {
          return _context.Divisions.Any(e => e.ID == id);
        }
    }
}
