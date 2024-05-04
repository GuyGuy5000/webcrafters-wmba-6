/// <summary>
/// Game
/// Farooq Jidelola
/// Emmanuel James
/// </summary>
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using OfficeOpenXml.Style;
using OfficeOpenXml.Table;
using wmbaApp.CustomControllers;
using wmbaApp.Data;
using wmbaApp.Models;
using wmbaApp.Utilities;
using wmbaApp.ViewModels;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace wmbaApp.Controllers
{
    [Authorize]
    public class GamesController : ElephantController
    {
        private readonly WmbaContext _context;
        private readonly ApplicationDbContext _AppContext;

        public GamesController(WmbaContext context, ApplicationDbContext appContext)
        {
            _context = context;
            _AppContext = appContext;

        }

        public async Task<IActionResult> Index(string SearchString, int? TeamID,
                    int? page, int? pageSizeID, string actionButton, string sortDirection = "asc", string sortField = "")
        {
            // Count the number of filters applied - start by assuming no filters
            ViewData["Filtering"] = " btn-outline-dark";
            int numberFilters = 0;
            // Then in each "test" for filtering, add to the count of Filters applied
            if (TempData["SuccessMessage"] != null)
            {
                ViewBag.SuccessMessage = TempData["SuccessMessage"].ToString();
            }
            // List of sort options.
            // NOTE: make sure this array has matching values to the column headings
            string[] sortOptions = new[] { "Teams", "Location" };

            PopulateDropDownLists();

            IQueryable<Game> games;

            if (User.IsInRole("Admin"))
            {
                games = _context.Games
                 .Include(g => g.GameLocation)
                 .Include(g => g.AwayTeam).ThenInclude(p => p.Division)
                 .Include(g => g.HomeTeam).ThenInclude(p => p.Division)
                 .Include(g => g.HomeLineup).ThenInclude(hl => hl.PlayerLineups).ThenInclude(pl => pl.Player)
                 .Include(g => g.AwayLineup).ThenInclude(hl => hl.PlayerLineups).ThenInclude(pl => pl.Player)
                 .Where(g => g.GameEndTime > DateTime.Now)
                 .OrderBy(g => g.GameStartTime)
                 .AsNoTracking();
            }
            else
            {
                var rolesTeamIDs = await UserRolesHelper.GetUserTeamIDs(_AppContext, User);
                var rolesDivisionIDs = await UserRolesHelper.GetUserDivisionIDs(_AppContext, User);

                games = _context.Games
                     .Include(g => g.GameLocation)
                     .Include(g => g.AwayTeam).ThenInclude(p => p.Division)
                     .Include(g => g.HomeTeam).ThenInclude(p => p.Division)
                     .Include(g => g.HomeLineup).ThenInclude(hl => hl.PlayerLineups).ThenInclude(pl => pl.Player)
                     .Include(g => g.AwayLineup).ThenInclude(hl => hl.PlayerLineups).ThenInclude(pl => pl.Player)
                     .Where(g => g.GameEndTime > DateTime.Now && ((rolesTeamIDs.Contains(g.HomeTeamID) || rolesTeamIDs.Contains(g.AwayTeamID) || rolesDivisionIDs.Contains(g.DivisionID))))
                     .OrderBy(g => g.GameStartTime)
                     .AsNoTracking();
            }




            if (!System.String.IsNullOrEmpty(SearchString))
            {
                games = games.Where(g => g.GameLocation.Name.ToUpper().Contains(SearchString.ToUpper())
                || g.HomeTeam.TmName.ToUpper().Contains(SearchString.ToUpper())
                || g.AwayTeam.TmName.ToUpper().Contains(SearchString.ToUpper())
                || g.Division.DivName.ToUpper().Contains(SearchString.ToUpper()));

                numberFilters++;
            }

            // Give feedback about the state of the filters
            if (numberFilters != 0)
            {
                // Toggle the Open/Closed state of the collapse depending on if we are filtering
                ViewData["Filtering"] = " btn-outline-dark";
                // Show how many filters have been applied
                ViewData["numberFilters"] = "(" + numberFilters.ToString()
                    + " Filter" + (numberFilters > 1 ? "s" : "") + " Applied)";
            }

            if (!System.String.IsNullOrEmpty(actionButton)) // Form submitted!
            {
                page = 1; // Reset page to start

                if (sortOptions.Contains(actionButton)) // Change of sort is requested
                {
                    if (actionButton == sortField) // Reverse order on same field
                    {
                        sortDirection = sortDirection == "asc" ? "desc" : "asc";
                    }
                    sortField = actionButton; // Sort by the button clicked
                }
            }

            // Sort data based on the selected field and direction
            if (sortField == "Teams")
            {
                games = sortDirection == "asc" ? games.OrderBy(g => g.HomeTeam) : games.OrderByDescending(g => g.HomeTeam);
            }
            else if (sortField == "Location")
            {
                games = sortDirection == "asc" ? games.OrderBy(g => g.GameLocation.Name) : games.OrderByDescending(g => g.GameLocation.Name);
            }


            // Store sort field and direction in ViewData
            ViewData["sortField"] = sortField;
            ViewData["sortDirection"] = sortDirection;

            // Set page size
            int pageSize = PageSizeHelper.SetPageSize(HttpContext, pageSizeID, ControllerName());
            ViewData["pageSizeID"] = PageSizeHelper.PageSizeList(pageSize);

            // Paginate the data
            var pagedData = await PaginatedList<Game>.CreateAsync(games, page ?? 1, pageSize);

            return View(pagedData);
        }


        public async Task<IActionResult> FinishedGames(string SearchString, int? TeamID,
            int? page, int? pageSizeID, string actionButton, string sortDirection = "asc", string sortField = "")
        {
            // Count the number of filters applied - start by assuming no filters
            ViewData["Filtering"] = "btn-outline-dark";
            int numberFilters = 0;
            // Then in each "test" for filtering, add to the count of Filters applied

            // List of sort options.
            // NOTE: make sure this array has matching values to the column headings
            string[] sortOptions = new[] { "Teams", "Location" };

            PopulateDropDownLists();

            IQueryable<Game> games;

            if (User.IsInRole("Admin"))
            {
                games = _context.Games
                 .Include(g => g.GameLocation)
                 .Include(g => g.AwayTeam).ThenInclude(p => p.Division)
                 .Include(g => g.HomeTeam).ThenInclude(p => p.Division)
                 .Include(g => g.HomeLineup).ThenInclude(hl => hl.PlayerLineups).ThenInclude(pl => pl.Player)
                 .Include(g => g.AwayLineup).ThenInclude(hl => hl.PlayerLineups).ThenInclude(pl => pl.Player)
                 .Where(g => g.GameEndTime < DateTime.Now)
                 .OrderBy(g => g.GameStartTime)
                 .AsNoTracking();
            }
            else
            {
                var rolesTeamIDs = await UserRolesHelper.GetUserTeamIDs(_AppContext, User);
                var rolesDivisionIDs = await UserRolesHelper.GetUserDivisionIDs(_AppContext, User);

                games = _context.Games
                     .Include(g => g.GameLocation)
                     .Include(g => g.AwayTeam).ThenInclude(p => p.Division)
                     .Include(g => g.HomeTeam).ThenInclude(p => p.Division)
                     .Include(g => g.HomeLineup).ThenInclude(hl => hl.PlayerLineups).ThenInclude(pl => pl.Player)
                     .Include(g => g.AwayLineup).ThenInclude(hl => hl.PlayerLineups).ThenInclude(pl => pl.Player)
                     .Where(g => g.GameEndTime < DateTime.Now && ((rolesTeamIDs.Contains(g.HomeTeamID) || rolesTeamIDs.Contains(g.AwayTeamID) || rolesDivisionIDs.Contains(g.DivisionID))))
                     .OrderBy(g => g.GameStartTime)
                     .AsNoTracking();
            }




            if (!System.String.IsNullOrEmpty(SearchString))
            {
                games = games.Where(g => g.GameLocation.Name.ToUpper().Contains(SearchString.ToUpper())
                || g.HomeTeam.TmName.ToUpper().Contains(SearchString.ToUpper())
                || g.AwayTeam.TmName.ToUpper().Contains(SearchString.ToUpper())
                || g.Division.DivName.ToUpper().Contains(SearchString.ToUpper()));

                numberFilters++;
            }

            // Give feedback about the state of the filters
            if (numberFilters != 0)
            {
                // Toggle the Open/Closed state of the collapse depending on if we are filtering
                ViewData["Filtering"] = " btn-outline-dark";
                // Show how many filters have been applied
                ViewData["numberFilters"] = "(" + numberFilters.ToString()
                    + " Filter" + (numberFilters > 1 ? "s" : "") + " Applied)";
            }

            if (!System.String.IsNullOrEmpty(actionButton)) // Form submitted!
            {
                page = 1; // Reset page to start

                if (sortOptions.Contains(actionButton)) // Change of sort is requested
                {
                    if (actionButton == sortField) // Reverse order on same field
                    {
                        sortDirection = sortDirection == "asc" ? "desc" : "asc";
                    }
                    sortField = actionButton; // Sort by the button clicked
                }
            }

            // Sort data based on the selected field and direction
            if (sortField == "Teams")
            {
                games = sortDirection == "asc" ? games.OrderBy(g => g.HomeTeam) : games.OrderByDescending(g => g.HomeTeam);
            }
            else if (sortField == "Location")
            {
                games = sortDirection == "asc" ? games.OrderBy(g => g.GameLocation.Name) : games.OrderByDescending(g => g.GameLocation.Name);
            }


            // Store sort field and direction in ViewData
            ViewData["sortField"] = sortField;
            ViewData["sortDirection"] = sortDirection;

            // Set page size
            int pageSize = PageSizeHelper.SetPageSize(HttpContext, pageSizeID, ControllerName());
            ViewData["pageSizeID"] = PageSizeHelper.PageSizeList(pageSize);

            // Paginate the data
            var pagedData = await PaginatedList<Game>.CreateAsync(games, page ?? 1, pageSize);

            return View(pagedData);
        }


        // GET: Games/Create
        [Authorize(Roles = "Admin,Convenor")]
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
        [Authorize(Roles = "Admin,Convenor")]
        public async Task<IActionResult> Create([Bind("ID,GameStartTime,GameEndTime,IsActive,GameLocationID,HomeTeamID,AwayTeamID,DivisionID")] Game game,
            int? selectedDivision, IFormFile theExcel)
        {
            if (!await UserRolesHelper.IsAuthorizedForGame(_AppContext, User, game))
                return RedirectToAction("Index", "Games");

            if (selectedDivision.HasValue)
            {
                ViewData["HomeTeamID"] = new SelectList(_context.Teams.Where(t => t.DivisionID == selectedDivision && t.IsActive == true), "ID", "TmName");
                ViewData["AwayTeamID"] = new SelectList(_context.Teams.Where(t => t.DivisionID == selectedDivision && t.IsActive == true), "ID", "TmName");
            }
            try
            {
                if (ModelState.IsValid)
                {

                    _context.Add(game);
                    TempData["SuccessMessage"] = "Game created successfully.";
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Details", new { game.ID });
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error creating player: {ex.Message}";
            }
            //ViewData["AwayTeamID"] = new SelectList(_context.Teams, "ID", "TmName", game.AwayTeamID);
            //ViewData["HomeTeamID"] = new SelectList(_context.Teams, "ID", "TmName", game.HomeTeamID);
            PopulateDropDownLists();
            return View(game);
        }

        // GET: Games/Edit/5
        [Authorize(Roles = "Admin,Convenor")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Games == null)
            {
                return NotFound();
            }

            var game = await _context.Games
                .Include(g => g.GameLocation)
                .Include(p => p.HomeTeam).ThenInclude(p => p.Players)
                .Include(p => p.AwayTeam).ThenInclude(p => p.Players)
                .Include(g => g.HomeLineup).ThenInclude(hl => hl.PlayerLineups).ThenInclude(pl => pl.Player)
                .Include(g => g.AwayLineup).ThenInclude(al => al.PlayerLineups).ThenInclude(pl => pl.Player)
                .FirstOrDefaultAsync(m => m.ID == id);

            if (game == null)
            {
                return NotFound();
            }

            if (!await UserRolesHelper.IsAuthorizedForGame(_AppContext, User, game))
                return RedirectToAction("Index", "Games");

            ViewData["HomeLineupID"] = new SelectList(_context.Lineups, "ID", "ID", game.HomeLineupID);
            ViewData["AwayLineupID"] = new SelectList(_context.Lineups, "ID", "ID", game.AwayLineupID);
            PopulateDropDownLists(game);
            return View(game);
        }

        // POST: Games/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Convenor")]
        public async Task<IActionResult> Edit(int id)
        {
            var gameToUpdate = await _context.Games
                .Include(g => g.GameLocation)
                .Include(p => p.HomeTeam).ThenInclude(p => p.Players)
                .Include(p => p.AwayTeam).ThenInclude(p => p.Players)
                .Include(g => g.HomeLineup).ThenInclude(hl => hl.PlayerLineups).ThenInclude(pl => pl.Player)
                .Include(g => g.AwayLineup).ThenInclude(al => al.PlayerLineups).ThenInclude(pl => pl.Player)
                .FirstOrDefaultAsync(m => m.ID == id);

            if (gameToUpdate == null)
            {
                return NotFound();
            }

            if (!await UserRolesHelper.IsAuthorizedForGame(_AppContext, User, gameToUpdate))
                return RedirectToAction("Index", "Games");

            if (await TryUpdateModelAsync<Game>(gameToUpdate, "",
                t => t.GameStartTime, t => t.GameEndTime, t => t.GameLocationID, t => t.DivisionID,
                t => t.AwayTeamID, t => t.HomeTeamID))
            {

                try
                {
                    _context.Update(gameToUpdate);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Game updated successfully.";
                    return RedirectToAction("Details", new { gameToUpdate.ID });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GameExists(gameToUpdate.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Error updating player: {ex.Message}";
                }
            }
            ViewData["HomeLineupID"] = new SelectList(_context.Lineups, "ID", "ID", gameToUpdate.HomeLineupID);
            ViewData["AwayLineupID"] = new SelectList(_context.Lineups, "ID", "ID", gameToUpdate.AwayLineupID);
            PopulateDropDownLists(gameToUpdate);
            return View(gameToUpdate);
        }

        // GET: Games/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            if (TempData["SuccessMessage"] != null)
            {
                ViewBag.SuccessMessage = TempData["SuccessMessage"].ToString();
            }

            // Retrieve the game from the database
            var game = await _context.Games
                .Include(g => g.GameLocation)
                .Include(p => p.HomeTeam).ThenInclude(p => p.Players)
                .Include(p => p.AwayTeam).ThenInclude(p => p.Players)
                .Include(g => g.HomeLineup).ThenInclude(hl => hl.PlayerLineups).ThenInclude(pl => pl.Player)
                    .ThenInclude(p => p.Statistics) // Include player statistics
                .Include(g => g.AwayLineup).ThenInclude(al => al.PlayerLineups).ThenInclude(pl => pl.Player)
                    .ThenInclude(p => p.Statistics) // Include player statistics
                .FirstOrDefaultAsync(m => m.ID == id);

            if (game == null)
            {
                return NotFound();
            }

            if (!await UserRolesHelper.IsAuthorizedForGame(_AppContext, User, game))
            {
                return RedirectToAction("Index", "Games");
            }

            // Check if there are players for home and away teams
            bool hasHomePlayers = game?.HomeTeam?.Players?.Any() ?? false;
            bool hasAwayPlayers = game?.AwayTeam?.Players?.Any() ?? false;

            // Pass the flags to the view
            ViewData["HasHomePlayers"] = hasHomePlayers;
            ViewData["HasAwayPlayers"] = hasAwayPlayers;

            // Calculate stats only if there are players
            if (hasHomePlayers)
            {
                // Calculate total stats for home team
                double totalHomeStats = CalculateTotalStats(game.HomeTeam.Players);
                double totalPossibleHomeStats = CalculateTotalPossibleStats(game.HomeTeam.Players);
                double homeTeamPercentage = totalHomeStats / totalPossibleHomeStats * 100;
                ViewData["HomeTeamPercentage"] = homeTeamPercentage;
            }

            if (hasAwayPlayers)
            {
                // Calculate total stats for away team
                double totalAwayStats = CalculateTotalStats(game.AwayTeam.Players);
                double totalPossibleAwayStats = CalculateTotalPossibleStats(game.AwayTeam.Players);
                double awayTeamPercentage = totalAwayStats / totalPossibleAwayStats * 100;
                ViewData["AwayTeamPercentage"] = awayTeamPercentage;
            }

            return View(game);
        }

        private int CalculateTotalStats(IEnumerable<Player> players)
        {
            int totalStats = 0;

            if (players != null)
            {
                foreach (var player in players)
                {
                    // Calculate the sum of player statistics
                    totalStats += CalculatePlayerStatistics(player.Statistics);
                }
            }

            return totalStats;
        }

        private int CalculatePlayerStatistics(Statistic statistics)
        {
            if (statistics != null)
            {
                return (int)((statistics.StatsH ?? 0) + (statistics.StatsR ?? 0) + (statistics.StatsHR ?? 0) +
                             (statistics.StatsAB ?? 0) + (statistics.StatsAVG ?? 0));
            }
            else
            {
                return 0; // or handle the null case as per your logic
            }
        }


        private double CalculateTotalPossibleStats(IEnumerable<Player> players)
        {
            const int MaxHits = 200;
            const int MaxRuns = 100;
            const int MaxHomeRuns = 50;
            const int MaxAtBats = 500;
            const double MaxBattingAverage = 1.0;

            double totalPossibleStats = 0;

            if (players != null)
            {
                foreach (var player in players)
                {
                    // Calculate the potential maximum stats for each player
                    double potentialStats = MaxHits + MaxRuns + MaxHomeRuns + MaxAtBats + MaxBattingAverage;

                    // Accumulate potential stats for all players
                    totalPossibleStats += potentialStats;
                }
            }

            return totalPossibleStats;
        }




        // GET: Games/Details/5
        [Authorize]
        public async Task<IActionResult> GameSummary(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var game = await _context.Games
                .Include(g => g.GameLocation)
                .Include(p => p.HomeTeam)
                .Include(p => p.AwayTeam)
                .Include(g => g.HomeLineup).ThenInclude(hl => hl.PlayerLineups).ThenInclude(pl => pl.Player)
                .Include(g => g.AwayLineup).ThenInclude(al => al.PlayerLineups).ThenInclude(pl => pl.Player)
                .Include(g => g.Innings).ThenInclude(i => i.PlayByPlays).ThenInclude(pl => pl.PlayerAction)
                .FirstOrDefaultAsync(m => m.ID == id);

            if (game == null)
            {
                return NotFound();
            }

            if (!await UserRolesHelper.IsAuthorizedForGame(_AppContext, User, game))
                return RedirectToAction("Index", "Games");

            var plays = game.Innings.SelectMany(i => i.PlayByPlays);
            List<Statistic> gameStats = new();

            if (plays.Any(p => game.HomeLineup.PlayerLineups.Any(pl => pl.PlayerID == p.PlayerID))) //if plays contain players from the homelineup
            {
                foreach (Player player in game.HomeLineup.PlayerLineups.Select(p => p.Player)) //get stats from homelineup
                {
                    List<PlayByPlay> playerPlays = plays.Where(p => p.PlayerID == player.ID).ToList();

                    Statistic playerStats = new()
                    {
                        StatsAB = playerPlays.Count(p => p.PlayerAction.PlayerActionName.ToLower() == "single"
                                                 || p.PlayerAction.PlayerActionName.ToLower() == "double"
                                                 || p.PlayerAction.PlayerActionName.ToLower() == "triple"
                                                 || p.PlayerAction.PlayerActionName.ToLower() == "out"
                                                 || p.PlayerAction.PlayerActionName.ToLower() == "fly out"
                                                 || p.PlayerAction.PlayerActionName.ToLower() == "ground out"
                                                 || p.PlayerAction.PlayerActionName.ToLower() == "home run"),

                        StatsR = playerPlays.Count(p => p.PlayerAction.PlayerActionName.ToLower() == "run"
                                                || p.PlayerAction.PlayerActionName.ToLower() == "home run"),

                        StatsH = playerPlays.Count(p => p.PlayerAction.PlayerActionName.ToLower() == "single"
                                                 || p.PlayerAction.PlayerActionName.ToLower() == "double"
                                                 || p.PlayerAction.PlayerActionName.ToLower() == "triple"
                                                 || p.PlayerAction.PlayerActionName.ToLower() == "home run"),

                        StatsBB = playerPlays.Count(p => p.PlayerAction.PlayerActionName.ToLower() == "walk"),
                        StatsHR = playerPlays.Count(p => p.PlayerAction.PlayerActionName.ToLower() == "home run"),
                    };
                    playerStats.Players.Add(player);
                    gameStats.Add(playerStats);
                }
            }
            else
            {
                foreach (Player player in game.AwayLineup.PlayerLineups.Select(p => p.Player)) //get stats from awaylineup
                {
                    List<PlayByPlay> playerPlays = plays.Where(p => p.PlayerID == player.ID).ToList();

                    Statistic playerStats = new()
                    {
                        StatsAB = playerPlays.Count(p => p.PlayerAction.PlayerActionName.ToLower() == "single"
                                                 || p.PlayerAction.PlayerActionName.ToLower() == "double"
                                                 || p.PlayerAction.PlayerActionName.ToLower() == "triple"
                                                 || p.PlayerAction.PlayerActionName.ToLower() == "out"
                                                 || p.PlayerAction.PlayerActionName.ToLower() == "fly out"
                                                 || p.PlayerAction.PlayerActionName.ToLower() == "ground out"
                                                 || p.PlayerAction.PlayerActionName.ToLower() == "home run"),

                        StatsR = playerPlays.Count(p => p.PlayerAction.PlayerActionName.ToLower() == "run"),

                        StatsH = playerPlays.Count(p => p.PlayerAction.PlayerActionName.ToLower() == "single"
                                                 || p.PlayerAction.PlayerActionName.ToLower() == "double"
                                                 || p.PlayerAction.PlayerActionName.ToLower() == "triple"
                                                 || p.PlayerAction.PlayerActionName.ToLower() == "home run"),

                        StatsBB = playerPlays.Count(p => p.PlayerAction.PlayerActionName.ToLower() == "walk"),
                        StatsHR = playerPlays.Count(p => p.PlayerAction.PlayerActionName.ToLower() == "home run"),
                    };
                    playerStats.Players.Add(player);
                    gameStats.Add(playerStats);
                }
            }

            ViewBag.GameStats = gameStats;
            return View(game);
        }


    

        public IActionResult SelectLineup(int gameId)
        {
            return RedirectToAction("Create", "Lineups", new { gameId });
        }

        #region selectlist
        private SelectList DivisionSelectList(int? selectedId)
        {
            return new SelectList(_context
                .Divisions
                .OrderBy(m => m.DivName), "ID", "DivName", selectedId);
        }

        private SelectList GameLocationSelectList(int? selectedId)
        {
            return new SelectList(_context
                .GameLocations
                .OrderBy(m => m.Name), "ID", "Name", selectedId);
        }
        private SelectList HomeTeamSelectList(int? selectedId)
        {
            return new SelectList(_context
                .HomeTeams
                .OrderBy(m => m.TmName), "ID", "TmName", selectedId);
        }
        private SelectList AwayTeamSelectList(int? selectedId)
        {
            return new SelectList(_context
                .AwayTeams
                .OrderBy(m => m.TmName), "ID", "TmName", selectedId);
        }


        private void PopulateDropDownLists(Game game = null)
        {
            ViewData["DivisionID"] = DivisionSelectList(game?.DivisionID);
            ViewData["GameLocationID"] = GameLocationSelectList(game?.GameLocationID);
            ViewData["HomeTeamID"] = HomeTeamSelectList(game?.HomeTeamID);
            ViewData["AwayTeamID"] = AwayTeamSelectList(game?.AwayTeamID);
        }

        #endregion

        [HttpGet]
        public JsonResult GetGameLocations(int? id)
        {
            return Json(GameLocationSelectList(id));
        }


        [HttpGet]
        public JsonResult GetTeamsByDivision(int divisionId)
        {
            var teams = _context.Teams
                .Where(t => t.DivisionID == divisionId)
                .Select(t => new { id = t.ID, name = t.TmName })
                .ToList();

            return Json(new { teams });
        }


        [HttpPost]
        public async Task<IActionResult> InsertFromExcel(IFormFile theExcel)
        {
            string feedBack = string.Empty;
            if (theExcel != null)
            {
                string mimeType = theExcel.ContentType;
                long fileLength = theExcel.Length;
                if (!(mimeType == "" || fileLength == 0))//Looks like we have a file!!!
                {
                    if (mimeType.Contains("excel") || mimeType.Contains("spreadsheet"))
                    {
                        ExcelPackage excel;
                        using (var memoryStream = new MemoryStream())
                        {
                            await theExcel.CopyToAsync(memoryStream);
                            excel = new ExcelPackage(memoryStream);
                        }

                        var workSheet = excel.Workbook.Worksheets[0];
                        var start = workSheet.Dimension.Start;
                        var end = workSheet.Dimension.End;
                        int successCount = 0;
                        int errorCount = 0;

                        if ((workSheet.Cells[1, 6].Text == "HomeTeam") && (workSheet.Cells[1, 7].Text == "AwayTeam") &&
                            (workSheet.Cells[1, 9].Text == "HomeDivision") && (workSheet.Cells[1, 8].Text == "Location") &&
                            (workSheet.Cells[1, 11].Text == "sDate") && (workSheet.Cells[1, 12].Text == "eDate"))
                        {
                            for (int row = start.Row + 1; row <= end.Row; row++)
                            {
                                Game g = new Game();
                                try
                                {
                                    int? homeTeam = _context.HomeTeams.FirstOrDefault(t => workSheet.Cells[row, 6].Text.Contains(t.TmName)).ID;
                                    int? awayTeam = _context.AwayTeams.FirstOrDefault(t => workSheet.Cells[row, 7].Text.Contains(t.TmName)).ID;
                                    int? division = _context.Divisions.FirstOrDefault(t => workSheet.Cells[row, 1].Text.Contains(t.DivName)).ID;


                                    // Row by row...
                                    g.HomeTeamID = (int)homeTeam;
                                    g.AwayTeamID = (int)awayTeam;
                                    g.DivisionID = (int)division;
                                    g.GameLocation = await _context.GameLocations.FindAsync(workSheet.Cells[row, 4].Text);
                                    g.GameStartTime = DateTime.Parse(workSheet.Cells[row, 5].Text);
                                    g.GameEndTime = DateTime.Parse(workSheet.Cells[row, 6].Text);


                                    _context.Games.Add(g);
                                    _context.SaveChanges();
                                    successCount++;
                                }
                                catch (DbUpdateException dex)
                                {
                                    errorCount++;
                                    if (dex.GetBaseException().Message.Contains("UNIQUE constraint failed"))
                                    {
                                        feedBack += "Error: Record " + g.Division + " was rejected as a duplicate."
                                                + "<br />";
                                    }
                                    else
                                    {
                                        feedBack += "Error: Record " + g.Division + " caused an error."
                                                + "<br />";
                                    }
                                    //Here is the trick to using SaveChanges in a loop.  You must remove the 
                                    //offending object from the cue or it will keep raising the same error.
                                    _context.Remove(g);
                                }
                                catch (Exception ex)
                                {
                                    errorCount++;
                                    if (ex.GetBaseException().Message.Contains("correct format"))
                                    {
                                        feedBack += "Error: Record " + g.Division + " was rejected becuase the standard charge was not in the correct format."
                                                + "<br />";
                                    }
                                    else
                                    {
                                        feedBack += "Error: Record " + g.Division + " caused and error."
                                                + "<br />";
                                    }
                                }
                            }
                            feedBack += "Finished Importing " + (successCount + errorCount).ToString() +
                                " Records with " + successCount.ToString() + " inserted and " +
                                errorCount.ToString() + " rejected";
                        }
                        else
                        {
                            feedBack = "Error: You may have selected the wrong file to upload.<br /> Remember, you must have the headings 'Name' and 'Standard Charge' in the first two cells of the first row.";
                        }
                    }
                    else
                    {
                        feedBack = "Error: That file is not an Excel spreadsheet.";
                    }
                }
                else
                {
                    feedBack = "Error:  file appears to be empty";
                }
            }
            else
            {
                feedBack = "Error: No file uploaded";
            }

            TempData["Feedback"] = feedBack + "<br /><br />";

            //Note that we are assuming that you are using the Preferred Approach to Lookup Values
            //And the custom LookupsController
            return Redirect("Index");
        }

        [Authorize(Roles = "Admin,Convenor,Coach")]
        public IActionResult DownloadGamesFixtures()
        {
            var sumQ = _context.Games
                .Include(r => r.HomeTeam)
                .Include(r => r.AwayTeam)
                .Include(r => r.Division)
                .OrderBy(r => r.Division.DivName)
                .Select(r => new
                {
                    Home_Team = r.HomeTeam.TmName,
                    Visitor_Team = r.AwayTeam.TmName,
                    Game_Division = r.Division.DivName,
                    Game_Location = r.GameLocation.Name,
                    Home_Score = r.HomeTeamScore,
                    Visitor_Score = r.AwayTeamScore,
                    Game_Status = r.HasStarted,
                    Game_Duration = r.Duration,
                    Game_Innings = r.CurrentInning,
                    Game_Date = r.Summary,

                })
                .AsNoTracking();

            //How many rows?
            int numRows = sumQ.Count();

            if (numRows > 0) //We have data
            {
                //Create a new spreadsheet from scratch.
                using (ExcelPackage excel = new ExcelPackage())
                {

                    var workSheet = excel.Workbook.Worksheets.Add("Game by Division");

                    //Note: Cells[row, column]
                    workSheet.Cells[3, 1].LoadFromCollection(sumQ, true);

                    //Note: You can define a BLOCK of cells: Cells[startRow, startColumn, endRow, endColumn]
                    //Make Date and Patient Bold
                    workSheet.Cells[10, 1, numRows + 3, 1].Style.Font.Bold = true;

                    //Set Style and backgound colour of headings
                    using (ExcelRange headings = workSheet.Cells[3, 1, 3, 7])
                    {
                        headings.Style.Font.Bold = true;
                        var fill = headings.Style.Fill;
                        fill.PatternType = ExcelFillStyle.Solid;
                        fill.BackgroundColor.SetColor(Color.LightBlue);
                    }


                    //Autofit columns
                    workSheet.Cells.AutoFitColumns();
                    //Note: You can manually set width of columns as well
                    //workSheet.Column(7).Width = 10;

                    //Add a title and timestamp at the top of the report
                    workSheet.Cells[1, 1].Value = "Game Fixtures";
                    using (ExcelRange Rng = workSheet.Cells[1, 1, 1, 7])
                    {
                        Rng.Merge = true; //Merge columns start and end range
                        Rng.Style.Font.Bold = true; //Font should be bold
                        Rng.Style.Font.Size = 18;
                        Rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    }
                    //Since the time zone where the server is running can be different, adjust to 
                    //Local for us.
                    DateTime utcDate = DateTime.UtcNow;
                    TimeZoneInfo esTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
                    DateTime localDate = TimeZoneInfo.ConvertTimeFromUtc(utcDate, esTimeZone);
                    using (ExcelRange Rng = workSheet.Cells[2, 7])
                    {
                        Rng.Value = "Created: " + localDate.ToShortTimeString() + " on " +
                            localDate.ToShortDateString();
                        Rng.Style.Font.Bold = true; //Font should be bold
                        Rng.Style.Font.Size = 12;
                        Rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    }

                    //Ok, time to download the Excel

                    try
                    {
                        Byte[] theData = excel.GetAsByteArray();
                        string filename = "Upcoming Games.xlsx";
                        string mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        return File(theData, mimeType, filename);
                    }
                    catch (Exception)
                    {
                        return BadRequest("Could not build and download the file.");
                    }
                }
            }
            return NotFound("No data.");
        }


        public async Task<IActionResult> StartGame(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var game = await _context.Games
                .Include(p => p.HomeTeam).ThenInclude(p => p.Players)
                .Include(p => p.AwayTeam).ThenInclude(p => p.Players)
                .Include(g => g.HomeLineup).ThenInclude(hl => hl.PlayerLineups).ThenInclude(pl => pl.Player)
                .Include(g => g.AwayLineup).ThenInclude(al => al.PlayerLineups).ThenInclude(pl => pl.Player)
                .FirstOrDefaultAsync(m => m.ID == id);

            if (game == null)
            {
                return NotFound();
            }

            if (!await UserRolesHelper.IsAuthorizedForGame(_AppContext, User, game))
                return RedirectToAction("Index", "Games");

            var gameId = game.ID;
            var homeTeamName = game.HomeTeam.TmName;
            var awayTeamName = game.AwayTeam.TmName;
            var lineUp = game.HomeLineup.PlayerLineups.Select(pl => new PlayerScoreKeepingVM(pl.Player.FullName, pl.ID)).ToList(); ;

            var scoreKeeping = new GameScoreKeepingVM(game.ID, homeTeamName, awayTeamName, lineUp);

            return RedirectToAction("Index", "ScoreKeeping", scoreKeeping);
        }

        public async Task<IActionResult> StartAwayGame(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var game = await _context.Games
                .Include(p => p.HomeTeam).ThenInclude(p => p.Players)
                .Include(p => p.AwayTeam).ThenInclude(p => p.Players)
                .Include(g => g.HomeLineup).ThenInclude(hl => hl.PlayerLineups).ThenInclude(pl => pl.Player)
                .Include(g => g.AwayLineup).ThenInclude(al => al.PlayerLineups).ThenInclude(pl => pl.Player)
                .FirstOrDefaultAsync(m => m.ID == id);

            if (game == null)
            {
                return NotFound();
            }

            if (!await UserRolesHelper.IsAuthorizedForGame(_AppContext, User, game))
                return RedirectToAction("Index", "Games");

            var gameId = game.ID;
            var homeTeamName = game.HomeTeam.TmName;
            var awayTeamName = game.AwayTeam.TmName;
            var lineUp = game.AwayLineup.PlayerLineups.Select(pl => new PlayerScoreKeepingVM(pl.Player.FullName, pl.ID)).ToList(); ;

            var scoreKeeping = new GameScoreKeepingVM(game.ID, homeTeamName, awayTeamName, lineUp);

            return RedirectToAction("Index", "ScoreKeepingAwayTeam", scoreKeeping);
        }

        //// POST: Games/Delete
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //[Authorize(Roles = "Admin,Convenor")]
        //public async Task<IActionResult> DeleteConfirmed()
        //{
        //    // Retrieve all games from the database
        //    var games = await _context.Games.ToListAsync();

        //    // Check if there are any games to delete
        //    if (games == null || games.Count == 0)
        //    {
        //        return NotFound();
        //    }

        //    // Delete all games
        //    _context.Games.RemoveRange(games);
        //    await _context.SaveChangesAsync();

        //    TempData["SuccessMessage"] = "All games have been deleted successfully.";

        //    // Redirect to an appropriate action after deleting all games
        //    return RedirectToAction("Index", "Games");
        //}

        //// GET: Games/Delete
        //[Authorize(Roles = "Admin,Convenor")]
        //public async Task<IActionResult> Delete()
        //{
        //    // Retrieve all games from the database
        //    var games = await _context.Games.ToListAsync();

        //    // Check if there are any games to delete
        //    if (games == null || games.Count == 0)
        //    {
        //        return NotFound();
        //    }

        //    // Delete all games
        //    _context.Games.RemoveRange(games);
        //    await _context.SaveChangesAsync();

        //    TempData["SuccessMessage"] = "All games have been deleted successfully.";

        //    // Redirect to an appropriate action after deleting all games
        //    return RedirectToAction("Index", "Games");
        //}


        // GET: Games/Delete
        [Authorize(Roles = "Admin,Convenor")]
        public async Task<IActionResult> Delete()
        {
            try
            {
                // Retrieve all games from the database
                var allGames = await _context.Games.ToListAsync();

                if (allGames != null && allGames.Count > 0)
                {
                    // Generate Excel file asynchronously
                    byte[] theData = await GenerateExcelFileAsync(allGames); // Updated method name
                    string filename = "Deleted Games.xlsx";
                    string mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                    // Delete the games and set success message
                    await DeleteGamesAndShowMessage(allGames);

                    // Return the Excel file for download
                    var fileContentResult = File(theData, mimeType, filename);
                    TempData["SuccessMessage"] = "All games have been downloaded, click again to delete.";
                    // Return the file content result for download
                    return fileContentResult;
                   
                }
                else
                {
                    TempData["SuccessMessage"] = "All games have been deleted  and downloaded successfully.";
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions and log them
                TempData["ErrorMessage"] = "An error occurred while processing your request.";
            }

            // Redirect to appropriate page if download or deletion fails
            return RedirectToAction("Index", "Games");
        }



        private async Task<byte[]> GenerateExcelFileAsync(List<Game> games)
        {
            var sumQ = _context.Games
                .Include(r => r.HomeTeam)
                .Include(r => r.AwayTeam)
                .Include(r => r.Division)
                .OrderBy(r => r.Division.DivName)
                .Select(r => new
                {
                    Home_Team = r.HomeTeam.TmName,
                    Visitor_Team = r.AwayTeam.TmName,
                    Game_Division = r.Division.DivName,
                    Game_Location = r.GameLocation.Name,
                    Home_Score = r.HomeTeamScore,
                    Visitor_Score = r.AwayTeamScore,
                    Game_Status = r.HasStarted,
                    Game_Duration = r.Duration,
                    Game_Innings = r.CurrentInning,
                    Game_Date = r.Summary,
                })
                .AsNoTracking();

            // How many rows?
            int numRows = await sumQ.CountAsync();

            if (numRows > 0)
            {
                using (ExcelPackage excel = new ExcelPackage())
                {
                    var workSheet = excel.Workbook.Worksheets.Add("Game by Division");

                    workSheet.Cells[3, 1].LoadFromCollection(sumQ, true);

                    workSheet.Cells[10, 1, numRows + 3, 1].Style.Font.Bold = true;

                    using (ExcelRange headings = workSheet.Cells[3, 1, 3, 7])
                    {
                        headings.Style.Font.Bold = true;
                        var fill = headings.Style.Fill;
                        fill.PatternType = ExcelFillStyle.Solid;
                        fill.BackgroundColor.SetColor(Color.LightBlue);
                    }

                    workSheet.Cells.AutoFitColumns();

                    workSheet.Cells[1, 1].Value = "Game Fixtures";
                    using (ExcelRange Rng = workSheet.Cells[1, 1, 1, 7])
                    {
                        Rng.Merge = true;
                        Rng.Style.Font.Bold = true;
                        Rng.Style.Font.Size = 18;
                        Rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    }

                    DateTime utcDate = DateTime.UtcNow;
                    TimeZoneInfo esTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
                    DateTime localDate = TimeZoneInfo.ConvertTimeFromUtc(utcDate, esTimeZone);
                    using (ExcelRange Rng = workSheet.Cells[2, 7])
                    {
                        Rng.Value = "Created: " + localDate.ToShortTimeString() + " on " +
                            localDate.ToShortDateString();
                        Rng.Style.Font.Bold = true;
                        Rng.Style.Font.Size = 12;
                        Rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    }

                    byte[] theData = excel.GetAsByteArray();
                    return theData;
                }
            }

            // If no rows, return null or handle as needed
            return null;
        }


        private async Task DeleteGamesAndShowMessage(List<Game> games)
        {
            // Delete the games
            _context.Games.RemoveRange(games);
            await _context.SaveChangesAsync();

            // Show success message
            TempData["SuccessMessage"] = "All games have been deleted successfully.";
        
        }


        private bool GameExists(int id)
        {
            return _context.Games.Any(e => e.ID == id);
        }
    }
}
