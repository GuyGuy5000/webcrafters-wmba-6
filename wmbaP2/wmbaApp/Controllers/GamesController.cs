/// <summary>
/// Game
/// Farooq Jidelola
/// </summary>
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using OfficeOpenXml.Table;
using wmbaApp.CustomControllers;
using wmbaApp.Data;
using wmbaApp.Models;
using wmbaApp.Utilities;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace wmbaApp.Controllers
{
    public class GamesController : ElephantController
    {
        private readonly WmbaContext _context;

        public GamesController(WmbaContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string SearchString, int? TeamID,
                    int? page, int? pageSizeID, string actionButton, string sortDirection = "asc", string sortField = "")
        {
            // Count the number of filters applied - start by assuming no filters
            ViewData["Filtering"] = "btn-outline-secondary";
            int numberFilters = 0;
            // Then in each "test" for filtering, add to the count of Filters applied

            // List of sort options.
            // NOTE: make sure this array has matching values to the column headings
            string[] sortOptions = new[] { "Games", "Game-Location" };



            var games = _context.Games
                .Include(g => g.AwayTeam).ThenInclude(p => p.Division)
                .Include(g => g.HomeTeam).ThenInclude(p => p.Division)
                .AsNoTracking();


            if (!System.String.IsNullOrEmpty(SearchString))
            {
                games = games.Where(g => g.GameLocation.ToUpper().Contains(SearchString.ToUpper())
                || g.HomeTeam.TmName.ToUpper().Contains(SearchString.ToUpper())
                || g.AwayTeam.TmName.ToUpper().Contains(SearchString.ToUpper())
                || g.Division.DivName.ToUpper().Contains(SearchString.ToUpper()));

                numberFilters++;
            }



            // Give feedback about the state of the filters
            if (numberFilters != 0)
            {
                // Toggle the Open/Closed state of the collapse depending on if we are filtering
                ViewData["Filtering"] = " btn-danger";
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
            if (sortField == "Games")
            {
                games = sortDirection == "asc" ? games.OrderBy(g => g.HomeTeam) : games.OrderByDescending(g => g.HomeTeam);
            }
            else if (sortField == "Game-Location")
            {
                games = sortDirection == "asc" ? games.OrderBy(g => g.GameLocation) : games.OrderByDescending(g => g.GameLocation);
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

        // GET: Games/Details/5
        public async Task<IActionResult> Details(int? id)
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

            //getting my new lineup created by any user
            //var lineup = await _context.Lineups
            //    .Include(l => l.PlayerLineups).ThenInclude(pl => pl.Player)
            //    .Include(l => l.HomeGames).ThenInclude(g => g.HomeTeam.Players)
            //    .Include(l => l.AwayGames).ThenInclude(g => g.AwayTeam.Players)
            //    .FirstOrDefaultAsync(m => m.ID == id);

            //ViewData["SelectedHomePlayers"] = lineup.HomeGames?.FirstOrDefault()?.HomeTeam?.Players
            //    .Where(p => lineup.PlayerLineups.Any(pl => pl.PlayerID == p.ID))
            //    .ToList();

            //ViewData["SelectedAwayPlayers"] = lineup.AwayGames?.FirstOrDefault()?.AwayTeam?.Players
            //    .Where(p => lineup.PlayerLineups.Any(pl => pl.PlayerID == p.ID))
            //    .ToList();

            return View(game);
        }



        // GET: Games/Create
        public IActionResult Create()
        {
            ViewData["AwayTeamID"] = new SelectList(_context.Teams, "ID", "TmName");
            ViewData["HomeTeamID"] = new SelectList(_context.Teams, "ID", "TmName");
            ViewData["DivisionID"] = new SelectList(_context.Divisions, "ID", "DivName");
            ViewData["GameLocations"] = new SelectList(_context.Games.Select(p => p.GameLocation).Distinct().ToList());
            return View();
        }

        // POST: Games/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,GameStartTime,GameEndTime,IsActive,GameLocation,HomeTeamID,AwayTeamID,DivisionID")] Game game,
            string newGameLocation, int? selectedDivision, IFormFile theExcel)
        {
            if (selectedDivision.HasValue)
            {
                ViewData["HomeTeamID"] = new SelectList(_context.Teams.Where(t => t.DivisionID == selectedDivision), "ID", "TmName");
                ViewData["AwayTeamID"] = new SelectList(_context.Teams.Where(t => t.DivisionID == selectedDivision), "ID", "TmName");
            }

            //i'm trying to avoid usingw same Gamelocation be used twice for different games same day.
            if (game.GameStartTime.HasValue && IsGameLocationUsedOnceADay(game.GameStartTime.Value.Date, game.GameLocation))
            {
                ModelState.AddModelError("GameLocation", "Game location can only be used once a day.");
            }

            if (theExcel != null)
            {
                await InsertFromExcel(theExcel);
            }


            if (ModelState.IsValid)
            {
                if (game.GameLocation == "Add New" && !string.IsNullOrWhiteSpace(newGameLocation))
                {
                    game.GameLocation = newGameLocation;
                }

                _context.Add(game);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            //ViewData["AwayTeamID"] = new SelectList(_context.Teams, "ID", "TmName", game.AwayTeamID);
            //ViewData["HomeTeamID"] = new SelectList(_context.Teams, "ID", "TmName", game.HomeTeamID);
            ViewData["DivisionID"] = new SelectList(_context.Divisions, "ID", "DivName");
            ViewData["GameLocations"] = new SelectList(_context.Games.Select(p => p.GameLocation).Distinct().ToList());
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
                .Include(p=>p.HomeTeam)
                .Include(p=>p.AwayTeam)
                .FirstOrDefaultAsync(m => m.ID == id);

            if (game == null)
            {
                return NotFound();
            }


            ViewData["AwayTeamID"] = new SelectList(_context.Teams, "ID", "TmName", game.AwayTeamID);
            ViewData["HomeTeamID"] = new SelectList(_context.Teams, "ID", "TmName", game.HomeTeamID);
            ViewData["DivisionID"] = new SelectList(_context.Divisions, "ID", "DivName");
            ViewData["GameLocations"] = new SelectList(_context.Games.Select(p => p.GameLocation).Distinct().ToList());
            return View(game);
        }

        // POST: Games/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,GameStartTime,GameEndTime,IsActive,GameLocation,HomeTeamID,AwayTeamID,DivisionID")] Game game,
                string newGameLocation)
        {
            var gameToUpdate = await _context.Games
                .FirstOrDefaultAsync(m => m.ID == id);

            if (gameToUpdate == null)
            {
                return NotFound();
            }

            if (await TryUpdateModelAsync(gameToUpdate, "", t => t.GameEndTime, t => t.GameLocation, t => t.DivisionID,
                t => t.AwayTeamID, t => t.HomeTeamID))
            {
                try
                {
                    if (game.GameLocation == "Add New" && !string.IsNullOrWhiteSpace(newGameLocation))
                    {
                        gameToUpdate.GameLocation = newGameLocation;
                    }

                    await _context.SaveChangesAsync();
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
                catch (DbUpdateException dex)
                {
                    // Handle UNIQUE constraint violations for Game entity
                    if (dex.InnerException.Message.Contains("UNIQUE"))
                    {
                        ModelState.AddModelError("", "A game with similar attributes already exists. Please provide unique attributes.");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                    }
                    return View(gameToUpdate);
                }
            }

            ViewData["DivisionID"] = new SelectList(_context.Divisions, "ID", "DivName");
            ViewData["GameLocations"] = new SelectList(_context.Games.Select(p => p.GameLocation).Distinct().ToList());

            return RedirectToAction("Details", new { id = gameToUpdate.ID });
        }


        // GET: Games/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Games == null)
            {
                return NotFound();
            }

            var game = await _context.Games
                .Include(g => g.AwayTeam)
                .Include(g => g.HomeTeam)
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
                return Problem("Entity set 'WmbaContext.Games'  is null.");
            }
            var game = await _context.Games.FindAsync(id);
            if (game != null)
            {
                _context.Games.Remove(game);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult SelectLineup(int gameId)
        {
            return RedirectToAction("Create", "Lineups", new { gameId });
        }

        private bool IsGameLocationUsedOnceADay(DateTime gameDate, string gameLocation)
        {
            return _context.Games.Any(g => g.GameStartTime == gameDate && g.GameLocation == gameLocation);
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
            string feedback = string.Empty;

            if (theExcel != null)
            {
                string mimeType = theExcel.ContentType;
                long fileLength = theExcel.Length;

                if (!(mimeType == "" || fileLength == 0))
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

                        // Calling the method to accept and run my excel file
                        await ReadImportedGameData(workSheet, feedback);
                    }
                    else if (mimeType.Contains("text/csv"))
                    {
                        var format = new ExcelTextFormat();
                        format.Delimiter = ',';
                        bool firstRowIsHeader = true;

                        using var reader = new System.IO.StreamReader(theExcel.OpenReadStream());

                        using ExcelPackage package = new ExcelPackage();
                        var result = reader.ReadToEnd();
                        ExcelWorksheet workSheet = package.Workbook.Worksheets.Add("Imported Report Data");

                        workSheet.Cells["A1"].LoadFromText(result, format, TableStyles.None, firstRowIsHeader);

                        // Call the method to read imported data and process it
                        await ReadImportedGameData(workSheet, feedback);
                    }
                    else
                    {
                        feedback = "Error: That file is not an Excel spreadsheet.";
                    }
                }
                else
                {
                    feedback = "Error: File appears to be empty";
                }
            }
            else
            {
                feedback = "Error: No file uploaded";
            }

            TempData["Feedback"] = feedback + "<br /><br />";
            return RedirectToAction("Index");

        }

        private async Task ReadImportedGameData(ExcelWorksheet workSheet, string feedback)
        {
            // Prepare the collection of imported data
          //  List<Game> games = new List<Game>();

            var start = workSheet.Dimension.Start;
            var end = workSheet.Dimension.End;
            int successCount = 0;
            int errorCount = 0;
            feedback = "";

            if (workSheet.Cells[1, 1].Text == "Division" &&
                workSheet.Cells[1, 2].Text == "Home Team" &&
                workSheet.Cells[1, 3].Text == "Away Team" &&
                workSheet.Cells[1, 4].Text == "Game Starts" &&
                workSheet.Cells[1, 5].Text == "Game Ends" &&
                workSheet.Cells[1, 6].Text == "Game Location")
            {
                for (int row = start.Row + 1; row <= end.Row; row++)
                {
                    try
                    {
                        Division division = await _context.Divisions.SingleOrDefaultAsync(d => d.DivName == workSheet.Cells[row, 1].Text);
                        Team homeTeam = await _context.Teams.SingleOrDefaultAsync(t => t.TmName == workSheet.Cells[row, 2].Text);
                        Team awayTeam = await _context.Teams.SingleOrDefaultAsync(t => t.TmName == workSheet.Cells[row, 3].Text);

                        if (division != null && homeTeam != null && awayTeam != null &&
                                    DateTime.TryParse(workSheet.Cells[row, 4].Text, out DateTime gameStartTime) &&
                                    DateTime.TryParse(workSheet.Cells[row, 5].Text, out DateTime gameEndTime))
                        {
                            Game gameData = new Game();
                            {
                                gameData.Division = division;
                                gameData.HomeTeam = homeTeam;
                                gameData.AwayTeam = awayTeam;
                                gameData.GameStartTime = gameStartTime;
                                gameData.GameEndTime = gameEndTime;
                                gameData.GameLocation = HandleGameLocation(workSheet.Cells[row, 6].Text);

                                _context.Games.Add(gameData);
                                await _context.SaveChangesAsync();
                                successCount++;
                            };
                        }
                        else
                        {
                            if (division == null)
                                feedback += $"Division '{workSheet.Cells[row, 1].Text}' not found. ";

                            if (homeTeam == null)
                                feedback += $"Home Team '{workSheet.Cells[row, 2].Text}' not found. ";

                            feedback += "<br />";


                            errorCount++;
                            feedback += $"Error: Record {workSheet.Cells[row, 3].Text} - Away Team not found in the database.<br />";

                        }
                    }
                    catch (Exception ex)
                    {
                        errorCount++;
                        if (ex.GetBaseException().Message.Contains("correct format"))
                        {
                            feedback += "Error: Record " + workSheet.Cells[row, 4].Text + " was rejected because the Start Time was not in the correct format."
                                    + "<br />";
                        }
                        else
                        {
                            feedback += "Error: Record " + workSheet.Cells[row, 4].Text + " caused an error."
                                    + "<br />";
                        }
                    }
                }
                feedback += "Finished Importing " + (successCount + errorCount).ToString() +
                    " Records with " + successCount.ToString() + " inserted and " +
                    errorCount.ToString() + " rejected";
            }
            else
            {
                feedback = "Error: You may have selected the wrong file to upload.<br /> Remember, you must have the headings 'Division', 'Home Team', 'Away Team', 'Game Starts', 'Game Ends', and 'Game Location' in the first row.";
            }
        }

        private string HandleGameLocation(string inputLocation)
        {
            // Check if the location already exists in the database
            var existingLocation = _context.Games.FirstOrDefault(gl => gl.GameLocation == inputLocation);

            if (existingLocation != null)
            {
                // location is in my db send the user back
                return existingLocation.GameLocation;
            }
            else
            {
                return inputLocation;
            }
        }

        public IActionResult DownloadGamesFixtures()
        {
            var sumQ = _context.Games
                .Include(r => r.HomeTeam)
                .Include(r => r.AwayTeam)
                .OrderBy(r => r.Division.DivName)
                .Select(r => new
                {
                    Games = r.FullVersus,
                    Game_Details = r.Summary,
                    Game_Division = r.Division.DivName,
                    Game_Location = r.GameLocation,
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
                    workSheet.Cells[4, 1, numRows + 3, 1].Style.Font.Bold = true;

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

        private bool GameExists(int id)
        {
          return _context.Games.Any(e => e.ID == id);
        }
    }
}
