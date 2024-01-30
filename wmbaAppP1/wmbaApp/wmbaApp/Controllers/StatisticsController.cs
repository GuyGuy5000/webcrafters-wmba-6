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

namespace wmbaApp.Controllers
{
    public class StatisticsController : ElephantController
    {
        private readonly WmbaContext _context;

        public StatisticsController(WmbaContext context)
        {
            _context = context;
        }

        // GET: Statistics
        public async Task<IActionResult> Index(int? page, int? pageSizeID)
        {
            var statistics =  _context.Statistics
                .Include(s => s.Players)
                .AsNoTracking();

            int pageSize = PageSizeHelper.SetPageSize(HttpContext, pageSizeID, ControllerName());
            ViewData["pageSizeID"] = PageSizeHelper.PageSizeList(pageSize);
            var pagedData = await PaginatedList<Statistic>.CreateAsync(statistics.AsNoTracking(), page ?? 1, pageSize);

            // Change the return statement to use the pagedData
            return View(pagedData);
        }

        public IActionResult DownloadStatisticsReport()
        {
            // Get the data from the database
            var statisticsData = _context.Statistics
                .OrderBy(r => r.StatsGP) // Change to the actual property for player name
                .Select(r => new
                {
                    
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
                    BB = r.StatsBB
                    // Add other properties as needed
                })
                .AsNoTracking();

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
                    using (ExcelRange Rng = workSheet.Cells[1, 1, 1, 13]) // Adjust the column count accordingly
                    {
                        Rng.Merge = true;
                        Rng.Style.Font.Bold = true;
                        Rng.Style.Font.Size = 18;
                        Rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    }

                    DateTime utcDate = DateTime.UtcNow;
                    TimeZoneInfo esTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
                    DateTime localDate = TimeZoneInfo.ConvertTimeFromUtc(utcDate, esTimeZone);
                    using (ExcelRange Rng = workSheet.Cells[2, 13]) // Adjust the column accordingly
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


        // GET: Statistics/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Statistics == null)
            {
                return NotFound();
            }

            var statistic = await _context.Statistics
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
        public async Task<IActionResult> Edit(int id)
        {

            var statistic = await _context.Statistics
                .Include(t => t.Players)
                .FirstOrDefaultAsync(s => s.ID == id);

    

            var statisticsToUpdate = await _context.Statistics.FirstOrDefaultAsync(p => p.ID == id);

            if (statisticsToUpdate == null)
            {
                return NotFound();
            }


            if (await TryUpdateModelAsync<Statistic>(statisticsToUpdate, "",
               s => s.StatsGP, s => s.StatsPA, s => s.StatsAB, s => s.StatsAVG, s => s.StatsOBP, s => s.StatsOPS, s => s.StatsSLG, 
              s => s.StatsH, s => s.StatsR, s => s.StatsK, s => s.StatsHR, s => s.StatsRBI, s => s.StatsBB))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    return Redirect(ViewData["returnURL"].ToString());
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
            }
            return View(statisticsToUpdate);
        }

        // GET: Statistics/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Statistics == null)
            {
                return NotFound();
            }

            var statistic = await _context.Statistics
                .FirstOrDefaultAsync(m => m.ID == id);
            if (statistic == null)
            {
                return NotFound();
            }

            return View(statistic);
        }

        // POST: Statistics/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Statistics == null)
            {
                return Problem("There are Staistics to delete.");
            }
            var statistic = await _context.Statistics.FindAsync(id);
            try
            {
                if (statistic != null)
                {
                    _context.Statistics.Remove(statistic);
                }
                await _context.SaveChangesAsync();
                return Redirect(ViewData["returnURL"].ToString());
            }
            catch (DbUpdateException dex)
            {
                if (dex.GetBaseException().Message.Contains("FOREIGN KEY constraint failed"))
                {
                    ModelState.AddModelError("", "Unable to Delete Staistics. Remember, you cannot delete a Staistics that is used in the system.");
                }
                else
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                }
            }


            return View(statistic);
        }

        private bool StatisticExists(int id)
        {
            return _context.Statistics.Any(e => e.ID == id);
        }
    }
}
