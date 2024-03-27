using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Numerics;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using wmbaApp.CustomControllers;
using wmbaApp.Data;
using wmbaApp.Models;
using wmbaApp.Utilities;
using wmbaApp.ViewModels;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace wmbaApp.Controllers
{
    [Authorize]
    public class PlayersController : ElephantController
    {
        private readonly WmbaContext _context;

        public PlayersController(WmbaContext context)
        {
            _context = context;
        }

        // GET: Players
        [Authorize(Roles = "Admin,Convenor,Coach,ScoreKeeper,IntermediateC,RookieC")]
        public async Task<IActionResult> Index(string SearchString, int? TeamID,
             int? page, int? pageSizeID, string actionButton, string sortDirection = "asc", string sortField = "")
        {
            //Count the number of filters applied - start by assuming no filters
            ViewData["Filtering"] = "btn-outline-secondary";
            int numberFilters = 0;
            //Then in each "test" for filtering, add to the count of Filters applied

            //List of sort options.
            //NOTE: make sure this array has matching values to the column headings
            string[] sortOptions = new[] { "Name", "Team" };

            PopulateDropDownLists();

            var players = _context.Players
                .Include(t => t.Team).ThenInclude(t => t.Division)
                .Include(t => t.Statistics)
                .Where(t => t.IsActive)
                .AsNoTracking();

            //Add as many filters as needed
            if (TeamID.HasValue)
            {
                players = players.Where(p => p.TeamID == TeamID);
                numberFilters++;
            }

            if (!System.String.IsNullOrEmpty(SearchString))
            {
                players = players.Where(p => p.PlyrFirstName.ToUpper().Contains(SearchString.ToUpper())
                                       || p.PlyrLastName.ToUpper().Contains(SearchString.ToUpper())
                                       || p.Team.TmName.ToUpper().Contains(SearchString.ToUpper())
                                       );

                numberFilters++;
            }
            //Give feedback about the state of the filters
            if (numberFilters != 0)
            {
                //Toggle the Open/Closed state of the collapse depending on if we are filtering
                ViewData["Filtering"] = " btn-danger";
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
            //Now we know which field and direction to sort by
            if (sortField == "Name")
            {

                if (sortDirection == "asc")
                {
                    players = players
                        .OrderBy(p => p.PlyrFirstName);
                }
                else
                {
                    players = players
                        .OrderByDescending(p => p.PlyrFirstName);
                }
            }
            else
            {
                if (sortDirection == "asc")
                {
                    players = players
                        .OrderBy(p => p.Team);
                }
                else
                {
                    players = players
                        .OrderByDescending(p => p.Team);
                }
            }
            ViewData["sortField"] = sortField;
            ViewData["sortDirection"] = sortDirection;

            int pageSize = PageSizeHelper.SetPageSize(HttpContext, pageSizeID, ControllerName());
            ViewData["pageSizeID"] = PageSizeHelper.PageSizeList(pageSize);
            var pagedData = await PaginatedList<Player>.CreateAsync(players.AsNoTracking(), page ?? 1, pageSize);

            return View(pagedData);
        }

        // GET: Players
        [Authorize(Roles = "Admin,Convenor,Coach,ScoreKeeper,IntermediateC,RookieC")]
        public async Task<IActionResult> InactiveIndex(string SearchString, int? TeamID,
             int? page, int? pageSizeID, string actionButton, string sortDirection = "asc", string sortField = "")
        {
            //Count the number of filters applied - start by assuming no filters
            ViewData["Filtering"] = "btn-outline-secondary";
            int numberFilters = 0;
            //Then in each "test" for filtering, add to the count of Filters applied

            //List of sort options.
            //NOTE: make sure this array has matching values to the column headings
            string[] sortOptions = new[] { "Name", "Team" };

            PopulateDropDownLists();

            var players = _context.Players
                .Include(t => t.Team).ThenInclude(t => t.Division)
                .Include(t => t.Statistics)
                .Where(t => !t.IsActive)
                .AsNoTracking();

            //Add as many filters as needed
            if (TeamID.HasValue)
            {
                players = players.Where(p => p.TeamID == TeamID);
                numberFilters++;
            }

            if (!System.String.IsNullOrEmpty(SearchString))
            {
                players = players.Where(p => p.PlyrFirstName.ToUpper().Contains(SearchString.ToUpper())
                                       || p.PlyrLastName.ToUpper().Contains(SearchString.ToUpper())
                                       || p.Team.TmName.ToUpper().Contains(SearchString.ToUpper())
                                       );

                numberFilters++;
            }
            //Give feedback about the state of the filters
            if (numberFilters != 0)
            {
                //Toggle the Open/Closed state of the collapse depending on if we are filtering
                ViewData["Filtering"] = " btn-danger";
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
            //Now we know which field and direction to sort by
            if (sortField == "Name")
            {

                if (sortDirection == "asc")
                {
                    players = players
                        .OrderBy(p => p.PlyrFirstName);
                }
                else
                {
                    players = players
                        .OrderByDescending(p => p.PlyrFirstName);
                }
            }
            else
            {
                if (sortDirection == "asc")
                {
                    players = players
                        .OrderBy(p => p.Team);
                }
                else
                {
                    players = players
                        .OrderByDescending(p => p.Team);
                }
            }
            ViewData["sortField"] = sortField;
            ViewData["sortDirection"] = sortDirection;

            int pageSize = PageSizeHelper.SetPageSize(HttpContext, pageSizeID, ControllerName());
            ViewData["pageSizeID"] = PageSizeHelper.PageSizeList(pageSize);
            var pagedData = await PaginatedList<Player>.CreateAsync(players.AsNoTracking(), page ?? 1, pageSize);

            return View(pagedData);
        }


        // GET: Players/Details/5
        [Authorize(Roles = "Admin,Convenor,Coach,ScoreKeeper,IntermediateC,RookieC")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Players == null)
            {
                return NotFound();
            }

            var player = await _context.Players
                .Include(p => p.Team).ThenInclude(p => p.Division)
                .Include(p => p.Statistics)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID == id);
            if (player == null)
            {
                return NotFound();
            }

            // making sure the login coach is same as coach associated to the team
            var userId = User.FindFirst(ClaimTypes.Email)?.Value;
            // making sure roles is what we want
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var findingCoach = await _context.DivisionCoaches
                .AnyAsync(dc => dc.TeamID == player.TeamID && dc.Coach.CoachEmail == userId);

            switch (userRole)
            {
                case "RookieC":
                    if (player.Team.Division.DivName != "9U")
                    {
                        if (!findingCoach)
                        {
                            return Forbid();
                        }
                    }
                    break;
                case "IntermediateC":
                    if (player.Team.Division.DivName != "11U" && player.Team.Division.DivName != "13U")
                    {
                        if (!findingCoach)
                        {
                            return Forbid();
                        }
                    }
                    break;
                case "Convenor":
                    if (player.Team.Division.DivName != "15U" && player.Team.Division.DivName != "18U")
                    {
                        if (!findingCoach)
                        {
                            return Forbid();
                        }
                    }
                    break;
                default:
                    break;
            }

            return View(player);
        }

        // GET: Players/Create
        [Authorize(Roles = "Admin,Coach")]
        public IActionResult Create()
        {
            Player player = new Player();
            PopulateDropDownLists(player);
            return View(player);
        }

        // POST: Players/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Coach")]
        public async Task<IActionResult> Create([Bind("ID,PlyrFirstName,PlyrLastName,PlyrJerseyNumber," +
            "PlyrMemberID,TeamID,StatisticID")] Player player)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(player);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Details", new { player.ID });
                }

            }
            catch (RetryLimitExceededException /* dex */)
            {
                ModelState.AddModelError("", "Unable to save changes after multiple attempts. Try again, and if the problem persists, see your system administrator.");
            }
            catch (DbUpdateException dex)
            {
                if (dex.InnerException.Message.Contains("UNIQUE")) //if a UNIQUE constraint caused the exception
                {
                    //check which field triggered the error
                    if (dex.InnerException.Message.Contains("PlyrMemberID"))
                        ModelState.AddModelError("PlyrMemberID", "A player with this member ID already exists."); //pass a message to the field that triggered the error
                    else if (dex.InnerException.Message.Contains("PlyrJerseyNumber"))
                        ModelState.AddModelError("PlyrJerseyNumber", "A player with this jersey number already exists.");
                }
                else
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }

            PopulateDropDownLists(player);
            return View(player);
        }

        // GET: Players/Edit/5
        [Authorize(Roles = "Admin,Convenor,Coach,ScoreKeeper,IntermediateC,RookieC")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Players == null)
            {
                return NotFound();
            }

            var player = await _context.Players
                .Include(p => p.Team).ThenInclude(p => p.Division)
                .Where(p => p.IsActive)
                .FirstOrDefaultAsync(f => f.ID == id);

            if (player == null)
            {
                return NotFound();
            }

            // making sure the login coach is same as coach associated to the team
            var userId = User.FindFirst(ClaimTypes.Email)?.Value;
            // making sure roles is what we want
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var findingCoach = await _context.DivisionCoaches
                .AnyAsync(dc => dc.TeamID == player.TeamID && dc.Coach.CoachEmail == userId);

            switch (userRole)
            {
                case "RookieC":
                    if (player.Team.Division.DivName != "9U")
                    {
                        if (!findingCoach)
                        {
                            return Forbid();
                        }
                    }
                    break;
                case "IntermediateC":
                    if (player.Team.Division.DivName != "11U" && player.Team.Division.DivName != "13U")
                    {
                        if (!findingCoach)
                        {
                            return Forbid();
                        }
                    }
                    break;
                case "Convenor":
                    if (player.Team.Division.DivName != "15U" && player.Team.Division.DivName != "18U")
                    {
                        if (!findingCoach)
                        {
                            return Forbid();
                        }
                    }
                    break;
                default:
                    break;
            }

            PopulateDropDownLists(player);
            return View(player);
        }


        // POST: Players/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Convenor,Coach,ScoreKeeper,IntermediateC,RookieC")]
        public async Task<IActionResult> Edit(int id)
        {
            var playerToUpdate = await _context.Players
                .Include(t => t.Team).ThenInclude(t => t.Division)
            .FirstOrDefaultAsync(m => m.ID == id);

            if (playerToUpdate == null)
            {
                return NotFound();
            }

            // making sure the login coach is same as coach associated to the team
            var userId = User.FindFirst(ClaimTypes.Email)?.Value;
            // making sure roles is what we want
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var findingCoach = await _context.DivisionCoaches
                .AnyAsync(dc => dc.TeamID == playerToUpdate.TeamID && dc.Coach.CoachEmail == userId);

            switch (userRole)
            {
                case "RookieC":
                    if (playerToUpdate.Team.Division.DivName != "9U")
                    {
                        if (!findingCoach)
                        {
                            return Forbid();
                        }
                    }
                    break;
                case "IntermediateC":
                    if (playerToUpdate.Team.Division.DivName != "11U" && playerToUpdate.Team.Division.DivName != "13U")
                    {
                        if (!findingCoach)
                        {
                            return Forbid();
                        }
                    }
                    break;
                case "Convenor":
                    if (playerToUpdate.Team.Division.DivName != "15U" && playerToUpdate.Team.Division.DivName != "18U")
                    {
                        if (!findingCoach)
                        {
                            return Forbid();
                        }
                    }
                    break;
                default:
                    break;
            }

            if (await TryUpdateModelAsync<Player>(playerToUpdate, "",
                t => t.PlyrFirstName, t => t.PlyrLastName, t => t.TeamID, t => t.PlyrJerseyNumber, t => t.PlyrMemberID,
                t => t.StatisticID))
            {
                try
                {
                    _context.Update(playerToUpdate);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Details", new { playerToUpdate.ID });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PlayerExists(playerToUpdate.ID))
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
                    if (dex.InnerException.Message.Contains("UNIQUE")) //if a UNIQUE constraint caused the exception
                    {
                        //check which field triggered the error
                        if (dex.InnerException.Message.Contains("PlyrMemberID"))
                            ModelState.AddModelError("PlyrMemberID", "A player with this member ID already exists."); //pass a message to the field that triggered the error
                        else if (dex.InnerException.Message.Contains("PlyrJerseyNumber"))
                            ModelState.AddModelError("PlyrJerseyNumber", "A player with this jersey number already exists.");
                    }
                    else
                        ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                }
            }
            PopulateDropDownLists(playerToUpdate);
            return View(playerToUpdate);
        }

        //// GET: Players/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null || _context.Players == null)
        //    {
        //        return NotFound();
        //    }

        //    var player = await _context.Players
        //        .FirstOrDefaultAsync(m => m.ID == id);
        //    if (player == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(player);
        //}

        //// POST: Players/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    if (_context.Players == null)
        //    {
        //        return Problem("Entity set 'WmbaContext.Players' is null.");
        //    }
        //    var player = await _context.Players.FindAsync(id);
        //    if (player != null)
        //    {
        //        _context.Players.Remove(player);
        //    }

        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        // GET: Players/Inactive/5
        [Authorize(Roles = "Admin,Convenor,Coach,ScoreKeeper,IntermediateC,RookieC")]
        public async Task<IActionResult> MakeInactive(int? id)
        {
            if (id == null || _context.Players == null)
                return NotFound();

            var player = await _context.Players
                .Include(p => p.Team).ThenInclude(p=>p.Division)
                .Include(p => p.Statistics)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (player == null)
                return NotFound();

            // making sure the login coach is same as coach associated to the team
            var userId = User.FindFirst(ClaimTypes.Email)?.Value;
            // making sure roles is what we want
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var findingCoach = await _context.DivisionCoaches
                .AnyAsync(dc => dc.TeamID == player.TeamID && dc.Coach.CoachEmail == userId);

            switch (userRole)
            {
                case "RookieC":
                    if (player.Team.Division.DivName != "9U")
                    {
                        if (!findingCoach)
                        {
                            return Forbid();
                        }
                    }
                    break;
                case "IntermediateC":
                    if (player.Team.Division.DivName != "11U" && player.Team.Division.DivName != "13U")
                    {
                        if (!findingCoach)
                        {
                            return Forbid();
                        }
                    }
                    break;
                case "Convenor":
                    if (player.Team.Division.DivName != "15U" && player.Team.Division.DivName != "18U")
                    {
                        if (!findingCoach)
                        {
                            return Forbid();
                        }
                    }
                    break;
                default:
                    break;
            }

            return View(player);
        }

        // POST: Players/Inactive/5
        [HttpPost, ActionName("MakeInactive")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Convenor,Coach,ScoreKeeper,IntermediateC,RookieC")]
        public async Task<IActionResult> MakeInactiveConfirmed(int id)
        {
            if (_context.Players == null)
            {
                return Problem("Entity set 'WmbaContext.Players' is null.");
            }

            var player = await _context.Players
                .Include(p => p.Team).ThenInclude(p => p.Division)
                .Include(p => p.Statistics)
                .FirstOrDefaultAsync(p => p.ID == id);

            if (player != null)
            {
                player.IsActive = false;
                _context.Players.Update(player);
            }

            // making sure the login coach is same as coach associated to the team
            var userId = User.FindFirst(ClaimTypes.Email)?.Value;
            // making sure roles is what we want
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var findingCoach = await _context.DivisionCoaches
                .AnyAsync(dc => dc.TeamID == player.TeamID && dc.Coach.CoachEmail == userId);

            switch (userRole)
            {
                case "RookieC":
                    if (player.Team.Division.DivName != "9U")
                    {
                        if (!findingCoach)
                        {
                            return Forbid();
                        }
                    }
                    break;
                case "IntermediateC":
                    if (player.Team.Division.DivName != "11U" && player.Team.Division.DivName != "13U")
                    {
                        if (!findingCoach)
                        {
                            return Forbid();
                        }
                    }
                    break;
                case "Convenor":
                    if (player.Team.Division.DivName != "15U" && player.Team.Division.DivName != "18U")
                    {
                        if (!findingCoach)
                        {
                            return Forbid();
                        }
                    }
                    break;
                default:
                    break;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        // GET: Players/Active/5
        [Authorize(Roles = "Admin,Convenor,Coach,ScoreKeeper,IntermediateC,RookieC")]
        public async Task<IActionResult> MakeActive(int? id)
        {
            if (id == null || _context.Players == null)
                return NotFound();

            var player = await _context.Players
                .Include(p => p.Team).ThenInclude(p => p.Division)
                .Include(p => p.Statistics)
                .FirstOrDefaultAsync(m => m.ID == id);

            if (player == null)
                return NotFound();

            // making sure the login coach is same as coach associated to the team
            var userId = User.FindFirst(ClaimTypes.Email)?.Value;
            // making sure roles is what we want
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var findingCoach = await _context.DivisionCoaches
                .AnyAsync(dc => dc.TeamID == player.TeamID && dc.Coach.CoachEmail == userId);

            switch (userRole)
            {
                case "RookieC":
                    if (player.Team.Division.DivName != "9U")
                    {
                        if (!findingCoach)
                        {
                            return Forbid();
                        }
                    }
                    break;
                case "IntermediateC":
                    if (player.Team.Division.DivName != "11U" && player.Team.Division.DivName != "13U")
                    {
                        if (!findingCoach)
                        {
                            return Forbid();
                        }
                    }
                    break;
                case "Convenor":
                    if (player.Team.Division.DivName != "15U" && player.Team.Division.DivName != "18U")
                    {
                        if (!findingCoach)
                        {
                            return Forbid();
                        }
                    }
                    break;
                default:
                    break;
            }

            return View(player);
        }

        // POST: Players/Active/5
        [HttpPost, ActionName("MakeActive")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Convenor,Coach,ScoreKeeper,IntermediateC,RookieC")]
        public async Task<IActionResult> MakeActiveConfirmed(int id)
        {
            if (_context.Players == null)
            {
                return Problem("Entity set 'WmbaContext.Players' is null.");
            }
            var player = await _context.Players
                .Include(p => p.Team).ThenInclude(p => p.Division)
                .Include(p => p.Statistics)
                .FirstOrDefaultAsync(p => p.ID == id);

            if (player != null)
            {
                if (player.Team.IsActive)
                {
                    player.IsActive = true;
                    _context.Players.Update(player);
                }
                else
                {
                    ModelState.AddModelError("", "This player's team is inactive. Reactivate the team or reassign this player to a different team to reactivate");
                    return View(player);
                }
            }

            // making sure the login coach is same as coach associated to the team
            var userId = User.FindFirst(ClaimTypes.Email)?.Value;
            // making sure roles is what we want
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var findingCoach = await _context.DivisionCoaches
                .AnyAsync(dc => dc.TeamID == player.TeamID && dc.Coach.CoachEmail == userId);

            switch (userRole)
            {
                case "RookieC":
                    if (player.Team.Division.DivName != "9U")
                    {
                        if (!findingCoach)
                        {
                            return Forbid();
                        }
                    }
                    break;
                case "IntermediateC":
                    if (player.Team.Division.DivName != "11U" && player.Team.Division.DivName != "13U")
                    {
                        if (!findingCoach)
                        {
                            return Forbid();
                        }
                    }
                    break;
                case "Convenor":
                    if (player.Team.Division.DivName != "15U" && player.Team.Division.DivName != "18U")
                    {
                        if (!findingCoach)
                        {
                            return Forbid();
                        }
                    }
                    break;
                default:
                    break;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("InactiveIndex");
        }



        [Authorize(Roles = "Admin,Convenor,Coach,ScoreKeeper,IntermediateC,RookieC")]
        public IActionResult DownloadInactivePlayersReport()
        {
            // Get the data from the database
            var inactivePlayersData = _context.Players
                .Include(p => p.Team)
                .Include(p => p.Statistics)
                .Where(p => !p.IsActive)

                .OrderBy(ip => ip.PlyrFirstName) // Change to the actual property for sorting
                .Select(ip => new
                {
                    Member_ID = ip.PlyrMemberID,
                    First_Name = ip.PlyrFirstName,
                    Last_Name = ip.PlyrLastName,
                    Team = ip.Team.TmName,
                    Division = ip.Team.Division.DivName,
                })
                .AsNoTracking();


            // How many rows?
            int numRows = inactivePlayersData.Count();

            if (numRows > 0)
            {
                // Create a new spreadsheet from scratch.
                using (ExcelPackage excel = new())
                {
                    var workSheet = excel.Workbook.Worksheets.Add("Inactive Players Report");

                    // Note: Cells[row, column]
                    workSheet.Cells[3, 1].LoadFromCollection(inactivePlayersData, true);

                    // Set column styles if needed

                    // Make certain cells bold
                    workSheet.Cells[4, 1, numRows + 3, 1].Style.Font.Bold = true;

                    // Autofit columns
                    workSheet.Cells.AutoFitColumns();

                    // Add a title and timestamp at the top of the report
                    workSheet.Cells[1, 1].Value = "Inactive Players Report";
                    using (ExcelRange Rng = workSheet.Cells[1, 1, 1, 3]) // Adjust the column count accordingly
                    {
                        Rng.Merge = true;
                        Rng.Style.Font.Bold = true;
                        Rng.Style.Font.Size = 18;
                        Rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    }

                    DateTime utcDate = DateTime.UtcNow;
                    TimeZoneInfo esTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
                    DateTime localDate = TimeZoneInfo.ConvertTimeFromUtc(utcDate, esTimeZone);
                    using (ExcelRange Rng = workSheet.Cells[2, 6]) // Adjust the column accordingly
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
                        string filename = "InActive Players Report.xlsx";
                        string mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        return File(theData, mimeType, filename);
                    }
                    catch (Exception)
                    {
                        return BadRequest("Could not build and download the file.");
                    }
                }
            }
            PopulateDropDownLists();
            return RedirectToAction("InactiveIndex");
        }


        #region SelectLists
        private SelectList TeamSelectList(int? selectedId)
        {
            return new SelectList(_context.Teams.Where(t => t.IsActive), "ID", "TmName", selectedId);
        }

        private SelectList DivisionSelectList(int? selectedId)
        {
            return new SelectList(_context.Divisions, "ID", "DivName", selectedId);
        }


        private SelectList StatisticSelectList(int? selectedId)
        {
            return new SelectList(_context.Statistics, "ID", "ID", selectedId);
        }
        #endregion
        private void PopulateDropDownLists(Player player = null)
        {
            ViewData["TeamID"] = TeamSelectList(player?.TeamID);
            ViewData["DivisionID"] = DivisionSelectList(player?.Team?.DivisionID);
            ViewData["StatisticID"] = StatisticSelectList(player?.StatisticID);
        }

        [HttpGet]
        public async Task<IActionResult> GetTeamsByDivision(int divisionId)
        {
            var teams = await _context.Teams.Where(t => t.DivisionID == divisionId).ToListAsync();
            return Json(teams);
        }

        private bool PlayerExists(int id)
        {
            return _context.Players.Any(e => e.ID == id);
        }
    }
}
