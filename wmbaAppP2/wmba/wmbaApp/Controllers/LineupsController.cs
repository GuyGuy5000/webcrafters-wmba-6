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
using System.Numerics;

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
        public IActionResult Index()
        {
            return View();
        }

        // GET: Lineups/Create
        public IActionResult Create(int gameId)
        {
            var game = _context.Games
                .Include(g => g.HomeTeam).ThenInclude(t => t.Players.Where(p => p.IsActive == true))
                .Include(g => g.AwayTeam).ThenInclude(t => t.Players.Where(p => p.IsActive == true))
                .FirstOrDefault(g => g.ID == gameId);

            if (game == null)
            {
                return NotFound();
            }

            Lineup lineup = new Lineup();
            ViewData["GameId"] = gameId;
            TeamPlayersCheckboxes(gameId);
            return View(lineup);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID")] Lineup lineup, int gameId, List<int> SelectedPlayers)
        {
            if (ModelState.IsValid)
            {
                var game = await _context.Games
                    .Include(g => g.HomeTeam).ThenInclude(t => t.Players.Where(p => p.IsActive == true))
                    .Include(g => g.AwayTeam).ThenInclude(t => t.Players.Where(p => p.IsActive == true))
                    .FirstOrDefaultAsync(g => g.ID == gameId);

                if (game != null)
                {
                    var selectedHomePlayersCount = SelectedPlayers.Count(p => game.HomeTeam.Players.Any(hp => hp.ID == p));
                    var selectedAwayPlayersCount = SelectedPlayers.Count(p => game.AwayTeam.Players.Any(ap => ap.ID == p));

                    lineup.HomeGames = new List<Game> { game };
                    lineup.AwayGames = new List<Game> { game };

                    foreach (var playerId in SelectedPlayers)
                    {
                        var playerLineup = new PlayerLineup
                        {
                            Lineup = lineup,
                            PlayerID = playerId
                        };
                        lineup.PlayerLineups.Add(playerLineup);
                    }

                    _context.Lineups.Add(lineup);
                    await _context.SaveChangesAsync();

                    // goes to the details page
                    return RedirectToAction("Details", "Games", new { id = game.ID });
                }
            }
            return View(lineup);
        }


        // GET: Lineups/Edit/5
        public IActionResult Edit(int gameId)
        {
            var firstCreatedLineup = _context.Lineups
                .Include(p => p.PlayerLineups)
                .Include(l => l.HomeGames)
            .FirstOrDefault(l => l.HomeGames.Any(g => g.ID == gameId));


            //AwayGames lineup
            var firstAwayCreatedLineup = _context.Lineups
                .Include(p => p.PlayerLineups)
                .Include(l => l.AwayGames)
            .FirstOrDefault(l => l.AwayGames.Any(g => g.ID == gameId));

            if (firstCreatedLineup == null && firstAwayCreatedLineup == null)
            {
                return NotFound();
            }

            ViewData["GameId"] = gameId;
            TeamPlayersCheckboxes(gameId, firstCreatedLineup, firstAwayCreatedLineup);
            return View(firstCreatedLineup);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID")] Lineup lineup, int gameId, List<int> SelectedPlayers)
        {
            if (id != lineup.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var firstCreatedLineup = _context.Lineups
                    .Include(l => l.PlayerLineups)
                    .FirstOrDefault(l => l.ID == id);

                if (firstCreatedLineup != null)
                {
                    firstCreatedLineup.PlayerLineups.Clear();
                    foreach (var playerId in SelectedPlayers)
                    {
                        var playerLineup = new PlayerLineup
                        {
                            Lineup = firstCreatedLineup,
                            PlayerID = playerId
                        };
                        firstCreatedLineup.PlayerLineups.Add(playerLineup);
                    }

                    _context.Update(firstCreatedLineup);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Details", "Games", new { id = gameId });
                }
                else
                {
                    return NotFound();
                }
            }

            ViewData["GameId"] = gameId;
            TeamPlayersCheckboxes(gameId);
            return View(lineup);
        }


        // GET: Lineups/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lineup = await _context.Lineups
                .Include(l => l.PlayerLineups).ThenInclude(pl => pl.Player.IsActive)
                .Include(l => l.HomeGames).ThenInclude(g => g.HomeTeam.Players)
                .Include(l => l.AwayGames).ThenInclude(g => g.AwayTeam.Players)
                .FirstOrDefaultAsync(m => m.ID == id);

            if (lineup == null)
            {
                return NotFound();
            }



            ViewData["SelectedHomePlayers"] = lineup.HomeGames?.FirstOrDefault()?.HomeTeam?.Players
                .Where(p => lineup.PlayerLineups.Any(pl => pl.PlayerID == p.ID))
                .ToList();

            ViewData["SelectedAwayPlayers"] = lineup.AwayGames?.FirstOrDefault()?.AwayTeam?.Players
                .Where(p => lineup.PlayerLineups.Any(pl => pl.PlayerID == p.ID))
                .ToList();

            ViewData["HomeTeamName"] = lineup.HomeGames?.FirstOrDefault().HomeTeam.TmName;
            ViewData["AwayTeamName"] = lineup.AwayGames?.FirstOrDefault().AwayTeam.TmName;



            return View(lineup);
        }


        private void TeamPlayersCheckboxes(int gameId, Lineup homeLineup = null, Lineup awayLineup = null)
        {
            var game = _context.Games
                .Include(g => g.HomeTeam.Players.Where(p => p.IsActive == true))
                .Include(g => g.AwayTeam.Players.Where(p => p.IsActive == true))
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

                    var homeCheckBoxes = homeTeamPlayers.Select(player => new CheckOptionVM
                    {
                        ID = player.ID,
                        DisplayText = player.AssignPlayer,
                        Assigned = homeLineup?.PlayerLineups.Any(pl => pl.PlayerID == player.ID) ?? false
                    }).ToList();

                    var awayCheckBoxes = awayTeamPlayers.Select(player => new CheckOptionVM
                    {
                        ID = player.ID,
                        DisplayText = player.AssignPlayer,
                        Assigned = awayLineup?.PlayerLineups.Any(pl => pl.PlayerID == player.ID) ?? false
                    }).ToList();

                    ViewData["HomePlayersOptions"] = homeCheckBoxes;
                    ViewData["AwayPlayersOptions"] = awayCheckBoxes;
                }
            }
            else
            {
                ModelState.AddModelError("", "Error, check if you have a game created, if yes and Error persists contact Administrator.");
            }
        }

        private bool LineupExists(int id)
        {
            return _context.Lineups.Any(e => e.ID == id);
        }
    }
}
