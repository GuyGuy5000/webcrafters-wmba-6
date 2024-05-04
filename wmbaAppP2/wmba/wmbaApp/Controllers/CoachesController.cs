using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    [Authorize(Roles ="Admin,Convenor")]
    public class CoachesController : ElephantController
    {
        private readonly WmbaContext _context;

        public CoachesController(WmbaContext context)
        {
            _context = context;
        }

        // GET: Coaches
        public async Task<IActionResult> Index(string SearchString, int? DivisionID, int? TeamID,
             int? page, int? pageSizeID, string actionButton, string sortDirection = "asc", string sortField = "")
        {
            //Count the number of filters applied - start by assuming no filters
            ViewData["Filtering"] = " btn-outline-dark";
            int numberFilters = 0;
            //Then in each "test" for filtering, add to the count of Filters applied

            //List of sort options.
            //NOTE: make sure this array has matching values to the column headings
            string[] sortOptions = new[] { "Coach Name" };

            PopulateDropDownLists();

            var coaches = _context.Coaches
                .Include(c => c.DivisionCoaches).ThenInclude(c => c.Division)
                .Include(c => c.DivisionCoaches).ThenInclude(c => c.Team)
                .AsNoTracking();

            //Add as many filters as needed
            if (DivisionID.HasValue)
            {
                coaches = coaches.Where(c => c.DivisionCoaches.Any(t => t.DivisionID == DivisionID));
            }
            if (TeamID.HasValue)
            {
                coaches = coaches.Where(c => c.DivisionCoaches.Any(t => t.TeamID == TeamID));
            }

            if (!System.String.IsNullOrEmpty(SearchString))
            {
                coaches = coaches.Where(c => c.CoachFirstName.ToUpper().Contains(SearchString.ToUpper())
                                          || c.CoachLastName.ToUpper().Contains(SearchString.ToUpper())
                                          || c.CoachEmail.ToUpper().Contains(SearchString.ToUpper())
                                          || c.CoachPhone.ToUpper().Contains(SearchString.ToUpper())
                                       );
                numberFilters++;
            }
            //Give feedback about the state of the filters
            if (numberFilters != 0)
            {
                //Toggle the Open/Closed state of the collapse depending on if we are filtering
                ViewData["Filtering"] = " btn-outline-dark";
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

            if (sortField == "Coach Name")
            {

                if (sortDirection == "asc")
                {
                    coaches = coaches
                        .OrderBy(c => c.CoachFirstName);
                }
                else
                {
                    coaches = coaches
                        .OrderByDescending(c => c.CoachFirstName);
                }
            }

            ViewData["sortField"] = sortField;
            ViewData["sortDirection"] = sortDirection;

            int pageSize = PageSizeHelper.SetPageSize(HttpContext, pageSizeID, ControllerName());
            ViewData["pageSizeID"] = PageSizeHelper.PageSizeList(pageSize);
            var pagedData = await PaginatedList<Coach>.CreateAsync(coaches.AsNoTracking(), page ?? 1, pageSize);

            return View(pagedData);
        }

        // GET: Coaches/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Coaches == null)
            {
                return NotFound();
            }

            var coach = await _context.Coaches
                .Include(p=>p.DivisionCoaches).ThenInclude(p=>p.Team)
                .Include(p => p.DivisionCoaches).ThenInclude(p => p.Division)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (coach == null)
            {
                return NotFound();
            }

            return View(coach);
        }

        // GET: Coaches/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Coaches/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,CoachFirstName,CoachLastName,CoachEmail,CoachPhone")] Coach coach)
        {
            if (ModelState.IsValid)
            {
                _context.Add(coach);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(coach);
        }

        // GET: Coaches/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Coaches == null)
            {
                return NotFound();
            }

            var coach = await _context.Coaches
                .Include(c => c.DivisionCoaches).ThenInclude(c => c.Division)
                .Include(c => c.DivisionCoaches).ThenInclude(c => c.Team)
                .FirstOrDefaultAsync(c => c.ID == id);

            if (coach == null)
            {
                return NotFound();
            }
            return View(coach);
        }

        // POST: Coaches/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id)
        {
            var coachToUpdate = await _context.Coaches
                .Include(c => c.DivisionCoaches).ThenInclude(c => c.Division)
                .Include(c => c.DivisionCoaches).ThenInclude(c => c.Team)
                .FirstOrDefaultAsync(c => c.ID == id);

            if (coachToUpdate == null)
            {
                return NotFound();
            }

            if (await TryUpdateModelAsync<Coach>(coachToUpdate, "",
                c => c.CoachFirstName, c => c.CoachLastName, c => c.CoachEmail, c => c.CoachPhone))
            {
                try
                {
                    _context.Update(coachToUpdate);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CoachExists(coachToUpdate.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(coachToUpdate);
        }

        // GET: Coaches/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Coaches == null)
            {
                return NotFound();
            }

            var coach = await _context.Coaches
                .FirstOrDefaultAsync(m => m.ID == id);
            if (coach == null)
            {
                return NotFound();
            }

            return View(coach);
        }

        // POST: Coaches/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Coaches == null)
            {
                return Problem("Entity set 'WmbaContext.Coaches'  is null.");
            }
            var coach = await _context.Coaches.FindAsync(id);
            if (coach != null)
            {
                _context.Coaches.Remove(coach);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }



        // GET: Games/Delete
        [Authorize(Roles = "Admin,Convenor")]
        public async Task<IActionResult> DeleteAll()
        {
            try
            {
                // Retrieve all games from the database
                var allCoaches = await _context.Coaches.ToListAsync();

                if (allCoaches != null && allCoaches.Count > 0)
                {
                    // Generate Excel file asynchronously
                    byte[] theData = await GenerateExcelFileAsync(allCoaches);
                    string filename = "Deleted Coaches.xlsx";
                    string mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                    // Delete the games and set success message
                    await DeleteGamesAndShowMessage(allCoaches);

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
            return RedirectToAction("Index", "Coaches");
        }

        private async Task<byte[]> GenerateExcelFileAsync(List<Coach> coach)
        {
            var sumQ = _context.Coaches
                 .Include(c => c.DivisionCoaches).ThenInclude(c => c.Division)
                .Include(c => c.DivisionCoaches).ThenInclude(c => c.Team)
                .OrderBy(r => r.CoachFirstName)
                .Select(r => new
                {
                    Coach_Name = r.CoachFirstName,
                    Team_ = r.DivisionCoaches.FirstOrDefault().Team.TmName,
                    Division_ = r.DivisionCoaches.FirstOrDefault().Division.DivName

                })
                .AsNoTracking();

            // How many rows?
            int numRows = await sumQ.CountAsync();

            if (numRows > 0)
            {
                using (ExcelPackage excel = new ExcelPackage())
                {
                    var workSheet = excel.Workbook.Worksheets.Add("Coaches");

                    workSheet.Cells[3, 1].LoadFromCollection(sumQ, true);

                    workSheet.Cells[3, 1, numRows + 3, 1].Style.Font.Bold = true;

                    using (ExcelRange headings = workSheet.Cells[3, 1, 3, 3])
                    {
                        headings.Style.Font.Bold = true;
                        var fill = headings.Style.Fill;
                        fill.PatternType = ExcelFillStyle.Solid;
                        fill.BackgroundColor.SetColor(Color.LightBlue);
                    }

                    workSheet.Cells.AutoFitColumns();

                    workSheet.Cells[1, 1].Value = "Players";
                    using (ExcelRange Rng = workSheet.Cells[1, 1, 1, 3])
                    {
                        Rng.Merge = true;
                        Rng.Style.Font.Bold = true;
                        Rng.Style.Font.Size = 18;
                        Rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    }

                    DateTime utcDate = DateTime.UtcNow;
                    TimeZoneInfo esTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
                    DateTime localDate = TimeZoneInfo.ConvertTimeFromUtc(utcDate, esTimeZone);
                    using (ExcelRange Rng = workSheet.Cells[2, 3])
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

        private async Task DeleteGamesAndShowMessage(List<Coach> coach)
        {
            // Delete the DivisionCoaches

            try
            {
                if( coach != null && coach.Count > 0)
                {
                    TempData["SuccessMessage"] = "There are currently no coaches, please try again";
                }

                foreach (var coaches in coach)
                {
                    _context.DivisionCoaches.RemoveRange(coaches.DivisionCoaches);
                }
                await _context.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                TempData["SuccessMessage"] = $"An error occured: {ex.Message}";
            }
           
            _context.Coaches.RemoveRange(coach);
            await _context.SaveChangesAsync();

            // Show success message
            TempData["SuccessMessage"] = "All games have been deleted successfully.";
        }



        #region SelectLists
        private SelectList DivisionSelectList(int? selectedId)
        {
            return new SelectList(_context.Divisions, "ID", "DivName", selectedId);
        }
        private SelectList TeamSelectList(int? selectedId)
        {
            return new SelectList(_context.Teams, "ID", "TmName", selectedId);
        }
        private void PopulateDropDownLists(Coach coach = null)
        {
            ViewData["DivisionID"] = DivisionSelectList(coach?.DivisionCoaches.FirstOrDefault(c => c.DivisionID == coach?.ID)?.DivisionID);
            ViewData["TeamID"] = TeamSelectList(coach?.DivisionCoaches.FirstOrDefault(c => c.DivisionID == coach?.ID)?.TeamID);
        }
        #endregion

        private bool CoachExists(int id)
        {
            return _context.Coaches.Any(e => e.ID == id);
        }
    }
}