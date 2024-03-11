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
using wmbaApp.ViewModels;
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
            string[] sortOptions = new[] { "Teamss", "Location" };

PopulateDropDownLists();

           var games = _context.Games
                .Include(g => g.GameLocation)
                .Include(g => g.AwayTeam).ThenInclude(p => p.Division)
                .Include(g => g.HomeTeam).ThenInclude(p => p.Division)
                .Include(g => g.HomeLineup).ThenInclude(hl => hl.PlayerLineups).ThenInclude(pl => pl.Player)
                .Include(g => g.AwayLineup).ThenInclude(hl => hl.PlayerLineups).ThenInclude(pl => pl.Player)
                .AsNoTracking();


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
        public async Task<IActionResult> Create([Bind("ID,GameStartTime,GameEndTime,IsActive,GameLocationID,HomeTeamID,AwayTeamID,DivisionID")] Game game,
            int? selectedDivision, IFormFile theExcel)
        {
            if (selectedDivision.HasValue)
            {
                ViewData["HomeTeamID"] = new SelectList(_context.Teams.Where(t => t.DivisionID == selectedDivision), "ID", "TmName");
                ViewData["AwayTeamID"] = new SelectList(_context.Teams.Where(t => t.DivisionID == selectedDivision), "ID", "TmName");
            }

            if (ModelState.IsValid)
            {

                _context.Add(game);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", new { game.ID });
            }
            //ViewData["AwayTeamID"] = new SelectList(_context.Teams, "ID", "TmName", game.AwayTeamID);
            //ViewData["HomeTeamID"] = new SelectList(_context.Teams, "ID", "TmName", game.HomeTeamID);
            PopulateDropDownLists();
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

            if (await TryUpdateModelAsync<Game>(gameToUpdate, "",
                t => t.GameStartTime, t => t.GameEndTime, t => t.GameLocationID, t => t.DivisionID,
                t => t.AwayTeamID, t => t.HomeTeamID))
            {

                try
                {
                    _context.Update(gameToUpdate);
                    await _context.SaveChangesAsync();
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
            }
            ViewData["HomeLineupID"] = new SelectList(_context.Lineups, "ID", "ID", gameToUpdate.HomeLineupID);
            ViewData["AwayLineupID"] = new SelectList(_context.Lineups, "ID", "ID", gameToUpdate.AwayLineupID);
            PopulateDropDownLists(gameToUpdate);
            return View(gameToUpdate);
        }

        // GET: Games/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
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


            return View(game);
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
                .Include(g => g.GameLocation)
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

            var gameId = game.ID;
            var homeTeamName = game.HomeTeam.TmName;
            var awayTeamName = game.AwayTeam.TmName;
            var lineUp = game.HomeLineup.PlayerLineups.Select(pl => new PlayerScoreKeepingVM(pl.Player.FullName, pl.ID)).ToList(); ;

            var scoreKeeping = new GameScoreKeepingVM(game.ID, homeTeamName, awayTeamName, lineUp);

            return RedirectToAction("Index", "ScoreKeeping", scoreKeeping);
        }

        private bool GameExists(int id)
        {
            return _context.Games.Any(e => e.ID == id);
        }
    }
}
