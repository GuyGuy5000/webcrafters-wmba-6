using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;
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
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly WmbaContext _context;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public HomeController(ILogger<HomeController> logger, WmbaContext context, RoleManager<ApplicationRole> roleManager)
        {
            _logger = logger;
            _context = context;
            this._roleManager = roleManager;

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
                StartTimeSummary = game.StartTimeSummary
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
            var teams = _context.Teams
                  .Include(t => t.GameTeams).ThenInclude(t => t.Game)
                  .AsNoTracking();

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
                StartTimeSummary = game.StartTimeSummary
            }).ToList();

            if (theFile == null)
            {
                ModelState.AddModelError("", "Choose a file to continue.");
                return View("ImportTeams");
            }

            //if the file format is csv
            if (theFile.ContentType == "text/csv")
            {
                ValidateCSVFile(theFile);
                if (!ModelState.IsValid) //if there were validation errors return to index
                    return View("ImportTeams");
                using var memoryStream = new MemoryStream();
                await theFile.CopyToAsync(memoryStream);
                StreamReader reader = new(memoryStream);
                reader.BaseStream.Position = 0;
                ImportTeamCSV(reader);
                reader.BaseStream.Position = 0;
                ImportPlayerCSV(reader);

            }
            //if the file format is excel
            else if (theFile.ContentType.Contains("excel") || theFile.ContentType.Contains("spreadsheet"))
            {
                ExcelPackage excel;
                using (var memoryStream = new MemoryStream())
                {
                    await theFile.CopyToAsync(memoryStream);
                    excel = new ExcelPackage(memoryStream);
                }
                ValidateExcelFile(excel);
                if (!ModelState.IsValid) //if there were validation errors return to index
                    return View("ImportTeams");
                ImportTeamExcel(excel);
                ImportPlayerExcel(excel);

            }
            else
            {
                ModelState.AddModelError("", "File is the wrong type. Only excel and CSV files are allowed");
            }
            if (!ModelState.IsValid) //if there were validation errors return to index
                return View("ImportTeams");

            return View("ImportTeams");
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

                        //retrieve the first line
                        while (firstLine == null)
                        {
                            firstLine = await reader.ReadLineAsync(); //attempt to get the first line
                            counter++;
                            if (counter > 5000) //if fails too many times the file is empty
                            {
                                ModelState.AddModelError("", "Cannot upload an empty file");
                                return;
                            }
                        }
                        if (firstLine == "ID,First Name,Last Name,Member ID,Season,Division,Club,Team") //if headers are in the correct format
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
        private async void ImportTeamCSV(StreamReader reader)
        {
            List<string> importedTeams = new(); //used to keep track of which teams were attempted to be imported.
            List<Team> successfullyImportedTeams = new();
            List<Team> failedImportedTeams = new();
            string feedBack = "";
            string[] headers = reader.ReadLine().Split(","); //order of the headers: [0]ID,[1]First Name,[2]Last Name,[3]Member ID,[4]Season,[5]Division,[6]Club,[7]Team
            var lines = reader.ReadToEnd().Split("\n"); //get all lines after the header
            int successCount = 0;
            int errorCount = 0;

            foreach (string line in lines)
            {
                Team t = new();

                string[] data = line.Split(','); //split the line into the individual components
                if (data.Length == 1) //if length is one, it reached the end of the file 
                    break;

                string teamName = data[7].Trim(); //check to see if the team name contains a division
                if (data[7].Contains("U "))
                    teamName = teamName.Split("U ")[1]; //if yes, split and get everything after the division

                if (!importedTeams.Contains(teamName)) //check to see if a team was already imported
                    try
                    {
                        int? divID = _context.Divisions.FirstOrDefault(d => d.DivName == data[5].ToUpper())?.ID;

                        // Row by row...
                        t.TmName = teamName;
                        t.DivisionID = (int)divID;
                        importedTeams.Add(t.TmName);
                        _context.Teams.Add(t);
                        _context.SaveChanges();
                        successCount++;
                        successfullyImportedTeams.Add(t);
                        IdentityResult roleResult;
                        roleResult = await _roleManager.CreateAsync(new ApplicationRole(t.TmName, 0, t.ID));


                    }
                    catch (DbUpdateException dex)
                    {
                        errorCount++;
                        if (dex.GetBaseException().Message.Contains("UNIQUE constraint failed"))
                        {
                            successfullyImportedTeams.Add(t);
                            failedImportedTeams.Add(t);
                        }
                        else
                        {
                            failedImportedTeams.Add(t);
                        }
                        _context.Remove(t);
                    }
                    catch (Exception ex)
                    {
                        errorCount++;
                        failedImportedTeams.Add(t);
                        _context.Remove(t);
                    }
            }

            feedBack += "Finished Importing " + (successCount + errorCount).ToString() +
                        " Teams with " + successCount.ToString() + " inserted and " +
                        errorCount.ToString() + " rejected";
            ViewData["SuccessfullyImportedTeams"] = successfullyImportedTeams;
            ViewData["FailedImportedTeams"] = failedImportedTeams;
            TempData["TeamImportFeedback"] = feedBack;
        }
        private void ImportPlayerCSV(StreamReader reader)
        {
            List<Player> succesfullyImportedPlayers = new();
            List<Player> failedImportedPlayers = new();
            string feedBack = "";
            string[] headers = reader.ReadLine().Split(",");
            var lines = reader.ReadToEnd().Split("\n");
            int successCount = 0;
            int errorCount = 0;

            foreach (string line in lines)
            {
                Player p = new();

                string[] data = line.Split(',');
                if (data.Length == 1)
                    break;

                string teamName = data[7]; //check to see if the team name contains a division
                if (data[7].Contains("U "))
                    teamName = teamName.Split("U ")[1].Trim(); //if yes, split and get everything after the division

                try
                {
                    int? teamID = _context.Teams.FirstOrDefault(t => t.TmName.ToLower() == teamName.ToLower())?.ID;

                    // Row by row...
                    p.PlyrFirstName = data[1];
                    p.PlyrLastName = data[2];
                    p.PlyrMemberID = data[3];
                    p.TeamID = (int)teamID;
                    p.Statistics = new Statistic();
                    _context.Players.Add(p);
                    _context.SaveChanges();
                    successCount++;
                    succesfullyImportedPlayers.Add(p);
                }
                catch (Exception ex)
                {
                    errorCount++;
                    p.Team = new Team { TmName = data[7] };
                    failedImportedPlayers.Add(p);
                }
            }

            feedBack += "Finished Importing " + (successCount + errorCount).ToString() +
                        " Players with " + successCount.ToString() + " inserted and " +
                        errorCount.ToString() + " rejected";
            ViewData["SuccesfullyImportedPlayers"] = succesfullyImportedPlayers;
            ViewData["FailedImportedPlayers"] = failedImportedPlayers;
            TempData["PlayerImportFeedback"] = feedBack;
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
        private async void ImportTeamExcel(ExcelPackage excel)
        {
            List<string> importedTeams = new(); //used to keep track of which teams were attempted to be imported.
            List<Team> successfullyImportedTeams = new();
            List<Team> failedImportedTeams = new();
            string feedBack = "";
            var workSheet = excel.Workbook.Worksheets[0];
            var start = workSheet.Dimension.Start;
            var end = workSheet.Dimension.End;
            int successCount = 0;
            int errorCount = 0;

            for (int row = start.Row + 1; row <= end.Row; row++)
            {
                Team t = new();


                string teamName = workSheet.Cells[row, 8].Text.Trim(); //check to see if the team name contains a division
                if (workSheet.Cells[row, 8].Text.Contains("U "))
                    teamName = teamName.Split("U ")[1].Trim(); //if yes, split and get everything after the division

                if (!importedTeams.Contains(teamName)) //check to see if a team was already imported
                    try
                    {
                        int? divID = _context.Divisions.FirstOrDefault(d => d.DivName == workSheet.Cells[row, 6].Text.ToUpper())?.ID;

                        // Row by row...
                        t.TmName = teamName;
                        t.DivisionID = (int)divID;
                        importedTeams.Add(t.TmName);
                        _context.Teams.Add(t);
                        _context.SaveChanges();
                        successCount++;
                        successfullyImportedTeams.Add(t);
                        IdentityResult roleResult;
                        roleResult = await _roleManager.CreateAsync(new ApplicationRole(t.TmName, 0, t.ID));
                    }
                    catch (DbUpdateException dex)
                    {
                        errorCount++;
                        if (dex.GetBaseException().Message.Contains("UNIQUE constraint failed"))
                        {
                            successfullyImportedTeams.Add(t);
                            failedImportedTeams.Add(t);
                        }
                        else
                        {
                            failedImportedTeams.Add(t);
                        }
                        _context.Remove(t);
                    }
                    catch (Exception ex)
                    {
                        errorCount++;
                        failedImportedTeams.Add(t);
                        _context.Remove(t);
                    }
            }
            feedBack += "Finished Importing " + (successCount + errorCount).ToString() +
                        " Teams with " + successCount.ToString() + " inserted and " +
                        errorCount.ToString() + " rejected";
            ViewData["SuccessfullyImportedTeams"] = successfullyImportedTeams;
            ViewData["FailedImportedTeams"] = failedImportedTeams;
            TempData["TeamImportFeedback"] = feedBack;
        }
        private void ImportPlayerExcel(ExcelPackage excel)
        {
            List<Player> succesfullyImportedPlayers = new();
            List<Player> failedImportedPlayers = new();
            string feedBack = "";
            var workSheet = excel.Workbook.Worksheets[0];
            var start = workSheet.Dimension.Start;
            var end = workSheet.Dimension.End;
            int successCount = 0;
            int errorCount = 0;

            for (int row = start.Row + 1; row <= end.Row; row++)
            {
                Player p = new();

                string teamName = workSheet.Cells[row, 8].Text.Trim(); //check to see if the team name contains a division
                if (workSheet.Cells[row, 8].Text.Contains("U "))
                    teamName = teamName.Split("U ")[1].Trim(); //if yes, split and get everything after the division

                try
                {
                    int? teamID = _context.Teams.FirstOrDefault(t => t.TmName.ToLower() == teamName.ToLower())?.ID;

                    // Row by row...
                    p.PlyrFirstName = workSheet.Cells[row, 2].Text;
                    p.PlyrLastName = workSheet.Cells[row, 3].Text;
                    p.PlyrMemberID = workSheet.Cells[row, 4].Text;
                    p.TeamID = (int)teamID;
                    p.Statistics = new Statistic();
                    _context.Players.Add(p);
                    _context.SaveChanges();
                    successCount++;
                    succesfullyImportedPlayers.Add(p);
                }
                catch (Exception ex)
                {
                    errorCount++;
                    p.Team = new Team { TmName = workSheet.Cells[row, 8].Text };
                    failedImportedPlayers.Add(p);
                }
            }
            feedBack += "Finished Importing " + (successCount + errorCount).ToString() +
                        " Players with " + successCount.ToString() + " inserted and " +
                        errorCount.ToString() + " rejected";
            ViewData["SuccesfullyImportedPlayers"] = succesfullyImportedPlayers;
            ViewData["FailedImportedPlayers"] = failedImportedPlayers;
            TempData["PlayerImportFeedback"] = feedBack;
        }

        #endregion
    }
}