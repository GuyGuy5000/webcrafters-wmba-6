using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using wmbaApp.CustomControllers;
using wmbaApp.Data;
using wmbaApp.Models;
using wmbaApp.Utilities;
using Microsoft.AspNetCore.Authorization;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using System.Drawing;

namespace wmbaApp.Controllers
{
    [Authorize(Roles = "Admin,Convenor,Coach")]
    public class StatisticsController : ElephantController
    {
        private readonly WmbaContext _context;
        private readonly ApplicationDbContext _AppContext;

        public StatisticsController(WmbaContext context, ApplicationDbContext appContext)
        {
            _context = context;
            _AppContext = appContext;
        }


        // GET: Statistics
        public async Task<IActionResult> Index(string SearchString, int? page, int? pageSizeID, string actionButton, string sortDirection = "asc", string sortField = "")
        {
            ViewData["Filtering"] = " btn-outline-dark";
            int numberFilters = 0;

            string[] sortOptions = new[] { "Player" };

            IQueryable<Statistic> statistics;
            if (TempData["SuccessMessage"] != null)
            {
                ViewBag.SuccessMessage = TempData["SuccessMessage"].ToString();
            }
            if (User.IsInRole("Admin"))
            {
                statistics = _context.Statistics
                .Include(s => s.Players).ThenInclude(p => p.Team)
                .Where(s => s.Players.First().IsActive)
                .AsNoTracking();
            }
            else
            {
                var rolesTeamIDs = await UserRolesHelper.GetUserTeamIDs(_AppContext, User);
                var rolesDivisionIDs = await UserRolesHelper.GetUserDivisionIDs(_AppContext, User);

                statistics = _context.Statistics
                .Include(s => s.Players).ThenInclude(p => p.Team)
                .Where(s => s.Players.First().IsActive && (rolesTeamIDs.Contains(s.Players.First().TeamID) || rolesDivisionIDs.Contains(s.Players.First().Team.DivisionID)))
                .AsNoTracking();
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


            if (!System.String.IsNullOrEmpty(SearchString))
            {
                var upperSearchString = SearchString.ToUpper(); // Convert the search string to upper case once for efficiency

                statistics = statistics.Where(s => s.Players.Any(p =>

                (p.PlyrFirstName.ToUpper() + " " + p.PlyrLastName.ToUpper()).Contains(SearchString.ToUpper())));


                numberFilters++;
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

            if (sortField == "Player")
            {

                if (sortDirection == "asc")
                {
                    statistics = statistics
                        .OrderBy(p => p.Players.FirstOrDefault().PlyrFirstName);
                }
                else
                {
                    statistics = statistics
                        .OrderByDescending(p => p.Players.FirstOrDefault().PlyrFirstName);
                }
            }



            ViewData["sortField"] = sortField;
            ViewData["sortDirection"] = sortDirection;

            // Default page size to 10 if pageSizeID is null
            int pageNumber = page ?? 1;

            int pageSize = PageSizeHelper.SetPageSize(HttpContext, pageSizeID, ControllerName());
            ViewData["pageSizeID"] = PageSizeHelper.PageSizeList(pageSize);
            var pagedData = await PaginatedList<Statistic>.CreateAsync(statistics.AsNoTracking(), page ?? 1, pageSize);

            // Change the return statement to use the pagedData
            return View(pagedData);
        }


        public IActionResult UpdateRating(int id, int rating)
        {
            var statistic = _context.Statistics.Find(id);
            if (statistic != null)
            {
                statistic.Rating = rating;
                _context.SaveChanges();
                TempData["Message"] = "Rating updated successfully.";

                return Ok("OK");
            }
            return NotFound();
        }


        public async Task<IActionResult> DownloadStatisticsReport()
        {
            IQueryable<Statistic> statistics;

            if (User.IsInRole("Admin"))
            {
                statistics = _context.Statistics
                .Include(s => s.Players).ThenInclude(p => p.Team)
                .Where(s => s.Players.First().IsActive)
                .AsNoTracking();
            }
            else
            {
                var rolesTeamIDs = await UserRolesHelper.GetUserTeamIDs(_AppContext, User);
                var rolesDivisionIDs = await UserRolesHelper.GetUserDivisionIDs(_AppContext, User);

                statistics = _context.Statistics
                .Include(s => s.Players).ThenInclude(p => p.Team)
                .Where(s => s.Players.First().IsActive && (rolesTeamIDs.Contains(s.Players.First().TeamID) || rolesDivisionIDs.Contains(s.Players.First().Team.DivisionID)))
                .AsNoTracking();
            }

            // Get the data from the database
   var statisticsData = statistics
    .AsEnumerable() 
    .OrderBy(r => r.Players.First().FullName) 
    .Select(r => new
    {
        Player = r.Players.First().FullName,
        GP = r.StatsGP,
        PA = r.StatsPA,
        AVG = r.StatsAVG,
        OBP = r.StatsOBP,
        OPS = r.StatsOPS,
        SLG = r.StatsSLG,
        H = r.StatsH,
        R = r.StatsR,
        K = r.StatsK,
        HR = r.StatsHR,
        RBI = r.StatsRBI,
        BB = r.StatsBB,
        Rating = ConvertRatingToWord(r.Rating).ToString()
    });

            // How many rows?
            int numRows = statisticsData.Count();

            if (numRows > 0)
            {
                // Create a new spreadsheet from scratch.
                using (ExcelPackage excel = new ExcelPackage())
                {
                    var workSheet = excel.Workbook.Worksheets.Add("Statistics Report");

                    // Note: Cells[row, column]
                    workSheet.Cells[3, 1].LoadFromCollection(statisticsData, true);

                    // Set column styles if needed

                    // Make certain cells bold
                    workSheet.Cells[4, 1, numRows + 3, 1].Style.Font.Bold = true;

                    // Autofit columns
                    workSheet.Cells.AutoFitColumns();

                    // Add a title and timestamp at the top of the report
                    workSheet.Cells[1, 1].Value = "Statistics Report";
                    using (ExcelRange Rng = workSheet.Cells[1, 1, 1, 15]) // Adjust the column count accordingly
                    {
                        Rng.Merge = true;
                        Rng.Style.Font.Bold = true;
                        Rng.Style.Font.Size = 18;
                        Rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    }

                    DateTime utcDate = DateTime.UtcNow;
                    TimeZoneInfo esTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
                    DateTime localDate = TimeZoneInfo.ConvertTimeFromUtc(utcDate, esTimeZone);
                    using (ExcelRange Rng = workSheet.Cells[2, 15]) // Adjust the column accordingly
                    {
                        Rng.Value = "Created: " + localDate.ToShortTimeString() + " on " +
                            localDate.ToShortDateString();
                        Rng.Style.Font.Bold = true;
                        Rng.Style.Font.Size = 12;
                        Rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    }

                    // Ok, time to download the Excel
                    try
                    {
                        Byte[] theData = excel.GetAsByteArray();
                        string filename = "StatisticsReport.xlsx";
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

        public static string ConvertRatingToWord(int rating)
        {
            switch (rating)
            {
                case 1:
                    return "One Star";
                case 2:
                    return "Two Stars";
                case 3:
                    return "Three Stars";
                case 4:
                    return "Four Stars";
                case 5:
                    return "Five Stars";
                default:
                    return "Unknown Rating";
            }
        }



        // GET: Statistics/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Statistics == null)
            {
                return NotFound();
            }

            var statistic = await _context.Statistics
                .Include(p => p.Players)
                .FirstOrDefaultAsync(m => m.ID == id);

            if (statistic == null)
            {
                return NotFound();
            }

            return View(statistic);
        }

        // GET: Statistics/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Statistics/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,StatsGP,StatsPA,StatsAB,StatsAVG,StatsOBP,StatsOPS,StatsSLG,StatsH,StatsR,StatsK,StatsHR,StatsRBI,StatsBB")] Statistic statistic)
        {
            try
            {

                if (ModelState.IsValid)
                {
                    _context.Add(statistic);
                    await _context.SaveChangesAsync();

                    return Redirect(ViewData["returnURL"].ToString());
                }
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }

            if (!ModelState.IsValid && Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                //Was an AJAX request so build a message with all validation errors
                string errorMessage = "";
                foreach (var modelState in ViewData.ModelState.Values)
                {
                    foreach (ModelError error in modelState.Errors)
                    {
                        errorMessage += error.ErrorMessage + "|";
                    }
                }
                //Note: returning a BadRequest results in HTTP Status code 400
                return BadRequest(errorMessage);
            }
            return View(statistic);
        }

        // GET: Statistics/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Statistics == null)
            {
                return NotFound();
            }

            var statistic = await _context.Statistics.FindAsync(id);
            if (statistic == null)
            {
                return NotFound();
            }
            return View(statistic);
        }

        // POST: Statistics/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Statistic statistic)
        {
            if (id != statistic.ID)
            {
                return NotFound();
            }

            var statisticsToUpdate = await _context.Statistics.FindAsync(id);

            if (statisticsToUpdate == null)
            {
                return NotFound();
            }

            // Update the properties of statisticsToUpdate based on the incoming model
            statisticsToUpdate.Rating = statistic.Rating; // Assuming Rating is the property for the star rating

            try
            {
                // Update the entity in the database
                _context.Update(statisticsToUpdate);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Statistic updated successfully."; // Set success message
                return RedirectToAction(nameof(Index)); // Redirect to the index page or any other appropriate action
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StatisticExists(statisticsToUpdate.ID))
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

            return View(statisticsToUpdate);
        }


        //// GET: Statistics/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null || _context.Statistics == null)
        //    {
        //        return NotFound();
        //    }

        //    var statistic = await _context.Statistics
        //        .FirstOrDefaultAsync(m => m.ID == id);
        //    if (statistic == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(statistic);
        //}

        //// POST: Statistics/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    if (_context.Statistics == null)
        //    {
        //        return Problem("There are Staistics to delete.");
        //    }
        //    var statistic = await _context.Statistics.FindAsync(id);
        //    try
        //    {
        //        if (statistic != null)
        //        {
        //            _context.Statistics.Remove(statistic);
        //        }
        //        await _context.SaveChangesAsync();
        //        return Redirect(ViewData["returnURL"].ToString());
        //    }
        //    catch (DbUpdateException dex)
        //    {
        //        if (dex.GetBaseException().Message.Contains("FOREIGN KEY constraint failed"))
        //        {
        //            ModelState.AddModelError("", "Unable to Delete Staistics. Remember, you cannot delete a Staistics that is used in the system.");
        //        }
        //        else
        //        {
        //            ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
        //        }
        //    }


        //    return View(statistic);
        //}


        // GET: Players/Delete
        [Authorize(Roles = "Admin,Convenor")]
        public async Task<IActionResult> Delete()
        {
            try
            {
                // Retrieve all games from the database
                var allPlayerStats = await _context.Statistics.ToListAsync();

                if (allPlayerStats != null && allPlayerStats.Count > 0)
                {
                    // Generate Excel file asynchronously
                    byte[] theData = await GenerateExcelFileAsync(allPlayerStats); // Updated method name
                    string filename = "Deleted Statistics.xlsx";
                    string mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                    // Delete the games and set success message
                    await DeletePlayersStatsAndShowMessage(allPlayerStats);

                    // Return the Excel file for download
                    var fileContentResult = File(theData, mimeType, filename);
                    TempData["SuccessMessage"] = "All players have been downloaded, click again to delete.";
                    // Return the file content result for download
                    return fileContentResult;

                }
                else
                {
                    TempData["SuccessMessage"] = "All players have been deleted  and downloaded successfully.";
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions and log them
                TempData["ErrorMessage"] = "An error occurred while processing your request.";
            }

            // Redirect to appropriate page if download or deletion fails
            return RedirectToAction("Index", "Statistics");
        }



        private async Task<byte[]> GenerateExcelFileAsync(List<Statistic> stats)
        {
            var sumQ = _context.Statistics
             .Include(t => t.Players)
                .OrderBy(r => r.Players.FirstOrDefault().FullName)
                .Select(r => new
                {
                    playe_r  = r.Players.FirstOrDefault().FullName,
                    Stats_GP = r.StatsGP,
                    Stats_PA = r.StatsPA,
                    Stats_AB = r.StatsAB,
                    Stats_AVG = r.StatsAVG,
                    Stats_OBP = r.StatsOBP,
                    Stats_OPS = r.StatsOPS,
                    Stats_SLG = r.StatsSLG,
                    Stats_H = r.StatsH,
                    Stats_R = r.StatsR,
                    Stats_K = r.StatsK,
                    Stats_HR = r.StatsHR,
                    Stats_RBI = r.StatsRBI,
                    Stats_BB = r.StatsBB,
                    Rating = ConvertRatingToWord(r.Rating).ToString()

                })
                .AsNoTracking();

            // How many rows?
            int numRows = await sumQ.CountAsync();

            if (numRows > 0)
            {
                using (ExcelPackage excel = new ExcelPackage())
                {
                    var workSheet = excel.Workbook.Worksheets.Add("Players");

                    workSheet.Cells[3, 1].LoadFromCollection(sumQ, true);

                    workSheet.Cells[4, 1, numRows + 3, 1].Style.Font.Bold = true;

                    using (ExcelRange headings = workSheet.Cells[3, 1, 3, 15])
                    {
                        headings.Style.Font.Bold = true;
                        var fill = headings.Style.Fill;
                        fill.PatternType = ExcelFillStyle.Solid;
                        fill.BackgroundColor.SetColor(Color.LightBlue);
                    }

                    workSheet.Cells.AutoFitColumns();

                    workSheet.Cells[1, 1].Value = "Players";
                    using (ExcelRange Rng = workSheet.Cells[1, 1, 1, 15])
                    {
                        Rng.Merge = true;
                        Rng.Style.Font.Bold = true;
                        Rng.Style.Font.Size = 18;
                        Rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    }

                    DateTime utcDate = DateTime.UtcNow;
                    TimeZoneInfo esTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
                    DateTime localDate = TimeZoneInfo.ConvertTimeFromUtc(utcDate, esTimeZone);
                    using (ExcelRange Rng = workSheet.Cells[2, 15])
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


            return null;
        }


        private async Task DeletePlayersStatsAndShowMessage(List<Statistic> stats)
        {
            try
            {
                // Check if there are any players to delete
                if (stats == null || stats.Count == 0)
                {
                    TempData["SuccessMessage"] = "There are no players available.";
                    return;
                }

                // Delete all players and let cascade delete handle related entities
                _context.Statistics.RemoveRange(stats);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "All players have been deleted successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred while deleting players: {ex.Message}";
            }
        }



        private bool StatisticExists(int id)
        {
            return _context.Statistics.Any(e => e.ID == id);
        }
    }
}