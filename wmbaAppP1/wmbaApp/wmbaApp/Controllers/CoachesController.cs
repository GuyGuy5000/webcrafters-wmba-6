using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using wmbaApp.CustomControllers;
using wmbaApp.Data;
using wmbaApp.Models;
using wmbaApp.Utilities;

namespace wmbaApp.Controllers
{
    public class CoachesController : ElephantController
    {
        private readonly WmbaContext _context;

        public CoachesController(WmbaContext context)
        {
            _context = context;
        }

        // GET: Coaches
        public async Task<IActionResult> Index(string SearchString, int? DivisionID, int? TeamID,
             int? page, int? pageSizeID, string actionButton, string sortDirection = "asc", string sortField = "")
        {
            //Count the number of filters applied - start by assuming no filters
            ViewData["Filtering"] = "btn-outline-secondary";
            int numberFilters = 0;
            //Then in each "test" for filtering, add to the count of Filters applied

            //List of sort options.
            //NOTE: make sure this array has matching values to the column headings
            string[] sortOptions = new[] { "Full Name", "Email", "Phone" };

            PopulateDropDownLists();

            var coaches = _context.Coaches
                .Include(c => c.DivisionCoaches).ThenInclude(c => c.Division)
                .Include(c => c.DivisionCoaches).ThenInclude(c => c.Team)
                .AsNoTracking();

            //Add as many filters as needed
            if (DivisionID.HasValue)
            {
                coaches = coaches.Where(c => c.DivisionCoaches.Any(t => t.DivisionID == DivisionID));
            }
            if (TeamID.HasValue)
            {
                coaches = coaches.Where(c => c.DivisionCoaches.Any(t => t.TeamID == TeamID));
            }

            if (!System.String.IsNullOrEmpty(SearchString))
            {
                coaches = coaches.Where(c => c.CoachFirstName.ToUpper().Contains(SearchString.ToUpper())
                                          || c.CoachLastName.ToUpper().Contains(SearchString.ToUpper())
                                          || c.CoachEmail.ToUpper().Contains(SearchString.ToUpper())
                                          || c.CoachPhone.ToUpper().Contains(SearchString.ToUpper())
                                       );
                numberFilters++;
            }
            //Give feedback about the state of the filters
            if (numberFilters != 0)
            {
                //Toggle the Open/Closed state of the collapse depending on if we are filtering
                ViewData["Filtering"] = " btn-danger";
                //Show how many filters have been applied
                ViewData["numberFilters"] = "(" + numberFilters.ToString()
                    + " Filter" + (numberFilters > 1 ? "s" : "") + " Applied)";
            }
            if (!System.String.IsNullOrEmpty(actionButton)) //Form Submitted!
            {
                page = 1;//Reset page to start

                if (sortOptions.Contains(actionButton))//Change of sort is requested
                {
                    if (actionButton == sortField) //Reverse order on same field
                    {
                        sortDirection = sortDirection == "asc" ? "desc" : "asc";
                    }
                    sortField = actionButton;//Sort by the button clicked
                }
            }

            if (sortField == "Full Name")
            {

                if (sortDirection == "asc")
                {
                    coaches = coaches
                        .OrderBy(c => c.CoachFirstName);
                }
                else
                {
                    coaches = coaches
                        .OrderByDescending(c => c.CoachFirstName);
                }
            }
            else if (sortField == "Email")
            {

                if (sortDirection == "asc")
                {
                    coaches = coaches
                        .OrderBy(c => c.CoachEmail);
                }
                else
                {
                    coaches = coaches
                        .OrderByDescending(c => c.CoachEmail);
                }
            }
            else
            {
                if (sortDirection == "asc")
                {
                    coaches = coaches
                        .OrderBy(c => c.CoachPhone);
                }
                else
                {
                    coaches = coaches
                        .OrderByDescending(c => c.CoachPhone);
                }
            }

            ViewData["sortField"] = sortField;
            ViewData["sortDirection"] = sortDirection;

            int pageSize = PageSizeHelper.SetPageSize(HttpContext, pageSizeID, ControllerName());
            ViewData["pageSizeID"] = PageSizeHelper.PageSizeList(pageSize);
            var pagedData = await PaginatedList<Coach>.CreateAsync(coaches.AsNoTracking(), page ?? 1, pageSize);

            return View(pagedData);
        }

        // GET: Coaches/Details/5
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

        // GET: Coaches/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Coaches/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,CoachFirstName,CoachLastName,CoachEmail,CoachPhone")] Coach coach)
        {
            if (ModelState.IsValid)
            {
                _context.Add(coach);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(coach);
        }

        // GET: Coaches/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Coaches == null)
            {
                return NotFound();
            }

            var coach = await _context.Coaches
                .Include(c => c.DivisionCoaches).ThenInclude(c => c.Division)
                .Include(c => c.DivisionCoaches).ThenInclude(c => c.Team)
                .FirstOrDefaultAsync(c => c.ID == id);

            if (coach == null)
            {
                return NotFound();
            }
            return View(coach);
        }

        // POST: Coaches/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id)
        {
            var coachToUpdate = await _context.Coaches
                .Include(c => c.DivisionCoaches).ThenInclude(c => c.Division)
                .Include(c => c.DivisionCoaches).ThenInclude(c => c.Team)
                .FirstOrDefaultAsync(c => c.ID == id);

            if (coachToUpdate == null)
            {
                return NotFound();
            }

            if (await TryUpdateModelAsync<Coach>(coachToUpdate, "",
                c => c.CoachFirstName, c => c.CoachLastName, c => c.CoachEmail, c => c.CoachPhone))
            {
                try
                {
                    _context.Update(coachToUpdate);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CoachExists(coachToUpdate.ID))
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
            return View(coachToUpdate);
        }

        // GET: Coaches/Delete/5
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

        // POST: Coaches/Delete/5
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

        #region SelectLists
        private SelectList DivisionSelectList(int? selectedId)
        {
            return new SelectList(_context.Divisions, "ID", "DivName", selectedId);
        }
        private SelectList TeamSelectList(int? selectedId)
        {
            return new SelectList(_context.Teams, "ID", "TmName", selectedId);
        }
        private void PopulateDropDownLists(Coach coach = null)
        {
            ViewData["DivisionID"] = DivisionSelectList(coach?.DivisionCoaches.FirstOrDefault(c => c.DivisionID == coach?.ID)?.DivisionID);
            ViewData["TeamID"] = TeamSelectList(coach?.DivisionCoaches.FirstOrDefault(c => c.DivisionID == coach?.ID)?.TeamID);
        }
        #endregion

        private bool CoachExists(int id)
        {
          return _context.Coaches.Any(e => e.ID == id);
        }
    }
}
