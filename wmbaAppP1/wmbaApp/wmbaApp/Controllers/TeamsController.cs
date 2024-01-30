using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    public class TeamsController : ElephantController
    {
        private readonly WmbaContext _context;

        public TeamsController(WmbaContext context)
        {
            _context = context;
        }

        // GET: Teams
        public async Task<IActionResult> Index(string SearchString, int? DivisionID,
             int? page, int? pageSizeID, string actionButton, string sortDirection = "asc", string sortField = "")
        {
            //Count the number of filters applied - start by assuming no filters
            ViewData["Filtering"] = "btn-outline-secondary";
            int numberFilters = 0;
            //Then in each "test" for filtering, add to the count of Filters applied

            //List of sort options.
            //NOTE: make sure this array has matching values to the column headings
            string[] sortOptions = new[] { "Name", "ABBR", "Division", "Coaches", "Players" };
            PopulateDropDownLists();

            var teams = _context.Teams
            .Include(t => t.Division)
            .Include(t => t.DivisionCoaches).ThenInclude(t => t.Coach)
            .Include(t => t.Players)
            .Include(t => t.GameTeams).ThenInclude(t => t.Game)
            .AsNoTracking();

            //Add as many filters as needed
            if (DivisionID.HasValue)
            {
                teams = teams.Where(t => t.DivisionID == DivisionID);
                numberFilters++;
            }

            if (!System.String.IsNullOrEmpty(SearchString))
            {
                teams = teams.Where(p => p.TmName.ToUpper().Contains(SearchString.ToUpper())
                                       || p.TmAbbreviation.ToUpper().Contains(SearchString.ToUpper())
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
                //Keep the Bootstrap collapse open
                //@ViewData["ShowFilter"] = " show";
            }

            //Before we sort, see if we have called for a change of filtering or sorting
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
            //Now we know which field and direction to sort by
            if (sortField == "Name")
            {

                if (sortDirection == "asc")
                {
                    teams = teams
                        .OrderBy(p => p.TmName);
                }
                else
                {
                    teams = teams
                        .OrderByDescending(p => p.TmName);
                }
            }
            else if (sortField == "ABBR")
            {
                if (sortDirection == "asc")
                {
                    teams = teams
                      .OrderBy(p => p.TmAbbreviation);
                }
                else
                {
                    teams = teams
                       .OrderByDescending(p => p.TmAbbreviation);
                }
            }
            else if (sortField == "Division")
            {
                if (sortDirection == "asc")
                {
                    teams = teams
                       .OrderBy(p => p.Division.DivName);

                }
                else
                {
                    teams = teams
                       .OrderByDescending(p => p.Division.DivName);

                }
            }

            //Gets matchups from teams query
            ViewData["Matchups"] = GameMatchup.GetMatchups(_context, teams.ToArray());

            //Set sort for next time
            ViewData["sortField"] = sortField;
            ViewData["sortDirection"] = sortDirection;

            int pageSize = PageSizeHelper.SetPageSize(HttpContext, pageSizeID, ControllerName());
            ViewData["pageSizeID"] = PageSizeHelper.PageSizeList(pageSize);
            var pagedData = await PaginatedList<Team>.CreateAsync(teams.AsNoTracking(), page ?? 1, pageSize);

            // Change the return statement to use the pagedData
            return View(pagedData);
        }

        // GET: Teams/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Teams == null)
            {
                return NotFound();
            }

            var team = await _context.Teams
           .Include(t => t.Division)
           .Include(t => t.DivisionCoaches).ThenInclude(t => t.Coach)
           .Include(t => t.Players)
           .Include(t => t.GameTeams).ThenInclude(t => t.Game)
           .AsNoTracking()
           .FirstOrDefaultAsync(m => m.ID == id);
            if (team == null)
            {
                return NotFound();
            }

            return View(team);
        }

        // GET: Teams/Create
        public IActionResult Create()
        {
            PopulateDropDownLists();
            return View();
        }

        // POST: Teams/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,TmName,TmAbbreviation,DivisionID")] Team team, string submitButton = "")
        {
            PopulateDropDownLists();
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(team);
                    await _context.SaveChangesAsync();
                    if (!String.IsNullOrEmpty(submitButton))
                    {
                        if (submitButton == "Create and add players")
                            return RedirectToAction("Create", "PlayerTeam", new { TeamID = team.ID});
                        else
                            return RedirectToAction("Index", "PlayerTeam", new { TeamID = team.ID });
                    }
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
                            ModelState.AddModelError("TmName", "A team with this name already exists. Please choose a different name."); //pass a message to the field that triggered the error
                        else if (dex.InnerException.Message.Contains("TmAbbreviation"))
                            ModelState.AddModelError("TmAbbreviation", "A team with this abbreviation already exists. Please choose a different abbreviation.");
                    }
                    else
                        ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                }
            }
            return View(team);
        }

        // GET: Teams/Edit/5 
        public async Task<IActionResult> Edit(int? id)
        {
            PopulateDropDownLists();
            if (id == null || _context.Teams == null)
            {
                return NotFound();
            }

            var team = await _context.Teams
            .Include(t => t.Division)
            .Include(t => t.DivisionCoaches).ThenInclude(t => t.Coach)
            .Include(t => t.Players)
            .Include(t => t.GameTeams).ThenInclude(t => t.Game)
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.ID == id);
            if (team == null)
            {
                return NotFound();
            }
            ViewBag.DivisionID = new SelectList(_context.Divisions, "ID", "DivName", team.DivisionID);
            return View(team);
        }

        // POST: Teams/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id)
        {
            PopulateDropDownLists();
            var teamToUpdate = await _context.Teams
                .Include(t => t.Division)
                .Include(t => t.DivisionCoaches).ThenInclude(t => t.Coach)
                .Include(t => t.Players)
                .Include(t => t.GameTeams).ThenInclude(t => t.Game)
                .FirstOrDefaultAsync(m => m.ID == id);

            if (teamToUpdate == null)
            {
                return NotFound();
            }

            if (await TryUpdateModelAsync<Team>(teamToUpdate, "",
                t => t.TmName, t => t.TmAbbreviation, t => t.DivisionID))
            {
                try
                {
                    _context.Update(teamToUpdate);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TeamExists(teamToUpdate.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (DbUpdateException dex)
                {
                    if (dex.InnerException.Message.Contains("UNIQUE")) //if a UNIQUE constraint caused the exception
                    {
                        //check which field triggered the error
                        if (dex.InnerException.Message.Contains("TmName"))
                            ModelState.AddModelError("TmName", "A team with this name already exists. Please choose a different name."); //pass a message to the field that triggered the error
                        else if (dex.InnerException.Message.Contains("TmAbbreviation"))
                            ModelState.AddModelError("TmAbbreviation", "A team with this abbreviation already exists. Please choose a different abbreviation.");
                    }
                    else
                        ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                        return View(teamToUpdate);
                }
            }
            ViewData["DivisionID"] = new SelectList(_context.Divisions, "ID", "DivName", teamToUpdate.DivisionID);

            return RedirectToAction("Details", new { teamToUpdate.ID });
        }

        // GET: Teams/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Teams == null)
            {
                return NotFound();
            }

            var team = await _context.Teams
             .Include(t => t.Division)
             .Include(t => t.DivisionCoaches).ThenInclude(t => t.Coach)
             .Include(t => t.Players)
             .Include(t => t.GameTeams).ThenInclude(t => t.Game)
             .AsNoTracking()
             .FirstOrDefaultAsync(m => m.ID == id);

            if (team == null)
            {
                return NotFound();
            }

            return View(team);
        }

        // POST: Teams/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Teams == null)
            {
                return Problem("Entity set 'WmbaContext.Teams'  is null.");
            }
            var team = await _context.Teams.FindAsync(id);
            try
            {
                if (team != null)
                    _context.Teams.Remove(team);

                await _context.SaveChangesAsync();
                return Redirect(ViewData["returnURL"].ToString());
            }
            catch (DbUpdateException dex)
            {
                if (dex.GetBaseException().Message.Contains("FOREIGN KEY constraint failed"))
                    ModelState.AddModelError("FK", $"Unable to delete a team that has players. Reassign or delete the players assigned to this team.");
                else
                    //Note: there is really no reason a delete should fail if you can "talk" to the database.
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                return View("Delete", team);
            }
        }

        #region SelectLists
        private SelectList DivisionSelectList(int? selectedId)
        {
            return new SelectList(_context.Divisions, "ID", "DivName", selectedId);
        }
        private void PopulateDropDownLists(Team team = null)
        {
            ViewData["DivisionID"] = DivisionSelectList(team?.DivisionID);
        }
        #endregion

        private bool TeamExists(int id)
        {
            return _context.Teams.Any(e => e.ID == id);
        }
    }
}
