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
using wmbaApp.ViewModels;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace wmbaApp.Controllers
{
    public class PlayersController : ElephantController
    {
        private readonly WmbaContext _context;

        public PlayersController(WmbaContext context)
        {
            _context = context;
        }

        // GET: Players
        public async Task<IActionResult> Index(string SearchString, int? TeamID,
             int? page, int? pageSizeID, string actionButton, string sortDirection = "asc", string sortField = "Player Full Name")
        {
            //Count the number of filters applied - start by assuming no filters
            ViewData["Filtering"] = "btn-outline-secondary";
            int numberFilters = 0;
            //Then in each "test" for filtering, add to the count of Filters applied

            //List of sort options.
            //NOTE: make sure this array has matching values to the column headings
            string[] sortOptions = new[] { "Player Full Name", "Team" };

            PopulateDropDownLists();

            var players = _context.Players
                .Include(t => t.Team).ThenInclude(t => t.Division)
                .Include(t => t.Statistics)
                .AsNoTracking();

            //Add as many filters as needed
            if (TeamID.HasValue)
            {
                players = players.Where(p => p.TeamID == TeamID);
                numberFilters++;
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

            return View(pagedData);

        }


        // GET: Players/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Players == null)
            {
                return NotFound();
            }

            var player = await _context.Players
                .Include(p => p.Team)
                .Include(p => p.Statistics)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID == id);
            if (player == null)
            {
                return NotFound();
            }

            return View(player);
        }

        // GET: Players/Create
        public IActionResult Create()
        {
            Player player = new Player();
            PopulateDropDownLists(player);
            return View(player);
        }

        // POST: Players/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,PlyrFirstName,PlyrLastName,PlyrJerseyNumber," +
            "PlyrDOB,TeamID,StatisticID")] Player player, IFormFile thePicture, string[] selectedOptions)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(player);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Details", new { player.ID });
                }

            }
            catch (RetryLimitExceededException /* dex */)
            {
                ModelState.AddModelError("", "Unable to save changes after multiple attempts. Try again, and if the problem persists, see your system administrator.");
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }

            PopulateDropDownLists(player);
            return View(player);
        }

        // GET: Players/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Players == null)
            {
                return NotFound();
            }

            var player = await _context.Players
                .FirstOrDefaultAsync(f => f.ID == id);

            if (player == null)
            {
                return NotFound();
            }

            PopulateDropDownLists(player);
            return View(player);
        }

        // POST: Players/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string[] selectedOptions)
        {
            var playerToUpdate = await _context.Players
            .FirstOrDefaultAsync(m => m.ID == id);

            if (playerToUpdate == null)
            {
                return NotFound();
            }


            if (await TryUpdateModelAsync<Player>(playerToUpdate, "",
                t => t.PlyrFirstName, t => t.PlyrLastName, t => t.TeamID, t => t.PlyrJerseyNumber,
                t => t.StatisticID))
            {
                try
                {
                    _context.Update(playerToUpdate);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Details", new { playerToUpdate.ID });
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
                catch (DbUpdateException)
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                }
            }
            PopulateDropDownLists(playerToUpdate);
            return View(playerToUpdate);
        }

        // GET: Players/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Players == null)
            {
                return NotFound();
            }

            var player = await _context.Players
                .FirstOrDefaultAsync(m => m.ID == id);
            if (player == null)
            {
                return NotFound();
            }

            return View(player);
        }

        // POST: Players/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Players == null)
            {
                return Problem("Entity set 'WmbaContext.Players' is null.");
            }
            var player = await _context.Players.FindAsync(id);
            if (player != null)
            {
                _context.Players.Remove(player);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        #region SelectLists
        private SelectList TeamSelectList(int? selectedId)
        {
            return new SelectList(_context.Teams, "ID", "TmName", selectedId);
        }
        private SelectList StatisticSelectList(int? selectedId)
        {
            return new SelectList(_context.Statistics, "ID", "ID", selectedId);
        }
        #endregion
        private void PopulateDropDownLists(Player player = null)
        {
            ViewData["TeamID"] = TeamSelectList(player?.TeamID);
            ViewData["StatisticID"] = StatisticSelectList(player?.StatisticID);
        }

        //#region PositionCheckboxes
        //private void PopulateAssignedPositionCheckboxes(Player player)
        //{
        //    //For this to work, you must have Included the FunctionRooms 
        //    //in the Function
        //    var allOptions = _context.Positions;
        //    var currentOptionIDs = new HashSet<int>(player.PlayerPositions.Select(b => b.PositionID));
        //    var checkBoxes = new List<CheckOptionVM>();
        //    foreach (var option in allOptions)
        //    {
        //        checkBoxes.Add(new CheckOptionVM
        //        {
        //            ID = option.ID,
        //            DisplayText = option.PosName,
        //            Assigned = currentOptionIDs.Contains(option.ID)
        //        });
        //    }
        //    ViewData["PositionOptions"] = checkBoxes;
        //}

        ////For the posiionList
        //private void PopulateAssignedPositionLists(Player player)
        //{
        //    //For this to work, you must have Included the child collection in the parent object
        //    var allOptions = _context.Positions;
        //    var currentOptionsHS = new HashSet<int>(player.PlayerPositions.Select(b => b.PositionID));
        //    //Instead of one list with a boolean, we will make two lists
        //    var selected = new List<ListOptionVM>();
        //    var available = new List<ListOptionVM>();
        //    foreach (var r in allOptions)
        //    {
        //        if (currentOptionsHS.Contains(r.ID))
        //        {
        //            selected.Add(new ListOptionVM
        //            {
        //                ID = r.ID,
        //                DisplayText = r.PosName
        //            });
        //        }
        //        else
        //        {
        //            available.Add(new ListOptionVM
        //            {
        //                ID = r.ID,
        //                DisplayText = r.PosName
        //            });
        //        }
        //    }

        //    ViewData["selOpts"] = new MultiSelectList(selected.OrderBy(s => s.DisplayText), "ID", "DisplayText");
        //    ViewData["availOpts"] = new MultiSelectList(available.OrderBy(s => s.DisplayText), "ID", "DisplayText");
        //}

        //private void UpdatePlayerPositionsListboxes(string[] selectedOptions, Player playerToUpdate)
        //{
        //    if (selectedOptions == null)
        //    {
        //        playerToUpdate.PlayerPositions = new List<PlayerPosition>();
        //        return;
        //    }

        //    var selectedOptionsHS = new HashSet<string>(selectedOptions);
        //    var currentOptionsHS = new HashSet<int>(playerToUpdate.PlayerPositions.Select(b => b.PositionID));
        //    foreach (var r in _context.Positions)
        //    {
        //        if (selectedOptionsHS.Contains(r.ID.ToString()))//it is selected
        //        {
        //            if (!currentOptionsHS.Contains(r.ID))
        //            {
        //                playerToUpdate.PlayerPositions.Add(new PlayerPosition
        //                {
        //                    PositionID = r.ID,
        //                    PlayerID = playerToUpdate.ID
        //                });
        //            }
        //        }
        //        else //not selected
        //        {
        //            if (currentOptionsHS.Contains(r.ID))
        //            {
        //                PlayerPosition positionToRemove = playerToUpdate.PlayerPositions.FirstOrDefault(d => d.PositionID == r.ID);
        //                _context.Remove(positionToRemove);
        //            }
        //        }
        //    }
        //}
        //#endregion
        private bool PlayerExists(int id)
        {
            return _context.Players.Any(e => e.ID == id);
        }
    }
}
