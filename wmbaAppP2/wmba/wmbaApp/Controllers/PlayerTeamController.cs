﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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
    public class PlayerTeamController : ElephantController
    {
        private readonly WmbaContext _context;

        public PlayerTeamController(WmbaContext context)
        {
            _context = context;
        }

        // GET: Players
        public async Task<IActionResult> Index(string SearchString, int? TeamID,
             int? page, int? pageSizeID, string actionButton, string sortDirection = "asc", string sortField = "")
        {
            //Count the number of filters applied - start by assuming no filters
            ViewData["Filtering"] = "btn-outline-secondary";
            int numberFilters = 0;
            //Then in each "test" for filtering, add to the count of Filters applied

            //List of sort options.
            //NOTE: make sure this array has matching values to the column headings
            string[] sortOptions = new[] { "Player Full Name", "Team" };

            var players = _context.Players
            .Include(t => t.Team)
            .Include(t => t.Statistics)
            .Where(t => t.IsActive)
            .AsNoTracking();

            //Add as many filters as needed
            if (TeamID.HasValue)
            {
                players = players.Where(p => p.TeamID == TeamID);
            }

            if (!System.String.IsNullOrEmpty(SearchString))
            {
                players = players.Where(p => p.PlyrFirstName.ToUpper().Contains(SearchString.ToUpper())
                                       || p.PlyrLastName.ToUpper().Contains(SearchString.ToUpper())
                                       || p.Team.TmName.ToUpper().Contains(SearchString.ToUpper())
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
            //Now we know which field and direction to sort by
            if (sortField == "Player Full Name")
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

            return View();
        }

        // GET: Players
        public async Task<IActionResult> InactiveIndex(string SearchString, int? TeamID,
             int? page, int? pageSizeID, string actionButton, string sortDirection = "asc", string sortField = "")
        {
            //Count the number of filters applied - start by assuming no filters
            ViewData["Filtering"] = "btn-outline-secondary";
            int numberFilters = 0;
            //Then in each "test" for filtering, add to the count of Filters applied

            //List of sort options.
            //NOTE: make sure this array has matching values to the column headings
            string[] sortOptions = new[] { "Player Full Name", "Team" };

            var players = _context.Players
            .Include(t => t.Team)
            .Include(t => t.Statistics)
            .Where(t => !t.IsActive)
            .AsNoTracking();

            //Add as many filters as needed
            if (TeamID.HasValue)
            {
                players = players.Where(p => p.TeamID == TeamID);
            }

            if (!System.String.IsNullOrEmpty(SearchString))
            {
                players = players.Where(p => p.PlyrFirstName.ToUpper().Contains(SearchString.ToUpper())
                                       || p.PlyrLastName.ToUpper().Contains(SearchString.ToUpper())
                                       || p.Team.TmName.ToUpper().Contains(SearchString.ToUpper())
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
            //Now we know which field and direction to sort by
            if (sortField == "Player Full Name")
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
        [HttpPost]
        [ValidateAntiForgeryToken]
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
                    if (!String.IsNullOrEmpty(submitButton))
                    {
                        if (submitButton == "Add player")
                            return RedirectToAction("Create", new { TeamID = team.ID });
                        if (submitButton == "Add player and finish")
                            return RedirectToAction("Index", new { TeamID = team.ID });
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
                   ModelState.AddModelError("", "Unable to save changes after multiple attempts. Try again, and if the problem persists, see your system administrator.");
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }



            return View(player);
        }

        // GET: PlayerTeam/Edit/5
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
                return RedirectToAction("Index", new { TeamID = team.ID });
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }

            return View(player);
        }

        // GET: Players/Active/5
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

        // POST: Players/Active/5
        [HttpPost, ActionName("MakeActive")]
        [ValidateAntiForgeryToken]
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

            await _context.SaveChangesAsync();
            return RedirectToAction("InactiveIndex", new { TeamID = team.ID });
        }

        private bool PlayerExists(int id)
        {
            return _context.Players.Any(e => e.ID == id);
        }
    }
}
