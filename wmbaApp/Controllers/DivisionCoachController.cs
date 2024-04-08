using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using wmbaApp.CustomControllers;
using wmbaApp.Data;
using wmbaApp.Models;
using wmbaApp.Utilities;

namespace wmbaApp.Controllers
{
    public class DivisionCoachController : ElephantController
    {
        private readonly WmbaContext _context;
        private readonly ApplicationDbContext _applicationDbContext;

        public DivisionCoachController(WmbaContext context, ApplicationDbContext applicationDbContext)
        {
            _context = context;
            _applicationDbContext = applicationDbContext;
        }

        // GET: DivisionCoach
        public async Task<IActionResult> Index(int? page, int? pageSizeID)
        {
            var divisionCoaches = _context.DivisionCoaches
                .Include(d => d.Coach)
                .Include(d => d.Division)
                .Include(d => d.Team)
                .AsNoTracking();

            int pageSize = PageSizeHelper.SetPageSize(HttpContext, pageSizeID, ControllerName());
            ViewData["pageSizeID"] = PageSizeHelper.PageSizeList(pageSize);
            var pagedData = await PaginatedList<DivisionCoach>.CreateAsync(divisionCoaches.AsNoTracking(), page ?? 1, pageSize);

            // Change the return statement to use the pagedData
            return View(pagedData);
        }

        // GET: DivisionCoach/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.DivisionCoaches == null)
            {
                return NotFound();
            }

            var divisionCoach = await _context.DivisionCoaches
                .Include(d => d.Coach)
                .Include(d => d.Division)
                .Include(d => d.Team)
                 .AsNoTracking()
                .FirstOrDefaultAsync(m => m.DivisionID == id);
            if (divisionCoach == null)
            {
                return NotFound();
            }

            return View(divisionCoach);
        }

        // GET: DivisionCoach/Create
        public IActionResult Create()
        {
           
            return View();
        }

        // POST: DivisionCoach/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DivisionID,CoachID,TeamID")] DivisionCoach divisionCoach)
        {
            PopulateDropDownLists();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(divisionCoach);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }

                catch (RetryLimitExceededException /* dex */)
                {
                    ModelState.AddModelError("", "Unable to save changes after multiple attempts. Try again, and if the problem persists, see your system administrator.");
                }
                catch (DbUpdateException dex)
                {
                    if (dex.InnerException.Message.Contains("UNIQUE")) //if a UNIQUE constraint caused the exception
                    {
                        //check which field triggered the error
                        if (dex.InnerException.Message.Contains("TmName"))
                            ModelState.AddModelError("DivName", "A Division with this name already exists. Please choose a different name."); //pass a message to the field that triggered the error
                        else if (dex.InnerException.Message.Contains("TmAbbreviation"))
                            ModelState.AddModelError("TmAbbreviation", "A team with this abbreviation already exists. Please choose a different abbreviation.");
                    }
                    else
                        ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                }
            }

            PopulateDropDownLists();
            return View(divisionCoach);
        }

        // GET: DivisionCoach/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.DivisionCoaches == null)
            {
                return NotFound();
            }

    
            

            var divisionCoach = await _context.DivisionCoaches
               .Include(d => d.Coach)
               .Include(d => d.Division)
               .Include(d => d.Team)
                
               .FirstOrDefaultAsync(m => m.DivisionID == id);
            if (divisionCoach == null)
            {
                return NotFound();
            }
            PopulateDropDownLists();
            return View(divisionCoach);
        }

        // POST: DivisionCoach/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id)
        {
            PopulateDropDownLists();
            var divisionCoach = await _context.DivisionCoaches
              .Include(d => d.Coach)
              .Include(d => d.Division)
              .Include(d => d.Team)
              .FirstOrDefaultAsync(d => d.DivisionID == id);

            if (id != divisionCoach.DivisionID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(divisionCoach);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DivisionCoachExists(divisionCoach.DivisionID))
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
            PopulateDropDownLists();
            return View(divisionCoach);
        }

        // GET: DivisionCoach/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.DivisionCoaches == null)
            {
                return NotFound();
            }

            var divisionCoach = await _context.DivisionCoaches
                .Include(d => d.Coach)
                .Include(d => d.Division)
                .Include(d => d.Team)
                .FirstOrDefaultAsync(m => m.DivisionID == id);
            if (divisionCoach == null)
            {
                return NotFound();
            }

            return View(divisionCoach);
        }

        // POST: DivisionCoach/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.DivisionCoaches == null)
            {
                return Problem("There are no Division Coaches to delete");
            }
            var divisionCoach = await _context.DivisionCoaches.FindAsync(id);
            try
            {
                if (divisionCoach != null)
                {
                    _context.DivisionCoaches.Remove(divisionCoach);
                }

                await _context.SaveChangesAsync();
                return Redirect(ViewData["returnURL"].ToString());
            }
            catch (DbUpdateException dex)
            {
                if (dex.GetBaseException().Message.Contains("FOREIGN KEY constraint failed"))
                    ModelState.AddModelError("FK", $"Unable to delete a division that has coaches. Reassign or delete the divisions assigned to this coach.");
                else
                    //Note: there is really no reason a delete should fail if you can "talk" to the database.
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                return View("Delete", divisionCoach);
            }
        }



        private void PopulateDropDownLists(DivisionCoach  divisionCoach = null)
        {
            ViewData["CoachID"] = new SelectList(_context.Coaches, "ID", "ID");
            ViewData["DivisionID"] = new SelectList(_context.Divisions, "ID", "DivName");
            ViewData["TeamID"] = new SelectList(_context.Teams, "ID", "TmName");
        }
        private bool DivisionCoachExists(int id)
        {
          return _context.DivisionCoaches.Any(e => e.DivisionID == id);
        }
    }
}
