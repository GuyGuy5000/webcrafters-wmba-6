using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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
    [Authorize]
    public class PlayerTeamController : ElephantController
    {
        private readonly WmbaContext _context;
        private readonly ApplicationDbContext _AppContext;

        public PlayerTeamController(WmbaContext context, ApplicationDbContext appContext)
        {
            _context = context;
            _AppContext = appContext;

        }

        // GET: Players
        public async Task<IActionResult> Index(string SearchString, int? TeamID,
             int? page, int? pageSizeID, string actionButton, string sortDirection = "asc", string sortField = "")
        {
            //Count the number of filters applied - start by assuming no filters
            ViewData["Filtering"] = "btn-outline-dark";
            int numberFilters = 0;
            //Then in each "test" for filtering, add to the count of Filters applied
            if (TempData["SuccessMessage"] != null)
            {
                ViewBag.SuccessMessage = TempData["SuccessMessage"].ToString();
            }
            //List of sort options.
            //NOTE: make sure this array has matching values to the column headings
            string[] sortOptions = new[] { "Name", "Team" };

            PopulateDropDownLists();

            IQueryable<Player> players;

            if (User.IsInRole("Admin"))
            {
                players = _context.Players
                .Include(t => t.Team).ThenInclude(t => t.Division)
                .Include(t => t.Statistics)
                .Where(t => t.IsActive)
                .AsNoTracking();
            }
            else
            {
                var rolesTeamIDs = await UserRolesHelper.GetUserTeamIDs(_AppContext, User);
                var rolesDivisionIDs = await UserRolesHelper.GetUserDivisionIDs(_AppContext, User);

                players = _context.Players
                .Include(t => t.Team).ThenInclude(t => t.Division)
                .Include(t => t.Statistics)
                .Where(t => t.IsActive)
                .AsNoTracking();
            }

            if (TeamID.HasValue)
            {
                players = players.Where(p => p.TeamID == TeamID);
            }

            if (!System.String.IsNullOrEmpty(SearchString))
            {
                players = players.Where(p => p.PlyrFirstName.ToUpper().Contains(SearchString.ToUpper())
                                       || p.PlyrLastName.ToUpper().Contains(SearchString.ToUpper())
                                       || p.Team.TmName.ToUpper().Contains(SearchString.ToUpper()));

                numberFilters++;
            }
            //Give feedback about the state of the filters
            if (numberFilters != 0)
            {
                //Toggle the Open/Closed state of the collapse depending on if we are filtering
                ViewData["Filtering"] = "btn-outline-dark";
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
            //Now we know which field and direction to sort by
            if (sortField == "Name")
            {

                if (sortDirection == "asc")
                {
                    players = players
                        .OrderBy(p => p.PlyrFirstName);
                }
                else
                {
                    players = players
                        .OrderByDescending(p => p.PlyrFirstName);
                }
            }
            else
            {
                if (sortDirection == "asc")
                {
                    players = players
                        .OrderBy(p => p.Team);
                }
                else
                {
                    players = players
                        .OrderByDescending(p => p.Team);
                }
            }
            ViewData["sortField"] = sortField;
            ViewData["sortDirection"] = sortDirection;

            int pageSize = PageSizeHelper.SetPageSize(HttpContext, pageSizeID, ControllerName());
            ViewData["pageSizeID"] = PageSizeHelper.PageSizeList(pageSize);
            var pagedData = await PaginatedList<Player>.CreateAsync(players.AsNoTracking(), page ?? 1, pageSize);

            var team = await _context.Teams
                .Include(t => t.Division)
                .Include(t => t.DivisionCoaches).ThenInclude(t => t.Coach)
                .Include(t => t.Players)
                .Include(t => t.GameTeams).ThenInclude(t => t.Game)
                .Where(t => t.ID == TeamID)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            ViewBag.Team = team;

            return View(pagedData);
        }

        // GET: PlayerTeam/Details/5
        public async Task<IActionResult> Details(int? id, int? TeamID)
        {
            if (id == null || _context.Players == null)
            {
                return NotFound();
            }
            if (TempData["SuccessMessage"] != null)
            {
                ViewBag.SuccessMessage = TempData["SuccessMessage"].ToString();
            }

            var player = await _context.Players
                .Include(p => p.Team)
                .Include(p => p.Statistics)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (player == null)
            {
                return NotFound();
            }

            var team = await _context.Teams
                .Include(t => t.Division)
                .Include(t => t.DivisionCoaches).ThenInclude(t => t.Coach)
                .Include(t => t.Players)
                .Include(t => t.GameTeams).ThenInclude(t => t.Game)
                .Where(t => t.ID == TeamID)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            ViewBag.Team = team;

            return View(player);
        }

        // GET: PlayerTeam/Create
        [Authorize(Roles = "Admin,Convenor,Coach")]
        public async Task<IActionResult> Create(int? TeamID)
        {
            var team = await _context.Teams
                .Include(t => t.Division)
                .Include(t => t.DivisionCoaches).ThenInclude(t => t.Coach)
                .Include(t => t.Players)
                .Include(t => t.GameTeams).ThenInclude(t => t.Game)
                .Where(t => t.ID == TeamID)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            ViewBag.Team = team;

            if (TempData["SuccessMessage"] != null)
            {
                ViewBag.SuccessMessage = TempData["SuccessMessage"].ToString();
                TempData.Remove("SuccessMessage"); // Remove the success message from TempData after displaying it
            }

            return View();
        }

        // GET: Players
        [Authorize(Roles = "Admin,Convenor,Coach")]
        public async Task<IActionResult> InactiveIndex(string SearchString, int? TeamID,
             int? page, int? pageSizeID, string actionButton, string sortDirection = "asc", string sortField = "")
        {
            //Count the number of filters applied - start by assuming no filters
            ViewData["Filtering"] = "btn-outline-dark";
            int numberFilters = 0;
            //Then in each "test" for filtering, add to the count of Filters applied

            if (TempData["SuccessMessage"] != null)
            {
                ViewBag.SuccessMessage = TempData["SuccessMessage"].ToString();
            }

            //List of sort options.
            //NOTE: make sure this array has matching values to the column headings
            string[] sortOptions = new[] { "Player Full Name", "Team" };

            IQueryable<Player> players;

            if (User.IsInRole("Admin"))
            {
                players = _context.Players
                .Include(t => t.Team).ThenInclude(t => t.Division)
                .Include(t => t.Statistics)
                .Where(t => !t.IsActive)
                .AsNoTracking();
            }
            else
            {
                var rolesTeamIDs = await UserRolesHelper.GetUserTeamIDs(_AppContext, User);
                var rolesDivisionIDs = await UserRolesHelper.GetUserDivisionIDs(_AppContext, User);

                players = _context.Players
                .Include(t => t.Team).ThenInclude(t => t.Division)
                .Include(t => t.Statistics)
                .Where(t => !t.IsActive && (rolesTeamIDs.Contains(t.TeamID) || rolesDivisionIDs.Contains(t.Team.DivisionID)))
                .AsNoTracking();
            }

            if (TeamID.HasValue)
            {
                players = players.Where(p => p.TeamID == TeamID);
            }

            if (!System.String.IsNullOrEmpty(SearchString))
            {
                players = players.Where(p => p.PlyrFirstName.ToUpper().Contains(SearchString.ToUpper())
                                       || p.PlyrLastName.ToUpper().Contains(SearchString.ToUpper())
                                       || p.Team.TmName.ToUpper().Contains(SearchString.ToUpper()));

                numberFilters++;
            }
            //Give feedback about the state of the filters
            if (numberFilters != 0)
            {
                //Toggle the Open/Closed state of the collapse depending on if we are filtering
                ViewData["Filtering"] = "btn-outline-dark";
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
            //Now we know which field and direction to sort by
            if (sortField == "Name")
            {

                if (sortDirection == "asc")
                {
                    players = players
                        .OrderBy(p => p.PlyrFirstName);
                }
                else
                {
                    players = players
                        .OrderByDescending(p => p.PlyrFirstName);
                }
            }
            else
            {
                if (sortDirection == "asc")
                {
                    players = players
                        .OrderBy(p => p.Team);
                }
                else
                {
                    players = players
                        .OrderByDescending(p => p.Team);
                }
            }
            ViewData["sortField"] = sortField;
            ViewData["sortDirection"] = sortDirection;

            int pageSize = PageSizeHelper.SetPageSize(HttpContext, pageSizeID, ControllerName());
            ViewData["pageSizeID"] = PageSizeHelper.PageSizeList(pageSize);
            var pagedData = await PaginatedList<Player>.CreateAsync(players.AsNoTracking(), page ?? 1, pageSize);

            var team = await _context.Teams
                .Include(t => t.Division)
                .Include(t => t.DivisionCoaches).ThenInclude(t => t.Coach)
                .Include(t => t.Players)
                .Include(t => t.GameTeams).ThenInclude(t => t.Game)
                .Where(t => t.ID == TeamID)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            ViewBag.Team = team;

            return View(pagedData);
        }

        // POST: PlayerTeam/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        // POST: PlayerTeam/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Convenor,Coach")]
        public async Task<IActionResult> Create([Bind("ID,PlyrFirstName,PlyrLastName,PlyrJerseyNumber,PlyrMemberID,TeamID,StatsID")] Player player, int? TeamID, string submitButton = "")
        {
            var team = await _context.Teams
                .Include(t => t.Division)
                .Include(t => t.DivisionCoaches).ThenInclude(t => t.Coach)
                .Include(t => t.Players)
                .Include(t => t.GameTeams).ThenInclude(t => t.Game)
                .Where(t => t.ID == TeamID)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            ViewBag.Team = team;

            player.Statistics = new Statistic();
            player.TeamID = (int)TeamID;

            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(player);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Player created successfully."; // Set the success message in TempData

                    if (!String.IsNullOrEmpty(submitButton))
                    {
                        if (submitButton == "Add player")
                        {
                            return RedirectToAction("Create", new { TeamID = team.ID }); // Redirect to the Create action to display the success message
                        }

                        return RedirectToAction("Index"); // Redirect to index or any other action as needed
                    }
                }
            }
            catch (RetryLimitExceededException dex)
            {
                if (dex.InnerException.Message.Contains("UNIQUE")) //if a UNIQUE constraint caused the exception
                {
                    if (dex.InnerException.Message.Contains("PlyrJerseyNumber"))
                        ModelState.AddModelError("PlyrJerseyNumber", "A player with this member ID already exists.");
                }
                else
                {
                    ModelState.AddModelError("", "Unable to save changes after multiple attempts. Try again, and if the problem persists, see your system administrator.");
                }
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error creating player: {ex.Message}";
                // Handle the exception as needed
            }

            return View(player);
        }


        // GET: PlayerTeam/Edit/5
        [Authorize(Roles = "Admin,Convenor,Coach")]
        public async Task<IActionResult> Edit(int? id, int? TeamID)
        {
            if (id == null || _context.Players == null)
            {
                return NotFound();
            }

            var player = await _context.Players.FindAsync(id);
            if (player == null)
            {
                return NotFound();
            }

            var team = await _context.Teams
                .Include(t => t.Division)
                .Include(t => t.DivisionCoaches).ThenInclude(t => t.Coach)
                .Include(t => t.Players)
                .Include(t => t.GameTeams).ThenInclude(t => t.Game)
                .Where(t => t.ID == TeamID)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            ViewBag.Team = team;

            return View(player);
        }

        // POST: PlayerTeam/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Convenor,Coach")]
        public async Task<IActionResult> Edit(int id, int? TeamID)
        {

            var team = await _context.Teams
                .Include(t => t.Division)
                .Include(t => t.DivisionCoaches).ThenInclude(t => t.Coach)
                .Include(t => t.Players)
                .Include(t => t.GameTeams).ThenInclude(t => t.Game)
                .Where(t => t.ID == TeamID)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            ViewBag.Team = team;

            var playerToUpdate = await _context.Players
                        .FirstOrDefaultAsync(m => m.ID == id);

            if (playerToUpdate == null)
            {
                return NotFound();
            }


            if (await TryUpdateModelAsync<Player>(playerToUpdate, "",
                t => t.PlyrFirstName, t => t.PlyrLastName, t => t.TeamID, t => t.PlyrJerseyNumber, t => t.PlyrMemberID,
                t => t.StatisticID))
            {
                try
                {
                    _context.Update(playerToUpdate);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Player updated successfully.";
                    return RedirectToAction("Details", new { playerToUpdate.ID, TeamID = team.ID });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PlayerExists(playerToUpdate.ID))
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
                        if (dex.InnerException.Message.Contains("PlyrJerseyNumber"))
                            ModelState.AddModelError("PlyrJerseyNumber", "A player with this jersey number already exists.");
                    }
                    else
                        ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Error creating player: {ex.Message}";
                    // Handle the exception as needed
                }
            }
            return View(playerToUpdate);
        }

        //// GET: PlayerTeam/Delete/5
        //public async Task<IActionResult> Delete(int? id, int? TeamID)
        //{
        //    var team = await _context.Teams
        //        .Include(t => t.Division)
        //        .Include(t => t.DivisionCoaches).ThenInclude(t => t.Coach)
        //        .Include(t => t.Players)
        //        .Include(t => t.GameTeams).ThenInclude(t => t.Game)
        //        .Where(t => t.ID == TeamID)
        //        .AsNoTracking()
        //        .FirstOrDefaultAsync();

        //    ViewBag.Team = team;
        //    if (id == null || _context.Players == null)
        //    {
        //        return NotFound();
        //    }

        //    var player = await _context.Players
        //        .Include(p => p.Team)
        //        .FirstOrDefaultAsync(m => m.ID == id);
        //    if (player == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(player);
        //}

        //// POST: PlayerTeam/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id, int? TeamID)
        //{
        //    var team = await _context.Teams
        //        .Include(t => t.Division)
        //        .Include(t => t.DivisionCoaches).ThenInclude(t => t.Coach)
        //        .Include(t => t.Players)
        //        .Include(t => t.GameTeams).ThenInclude(t => t.Game)
        //        .Where(t => t.ID == TeamID)
        //        .AsNoTracking()
        //        .FirstOrDefaultAsync();

        //    ViewBag.Team = team;

        //    if (_context.Players == null)
        //    {
        //        return Problem("Entity set 'WmbaContext.Players'  is null.");
        //    }
        //    var player = await _context.Players
        //        .Include(t => t.Team)
        //        .Include(t => t.Statistics)
        //        .FirstOrDefaultAsync(m => m.ID == id);

        //    try
        //    {
        //        if (player != null)
        //        {
        //            _context.Players.Remove(player);
        //        }

        //        await _context.SaveChangesAsync();
        //        return Redirect(ViewData["returnURL"].ToString());
        //    }
        //    catch (DbUpdateException)
        //    {
        //        ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
        //    }

        //    return View(player);
        //}

        // GET: PlayerTeam/Inactive/5
        [Authorize(Roles = "Admin,Convenor,Coach")]
        public async Task<IActionResult> MakeInactive(int? id, int? TeamID)
        {
            var team = await _context.Teams
                .Include(t => t.Division)
                .Include(t => t.DivisionCoaches).ThenInclude(t => t.Coach)
                .Include(t => t.Players)
                .Include(t => t.GameTeams).ThenInclude(t => t.Game)
                .Where(t => t.ID == TeamID)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            ViewBag.Team = team;

            if (id == null || _context.Players == null)
                return NotFound();

            var player = await _context.Players
                .Include(p => p.Team)
                .Include(p => p.Statistics)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (player == null)
                return NotFound();

            return View(player);
        }

        // POST: PlayerTeam/Inactive/5
        [HttpPost, ActionName("MakeInactive")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Convenor,Coach")]
        public async Task<IActionResult> MakeInactiveConfirmed(int id, int? TeamID)
        {
            var team = await _context.Teams
                .Include(t => t.Division)
                .Include(t => t.DivisionCoaches).ThenInclude(t => t.Coach)
                .Include(t => t.Players)
                .Include(t => t.GameTeams).ThenInclude(t => t.Game)
                .Where(t => t.ID == TeamID)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            ViewBag.Team = team;

            if (_context.Players == null)
            {
                return Problem("Entity set 'WmbaContext.Players' is null.");
            }
            var player = await _context.Players
                .Include(t => t.Team)
                .Include(t => t.Statistics)
                .FirstOrDefaultAsync(m => m.ID == id);

            try
            {

                if (player != null)
                {
                    player.IsActive = false;
                    _context.Players.Update(player);
                }

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Player deactivated successfully.";
                return RedirectToAction("Index", new { TeamID = team.ID });
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error creating player: {ex.Message}";
                // Handle the exception as needed
            }

            return View(player);
        }

        // GET: Players/Active/5
        [Authorize(Roles = "Admin,Convenor,Coach")]
        public async Task<IActionResult> MakeActive(int? id, int? TeamID)
        {
            var team = await _context.Teams
                .Include(t => t.Division)
                .Include(t => t.DivisionCoaches).ThenInclude(t => t.Coach)
                .Include(t => t.Players)
                .Include(t => t.GameTeams).ThenInclude(t => t.Game)
                .Where(t => t.ID == TeamID)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            ViewBag.Team = team;
            try
            {
                if (id == null || _context.Players == null)
                    return NotFound();

                //TempData["SuccessMessage"] = "Player activated successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error creating player: {ex.Message}";
                // Handle the exception as needed
            }
            var player = await _context.Players
                .Include(p => p.Team)
                .Include(p => p.Statistics)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (player == null)
                return NotFound();
            //TempData["SuccessMessage"] = "Player activated successfully.";
            return View(player);
        }


        // POST: Players/Active/5
        [HttpPost, ActionName("MakeActive")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Convenor,Coach")]
        public async Task<IActionResult> MakeActiveConfirmed(int id, int? TeamID)
        {
            var team = await _context.Teams
                .Include(t => t.Division)
                .Include(t => t.DivisionCoaches).ThenInclude(t => t.Coach)
                .Include(t => t.Players)
                .Include(t => t.GameTeams).ThenInclude(t => t.Game)
                .Where(t => t.ID == TeamID)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            ViewBag.Team = team;

            if (_context.Players == null)
            {
                return Problem("Entity set 'WmbaContext.Players' is null.");
            }

            var player = await _context.Players
                .Include(p => p.Team)
                .Include(p => p.Statistics)
                .FirstOrDefaultAsync(p => p.ID == id);

            try
            {
                if (player != null)
                {
                    if (player.Team.IsActive)
                    {
                        player.IsActive = true;
                        _context.Players.Update(player);
                    }
                    else
                    {
                        ModelState.AddModelError("", "This player's team is inactive. Reactivate the team or reassign this player to a different team to reactivate");
                        return View(player);
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error creating player: {ex.Message}";
                // Handle the exception as needed
            }

            await _context.SaveChangesAsync();

            // Set the success message in TempData
            TempData["SuccessMessage"] = "Player activated successfully.";

            // Redirect to the InactiveIndex action with the TeamID and SuccessMessage as query parameters
            return RedirectToAction("InactiveIndex", new { TeamID = team.ID, SuccessMessage = TempData["SuccessMessage"] });
        }


        #region SelectLists
        private SelectList TeamSelectList(int? selectedId)
        {
            return new SelectList(_context.Teams.Where(t => t.IsActive), "ID", "TmName", selectedId);
        }

        private SelectList DivisionSelectList(int? selectedId)
        {
            return new SelectList(_context.Divisions, "ID", "DivName", selectedId);
        }


        private SelectList StatisticSelectList(int? selectedId)
        {
            return new SelectList(_context.Statistics, "ID", "ID", selectedId);
        }
        #endregion
        private void PopulateDropDownLists(Player player = null)
        {
            ViewData["TeamID"] = TeamSelectList(player?.TeamID);
            ViewData["DivisionID"] = DivisionSelectList(player?.Team?.DivisionID);
            ViewData["StatisticID"] = StatisticSelectList(player?.StatisticID);
        }

        private bool PlayerExists(int id)
        {
            return _context.Players.Any(e => e.ID == id);
        }
    }
}
