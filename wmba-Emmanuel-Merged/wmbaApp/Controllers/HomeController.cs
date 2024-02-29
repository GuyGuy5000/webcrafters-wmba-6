using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System.Collections.ObjectModel;
using System.Diagnostics;
using wmbaApp.Data;
using wmbaApp.Models;
using wmbaApp.Utilities;
using wmbaApp.ViewModels;

namespace wmbaApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly WmbaContext _context;

        public HomeController(ILogger<HomeController> logger, WmbaContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult UnderConstruction()
        {
            return View();
        }

        //public IActionResult Index()
        //{
        //    var teams = _context.Teams
        //      .Include(t => t.GameTeams).ThenInclude(t => t.Game)
        //      .AsNoTracking();
        //    ViewData["Matchups"] = GameMatchup.GetMatchups(_context, teams.ToArray());

        //    return View(teams);
        //}

        public IActionResult Index()
        {
            // trying to get all Games
            var games = _context.Games
                .Include(p => p.GameTeams).ThenInclude(p => p.Team)
                .Include(p => p.AwayTeam)
                .Include(p => p.HomeTeam)
                .Include(p => p.HomeLineup)
                .Include(p => p.AwayLineup)
                .AsNoTracking()
                .ToList();

            // add infos to the List<GameIndexVM>
            var viewModelList = games.Select(game => new GameIndexVM
            {
                FullVersus = game.FullVersus,
                Summary = game.Summary
            }).ToList();

            // Pass the viewModelList to the view
            return View(viewModelList);
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        /// <summary>
        /// Allows importing teams into the db using a csv or excel file
        /// </summary>
        /// <param name="theFile"></param>
        /// <returns></returns>
        public async Task<IActionResult> ImportTeams(IFormFile theFile)
        {
            var games = _context.Games
                .Include(p => p.GameTeams).ThenInclude(p => p.Team)
                .Include(p => p.AwayTeam)
                .Include(p => p.HomeTeam)
                .AsNoTracking()
                .ToList();
            //ViewData["Matchups"] = GameMatchup.GetMatchups(_context, teams.ToArray());

            // add infos to the List<GameIndexVM>
            var viewModelList = games.Select(game => new GameIndexVM
            {
                FullVersus = game.FullVersus,
                Summary = game.Summary
            }).ToList();

            if (theFile == null)
            {
                ModelState.AddModelError("", "Upload a file to continue.");
                return View("Index", viewModelList);
            }

            if (theFile.ContentType == "text/csv")
            {
                ValidateCSVFile(theFile);


            }
            else if (theFile.ContentType.Contains("excel") || theFile.ContentType.Contains("spreadsheet"))
            {
                ExcelPackage excel;
                using (var memoryStream = new MemoryStream())
                {
                    await theFile.CopyToAsync(memoryStream);
                    excel = new ExcelPackage(memoryStream);
                }
                ValidateExcelFile(excel);
                ImportTeamExcel(excel);
                ImportPlayerExcel(excel);

            }
            else
            {
                ModelState.AddModelError("", "File is the wrong type. Only .CSV files are allowed");
            }
            if (!ModelState.IsValid) //if there were validation errors return to view
                return View("Index", viewModelList);

            return View("Index", viewModelList);
        }


        #region ImportMethods
        private async void ValidateCSVFile(IFormFile theFile)
        {
            try
            {
                string mimeType = theFile.ContentType;
                long fileLength = theFile.Length;

                if (!(mimeType == "" || fileLength == 0)) //if file exists
                {
                    if (mimeType == "text/csv") //if it is a csv
                    {
                        using var memoryStream = new MemoryStream();
                        await theFile.CopyToAsync(memoryStream);
                        StreamReader reader = new(memoryStream);
                        reader.BaseStream.Position = 0;

                        string firstLine = null;
                        int counter = 0;
                        while (firstLine == null)
                        {
                            firstLine = await reader.ReadLineAsync();
                            counter++;
                            if (counter > 5000)
                            {
                                ModelState.AddModelError("", "Cannot upload an empty file");
                                return;
                            }
                        }
                        if (firstLine == "ID,First Name,Last Name,Member ID,Season,Division,Club,Team")
                        {
                            return;
                        }
                        else //first line isn't formatted correctly
                            ModelState.AddModelError("", "File contents are in the wrong format. Please ensure the headers are on the first line, and match the example");
                    }
                    else
                        ModelState.AddModelError("", "File is the wrong type. Only .CSV files are allowed");
                }
                else //empty file
                    ModelState.AddModelError("", "Cannot upload an empty file");
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "An unknown error occured.  Try again, and if the problem persists see your system administrator.");
            }
        }

        private void ValidateExcelFile(ExcelPackage excel)
        {

            var workSheet = excel.Workbook.Worksheets[0];

            //if the first line is in the wrong format
            if (!(workSheet.Cells[1, 1].Text == "ID" &&
                            workSheet.Cells[1, 2].Text == "First Name" &&
                            workSheet.Cells[1, 3].Text == "Last Name" &&
                            workSheet.Cells[1, 4].Text == "Member ID" &&
                            workSheet.Cells[1, 5].Text == "Season" &&
                            workSheet.Cells[1, 6].Text == "Division" &&
                            workSheet.Cells[1, 7].Text == "Club" &&
                            workSheet.Cells[1, 8].Text == "Team"))
            {
                //check which headers are wrong
                if (workSheet.Cells[1, 1].Text != "ID")
                    ModelState.AddModelError("", "Cell 1A is reserved for the 'ID' header. Please correct this cell's content and try again.");
                if (workSheet.Cells[1, 2].Text != "First Name")
                    ModelState.AddModelError("", "Cell 1B is reserved for the 'First Name' header. Please correct this cell's content and try again.");
                if (workSheet.Cells[1, 3].Text != "Last Name")
                    ModelState.AddModelError("", "Cell 1C is reserved for the 'Last Name' header. Please correct this cell's content and try again.");
                if (workSheet.Cells[1, 4].Text != "Member ID")
                    ModelState.AddModelError("", "Cell 1D is reserved for the 'Member ID' header. Please correct this cell's content and try again.");
                if (workSheet.Cells[1, 5].Text != "Season")
                    ModelState.AddModelError("", "Cell 1E is reserved for the 'Season' header. Please correct this cell's content and try again.");
                if (workSheet.Cells[1, 6].Text != "Division")
                    ModelState.AddModelError("", "Cell 1F is reserved for the 'Division' header. Please correct this cell's content and try again.");
                if (workSheet.Cells[1, 7].Text != "Club")
                    ModelState.AddModelError("", "Cell 1G is reserved for the 'Club' header. Please correct this cell's content and try again.");
                if (workSheet.Cells[1, 8].Text != "Team")
                    ModelState.AddModelError("", "Cell 1H is reserved for the 'Team' header. Please correct this cell's content and try again.");
            }

            if (String.IsNullOrEmpty(workSheet.Cells[2, 1].Text))
                ModelState.AddModelError("", "No data detected. Please ensure that the data you want to import starts at Cell 2A of the spreadsheet");

        }
        private void ImportTeamExcel(ExcelPackage excel)
        {
            Collection<string> importedTeams = new();
            string feedBack = "";
            var workSheet = excel.Workbook.Worksheets[0];
            var start = workSheet.Dimension.Start;
            var end = workSheet.Dimension.End;
            int successCount = 0;
            int errorCount = 0;

            for (int row = start.Row + 1; row <= end.Row; row++)
            {
                Team t = new();

                if (!importedTeams.Contains(workSheet.Cells[row, 8].Text)) //check to see if a team was already imported
                    try
                    {
                        int? divID = _context.Divisions.FirstOrDefault(d => d.DivName == workSheet.Cells[row, 6].Text.ToUpper())?.ID;

                        // Row by row...
                        t.TmName = workSheet.Cells[row, 8].Text;
                        t.DivisionID = (int)divID;
                        _context.Teams.Add(t);
                        _context.SaveChanges();
                        successCount++;
                        importedTeams.Add(t.TmName);
                    }
                    catch (DbUpdateException dex)
                    {
                        errorCount++;
                        if (dex.GetBaseException().Message.Contains("UNIQUE constraint failed"))
                        {
                            feedBack += "Error: Record " + t.TmName + " was rejected as a duplicate."
                                    + "<br />";
                        }
                        else
                        {
                            feedBack += "Error: Record " + t.TmName + " caused an error."
                                    + "<br />";
                        }
                        //Here is the trick to using SaveChanges in a loop.  You must remove the 
                        //offending object from the cue or it will keep raising the same error.
                        _context.Remove(t);
                    }
                    catch (Exception ex)
                    {
                        errorCount++;
                        if (ex.GetBaseException().Message.Contains("correct format"))
                        {
                            feedBack += "Error: Record " + t.TmName + " was rejected becuase the standard charge was not in the correct format."
                                    + "<br />";
                        }
                        else
                        {
                            feedBack += "Error: Record " + t.TmName + " caused and error."
                                    + "<br />";
                        }
                    }
            }
            feedBack += "Finished Importing " + (successCount + errorCount).ToString() +
                        " Records with " + successCount.ToString() + " inserted and " +
                        errorCount.ToString() + " rejected";
            TempData["TeamImportFeedback"] = feedBack;
        }
        private void ImportPlayerExcel(ExcelPackage excel)
        {
            string feedBack = "";
            var workSheet = excel.Workbook.Worksheets[0];
            var start = workSheet.Dimension.Start;
            var end = workSheet.Dimension.End;
            int successCount = 0;
            int errorCount = 0;

            for (int row = start.Row + 1; row <= end.Row; row++)
            {
                Player p = new();
                try
                {
                    int? teamID = _context.Teams.FirstOrDefault(t => t.TmName == workSheet.Cells[row, 8].Text)?.ID;

                    // Row by row...
                    p.PlyrFirstName = workSheet.Cells[row, 2].Text;
                    p.PlyrLastName = workSheet.Cells[row, 3].Text;
                    p.PlyrMemberID = workSheet.Cells[row, 4].Text;
                    p.TeamID = (int)teamID;
                    _context.Players.Add(p);
                    _context.SaveChanges();
                    successCount++;
                }
                catch (DbUpdateException dex)
                {
                    errorCount++;
                    if (dex.GetBaseException().Message.Contains("UNIQUE constraint failed"))
                    {
                        feedBack += "Error: Record " + p.PlyrMemberID + " was rejected as a duplicate."
                                + "<br />";
                    }
                    else
                    {
                        feedBack += "Error: Record " + p.PlyrMemberID + " caused an error."
                                + "<br />";
                    }
                    //Here is the trick to using SaveChanges in a loop.  You must remove the 
                    //offending object from the cue or it will keep raising the same error.
                    _context.Remove(p);
                }
                catch (Exception ex)
                {
                    errorCount++;
                    if (ex.GetBaseException().Message.Contains("correct format"))
                    {
                        feedBack += "Error: Record " + p.PlyrMemberID + " was rejected becuase the standard charge was not in the correct format."
                                + "<br />";
                    }
                    else
                    {
                        feedBack += "Error: Record " + p.PlyrMemberID + " caused and error."
                                + "<br />";
                    }
                }
            }
            feedBack += "Finished Importing " + (successCount + errorCount).ToString() +
                        " Records with " + successCount.ToString() + " inserted and " +
                        errorCount.ToString() + " rejected";
            TempData["PlayerImportFeedback"] = feedBack;
        }

        #endregion
    }
}