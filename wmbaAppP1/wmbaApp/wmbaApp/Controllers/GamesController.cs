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
    public class GamesController : ElephantController
    {
        private readonly WmbaContext _context;

        public GamesController(WmbaContext context)
        {
            _context = context;
        }

        // GET: Games
        public async Task<IActionResult> Index(string SearchString, int? ID,
             int? page, int? pageSizeID, string actionButton, string sortDirection = "asc", string sortField = "")
        {
            //Count the number of filters applied - start by assuming no filters
            ViewData["Filtering"] = "btn-outline-secondary";
            int numberFilters = 0;
            //Then in each "test" for filtering, add to the count of Filters applied

            //List of sort options.
            //NOTE: make sure this array has matching values to the column headings
            PopulateDropDownLists();
            string[] sortOptions = new[] { "Start Time", "End Time", "Location" };
           

            var games = _context.Games
                .Include(t => t.GameTeams)
                .AsNoTracking();
            //Add as many filters as needed
            if (ID.HasValue)
            {
                games = games.Where(t => t.ID == ID);
                numberFilters++;
            }

            if (!System.String.IsNullOrEmpty(SearchString))
            {
                games = games.Where(p => p.GameLocation.ToUpper().Contains(SearchString.ToUpper())
                                      
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
            if (sortField == "Start Time")
            {

                if (sortDirection == "asc")
                {
                    games = games
                        .OrderBy(p => p.GameStartTime);
                }
                else
                {
                    games = games
                        .OrderByDescending(p => p.GameStartTime);
                }
            }
            else if (sortField == "End Time")
            {
                if (sortDirection == "asc")
                {
                    games = games
                      .OrderBy(p => p.GameEndTime);
                }
                else
                {
                    games = games
                       .OrderByDescending(p => p.GameEndTime);
                }
            }
            else if (sortField == "Location")
            {
                if (sortDirection == "asc")
                {
                    games = games
                       .OrderBy(p => p.GameLocation);

                }
                else
                {
                    games = games
                       .OrderByDescending(p => p.GameLocation);

                }
            }

            //Gets matchups from teams query
            ViewData["Matchups"] = GameMatchup.GetMatchups(_context, _context.Teams.Include(t => t.GameTeams).ThenInclude(gt => gt.Game).ToArray());

            //Set sort for next time
            ViewData["sortField"] = sortField;
            ViewData["sortDirection"] = sortDirection;

            int pageSize = PageSizeHelper.SetPageSize(HttpContext, pageSizeID, ControllerName());
            ViewData["pageSizeID"] = PageSizeHelper.PageSizeList(pageSize);
            var pagedData = await PaginatedList<Game>.CreateAsync(games.AsNoTracking(), page ?? 1, pageSize);

            // Change the return statement to use the pagedData
            return View(pagedData);
        }

        // GET: Games/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Games == null)
            {
                return NotFound();
            }

            var game = await _context.Games
                .FirstOrDefaultAsync(m => m.ID == id);
            if (game == null)
            {
                return NotFound();
            }

            return View(game);
        }

        // GET: Games/Create
        public IActionResult Create()
        {
            PopulateDropDownLists();
            return View();
        }

        // POST: Games/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,GameStartTime,GameEndTime,GameLocation")] Game game, int? HomeTeamID, int? AwayTeamID)
        {
            PopulateDropDownLists();
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(game);
                    CreateGameTeams(HomeTeamID, AwayTeamID);
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
                        if (dex.InnerException.Message.Contains("GameStartTime"))
                            ModelState.AddModelError("GameStartTime", "A game with this start time already exists. Please choose a different time."); //pass a message to the field that triggered the error
                        if (dex.InnerException.Message.Contains("Foreign Key"))
                            ModelState.AddModelError("", "A game cannot have the same team on both sides. Please choose two different teams."); //pass a message to the field that triggered the error


                    }
                    else
                        ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                }
            }
            return View(game);
        }

        // GET: Games/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Games == null)
            {
                return NotFound();
            }

            var game = await _context.Games
                 .Include(t => t.GameTeams)
                 .AsNoTracking()
            .FirstOrDefaultAsync(m => m.ID == id);

            PopulateDropDownLists(game);

            if (game == null)
            {
                return NotFound();
            }
            return View(game);
        }

        // POST: Games/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id)
        {
            var gamesToUpdate = await _context.Games
                .Include(t => t.GameTeams)
                .FirstOrDefaultAsync(m => m.ID == id);
            
            PopulateDropDownLists(gamesToUpdate);

            if (gamesToUpdate == null)
            {
                return NotFound();
            }

            if (await TryUpdateModelAsync<Game>(gamesToUpdate, "", g => g.GameStartTime, g => g.GameEndTime, g => g.GameLocation))
            {
                try
                {
                    _context.Update(gamesToUpdate);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GameExists(gamesToUpdate.ID))
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
                        if (dex.InnerException.Message.Contains("GameStartTime"))
                            ModelState.AddModelError("GameStartTime", "A game with this start time already exists. Please choose a different time."); //pass a message to the field that triggered the error
                        else if (dex.InnerException.Message.Contains("GameEndTime"))
                            ModelState.AddModelError("GameEndTime", "A team with this End Time already exists. Please choose a different time.");
                    }
                    else
                        ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                    return View(gamesToUpdate);
                }
            }


            return RedirectToAction("Details", new { gamesToUpdate.ID});
        }

        // GET: Games/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Games == null)
            {
                return NotFound();
            }

            var game = await _context.Games
                .Include(g => g.GameTeams)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID == id);
            if (game == null)
            {
                return NotFound();
            }

            return View(game);
        }

        // POST: Games/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Games == null)
            {
                return Problem("The Game you are trying to delete is not available.");
            }
            var game = await _context.Games.FindAsync(id);
            try
            {
                if (game != null)
                {
                    _context.Games.Remove(game);
                }

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
                return View("Delete", game);
            }
        }

        private void CreateGameTeams( int? HomeTeamID, int? AwayTeamID)
        {
            int gameID = _context.Games.Count() + 1;
            _context.GameTeams.AddRange(
                new GameTeam
                {
                    GameID = gameID,
                    TeamID = HomeTeamID.Value,
                    GmtmLineup = "TBA"
                },
                new GameTeam
                {
                    GameID = gameID,
                    TeamID = AwayTeamID.Value,
                    GmtmLineup = "TBA"
                });
            _context.SaveChanges();
        }

        private SelectList GameSelectList(int? selectedId)
        {
            return new SelectList(_context.Games, "ID", "GameLocation", selectedId);
        }
        private SelectList TeamSelectList(int? selectedId)
            => new(_context.Teams, "ID", "TmName", selectedId);
        private void PopulateDropDownLists(Game game = null)
        {
            ViewData["HomeTeamID"] = TeamSelectList(null);
            ViewData["AwayTeamID"] = TeamSelectList(null);
            ViewData["ID"] = GameSelectList(game?.ID);
        }

        private bool GameExists(int id)
        {
          return _context.Games.Any(e => e.ID == id);
        }
    }
}
