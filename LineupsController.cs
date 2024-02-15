/// <summary>
/// Lineup
/// Farooq Jidelola
/// </summary>
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
using static System.Runtime.InteropServices.JavaScript.JSType;
using wmbaApp.ViewModels;
using Microsoft.EntityFrameworkCore.Storage;

namespace wmbaApp.Controllers
{
    public class LineupsController : ElephantController
    {
        private readonly WmbaContext _context;

        public LineupsController(WmbaContext context)
        {
            _context = context;
        }

        // GET: Lineups
        //public async Task<IActionResult> Index()
        //{
        //    var wmbaContext = _context.Lineups.Include(l => l.Game);
        //    return View(await wmbaContext.ToListAsync());
        //}

        // GET: Lineups
        public IActionResult Index()
        {

            return View();
        }

        // GET: Lineups/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Lineups == null)
            {
                return NotFound();
            }

            var lineup = await _context.Lineups
                .Include(l => l.LineupPlayers).ThenInclude(lp => lp.HomePlayer)
                .Include(l => l.LineupPlayers).ThenInclude(lp => lp.AwayPlayer)
                .FirstOrDefaultAsync(m => m.ID == id);

            if (lineup == null)
            {
                return NotFound();
            }

            return View(lineup);
        }


        // GET: Lineups/Create
        public IActionResult Create(int gameId)
        {
            Lineup lineup = new Lineup();
            TeamPlayersCheckboxes(gameId);
            PopulateDropDownLists(lineup);
            return View(lineup);
        }

        // POST: Lineups/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("ID,HomeTeamID,AwayTeamID")] Lineup lineup, string[] selectedOptions)
        //{
        //    try
        //    {
        //        //Add the selected conditions
        //        if (selectedOptions != null)
        //        {
        //            foreach (var condition in selectedOptions)
        //            {
        //                var playerToAdd = new Game { LineupID = lineup.ID, PlayerID = int.Parse(condition) };
        //                lineup.Games.Add(playerToAdd);
        //            }
        //        }
        //        if (ModelState.IsValid)
        //        {
        //            _context.Add(lineup);
        //            await _context.SaveChangesAsync();
        //            return RedirectToAction("Details", new { lineup.ID });
        //        }
        //    }
        //    catch (RetryLimitExceededException /* dex */)
        //    {
        //        ModelState.AddModelError("", "Unable to save changes after multiple attempts. Try again, and if the problem persists, see your system administrator.");
        //    }
        //    catch (DbUpdateException)
        //    {
        //        ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
        //    }

        //   // TeamPlayersCheckboxes(lineup);
        //    PopulateDropDownLists(lineup);
        //    return View(lineup);
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,HomeTeamID,AwayTeamID")] Lineup lineup, int gameId, string[] homeSelectedOptions, string[] awaySelectedOptions)
        {
            try
            {
                // Add the selected players to the lineup for HomeTeam
                if (homeSelectedOptions == null || !homeSelectedOptions.Any())
                {
                    ModelState.AddModelError("", "Select players in the Home Team for the game to start.");
                    TeamPlayersCheckboxes(gameId);
                    PopulateDropDownLists(lineup);
                    return View(lineup);
                }
                else
                {
                    foreach (var hpcondition in homeSelectedOptions)
                    {
                        var homePlayerId = int.Parse(hpcondition);
                        var homePlayer = await _context.Players.FindAsync(homePlayerId);

                        if (homePlayer != null)
                        {
                            var homeplayerToAdd = new LineupPlayer { LineupID = lineup.ID, HomePlayerID = homePlayerId, HomePlayer = homePlayer };

                            // Set the Lineup and HomePlayer properties
                            homeplayerToAdd.Lineup = lineup;
                            homeplayerToAdd.HomePlayer = homePlayer;

                            lineup.LineupPlayers.Add(homeplayerToAdd);
                        }
                    }
                }

                // Add the selected players to the lineup for AwayTeam
                if (awaySelectedOptions == null || !awaySelectedOptions.Any())
                {
                    ModelState.AddModelError("", "Select players in the Away Team for the game to start.");
                    TeamPlayersCheckboxes(gameId);
                    PopulateDropDownLists(lineup);
                    return View(lineup);
                }
                else
                {
                    foreach (var apcondition in awaySelectedOptions)
                    {
                        var awayPlayerId = int.Parse(apcondition);
                        var awayPlayer = await _context.Players.FindAsync(awayPlayerId);

                        if (awayPlayer != null)
                        {
                            var awayplayerToAdd = new LineupPlayer { LineupID = lineup.ID, AwayPlayerID = awayPlayerId, AwayPlayer = awayPlayer };

                            // Set the Lineup and AwayPlayer properties
                            awayplayerToAdd.Lineup = lineup;
                            awayplayerToAdd.AwayPlayer = awayPlayer;

                            lineup.LineupPlayers.Add(awayplayerToAdd);
                        }
                    }
                }

                if (ModelState.IsValid)
                {
                    _context.Add(lineup);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Details", new { id = lineup.ID });
                }
                else
                {
                    // Handle model state errors, re-render the view
                    TeamPlayersCheckboxes(gameId);
                    PopulateDropDownLists(lineup);
                    return View(lineup);
                }
            }
            catch (RetryLimitExceededException)
            {
                ModelState.AddModelError("", "Unable to save changes after multiple attempts. Try again, and if the problem persists, see your system administrator.");
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }

            TeamPlayersCheckboxes(gameId);
            PopulateDropDownLists(lineup);
            return View(lineup);
        }



        private void TeamPlayersCheckboxes(int gameId)
        {
            var game = _context.Games
                .Include(g => g.HomeTeam.Players)
                .Include(g => g.AwayTeam.Players)
                .FirstOrDefault(g => g.ID == gameId);

            if (game != null)
            {
                var homeTeamPlayers = game.HomeTeam?.Players;
                var awayTeamPlayers = game.AwayTeam?.Players;

                if (homeTeamPlayers != null && awayTeamPlayers != null)
                {
                    var currentHomePlayersIDs = new HashSet<int>(homeTeamPlayers.Select(p => p.ID));
                    var currentAwayPlayersIDs = new HashSet<int>(awayTeamPlayers.Select(p => p.ID));

                    ViewData["HomeTeamName"] = game.HomeTeam.TmName;
                    ViewData["AwayTeamName"] = game.AwayTeam.TmName;

                    var homeCheckBoxes = homeTeamPlayers.Select(player => new CheckOptionVM //throw it into the Viewmodel
                    {
                        ID = player.ID,
                        DisplayText = player.AssignPlayer,
                        Assigned = false  // i made it unchecked so the user can check who is needed
                    }).ToList();

                    var awayCheckBoxes = awayTeamPlayers.Select(player => new CheckOptionVM
                    {
                        ID = player.ID,
                        DisplayText = player.AssignPlayer,
                        Assigned = false  // i made it unchecked so the user can check who is needed
                    }).ToList();

                    ViewData["HomePlayersOptions"] = homeCheckBoxes;
                    ViewData["AwayPlayersOptions"] = awayCheckBoxes;
                }
            }
            else
            {
                ViewData["HomePlayersOptions"] = new List<CheckOptionVM>();
                ViewData["AwayPlayersOptions"] = new List<CheckOptionVM>();
                ViewData["HomeTeamName"] = "Home Team ???";
                ViewData["AwayTeamName"] = "Away Team ???";
            }
        }




        // GET: Lineups/Edit/5
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null || _context.Lineups == null)
        //    {
        //        return NotFound();
        //    }

        //    var lineup = await _context.Lineups
        //        .Include(f => f.Games).ThenInclude(fr => fr.Player)
        //        .FirstOrDefaultAsync(f => f.ID == id);

        //    if (lineup == null)
        //    {
        //        return NotFound();
        //    }

        //    PopulateDropDownLists(lineup);
        //    return View(lineup);
        //}


        // POST: Lineups/Edit/5
        //To protect from overposting attacks, enable the specific properties you want to bind to.
        //For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, string[] selectedOptions)
        //{
        //    var lineupToUpdate = await _context.Lineups
        //        .Include(f => f.Games).ThenInclude(fr => fr.HomeTeam.Players)
        //        .Include(f => f.Games).ThenInclude(fr => fr.AwayTeam.Players)
        //        .FirstOrDefaultAsync(f => f.ID == id);

        //    UpdatePlayersListBoxes(selectedOptions, lineupToUpdate);

        //    if (lineupToUpdate == null)
        //    {
        //        return NotFound();
        //    }
        //    if (await TryUpdateModelAsync<Lineup>(lineupToUpdate, "",
        //        f => f.AwayTeamID, f => f.HomeTeamID))
        //    {
        //        try
        //        {
        //            await _context.SaveChangesAsync();
        //            return RedirectToAction("Details", new { lineupToUpdate.ID });
        //        }
        //        catch (RetryLimitExceededException /* dex */)
        //        {
        //            ModelState.AddModelError("", "Unable to save changes after multiple attempts. Try again, and if the problem persists, see your system administrator.");
        //        }
        //        catch (DbUpdateException)
        //        {
        //            ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
        //        }
        //    }
        //  //  TeamPlayersCheckboxes(lineupToUpdate);
        //    PopulateDropDownLists(lineupToUpdate);
        //    return View(lineupToUpdate);
        //}

        //// GET: Lineups/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null || _context.Lineups == null)
        //    {
        //        return NotFound();
        //    }

        //    var lineup = await _context.Lineups
        //        .Include(l => l.Game)
        //        .FirstOrDefaultAsync(m => m.ID == id);
        //    if (lineup == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(lineup);
        //}

        //// POST: Lineups/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    if (_context.Lineups == null)
        //    {
        //        return Problem("Entity set 'WmbaContext.Lineups'  is null.");
        //    }
        //    var lineup = await _context.Lineups.FindAsync(id);
        //    if (lineup != null)
        //    {
        //        _context.Lineups.Remove(lineup);
        //    }

        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}




        //private void PopulatePlayersListBoxes(Lineup lineup)
        //{
        //    //For this to work, you must have Included the child collection in the parent object
        //    var allOptions = _context.Players;
        //    var currentOptionsHS = new HashSet<int>((IEnumerable<int>)lineup.Games.Select(b => b.PlayerID));
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
        //                DisplayText = r.Summary
        //            });
        //        }
        //        else
        //        {
        //            available.Add(new ListOptionVM
        //            {
        //                ID = r.ID,
        //                DisplayText = r.Summary
        //            });
        //        }
        //    }

        //    ViewData["selOpts"] = new MultiSelectList(selected.OrderBy(s => s.DisplayText), "ID", "DisplayText");
        //    ViewData["availOpts"] = new MultiSelectList(available.OrderBy(s => s.DisplayText), "ID", "DisplayText");
        //}



        //private void UpdatePlayersListBoxes(string[] selectedOptions, Lineup lineupToUpdate)
        //{
        //    if (selectedOptions == null)
        //    {
        //        lineupToUpdate.Games = new List<Game>();
        //        return;
        //    }

        //    var selectedOptionsHS = new HashSet<string>(selectedOptions);
        //    var currentOptionsHS = new HashSet<int>((IEnumerable<int>)lineupToUpdate.Games.Select(b => b.PlayerID));
        //    foreach (var r in _context.Players)
        //    {
        //        if (selectedOptionsHS.Contains(r.ID.ToString()))//it is selected
        //        {
        //            if (!currentOptionsHS.Contains(r.ID))//but not currently in the Function's collection - Add it!
        //            {
        //                lineupToUpdate.Games.Add(new Game
        //                {
        //                    PlayerID = r.ID,
        //                    LineupID = lineupToUpdate.ID
        //                });
        //            }
        //        }
        //        else //not selected
        //        {
        //            if (currentOptionsHS.Contains(r.ID))//but is currently in the Function's collection - Remove it!
        //            {
        //                Game playerToRemove = lineupToUpdate.Games.FirstOrDefault(d => d.HomeTeamID == r.ID);
        //                _context.Remove(playerToRemove);
        //            }
        //        }
        //    }
        //}


        private SelectList HomeTeamList(int? selectedId)
        {
            return new SelectList(_context
                .HomeTeams
                .OrderBy(m => m.TmName), "ID", "TmName", selectedId);
        }
        private SelectList AwayTeamList(int? selectedId)
        {
            return new SelectList(_context
                .AwayTeams
                .OrderBy(m => m.TmName), "ID", "TmName", selectedId);
        }
        private void PopulateDropDownLists(Lineup lineup = null)
        {
            ViewData["HomeTeamID"] = HomeTeamList(lineup?.HomeTeamID);
            ViewData["AwayTeamID"] = AwayTeamList(lineup?.AwayTeamID);
        }

        private bool LineupExists(int id)
        {
            return _context.Lineups.Any(e => e.ID == id);
        }
    }
}
