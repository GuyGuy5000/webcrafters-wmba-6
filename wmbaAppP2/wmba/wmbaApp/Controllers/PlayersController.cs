using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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
        private readonly ApplicationDbContext _AppContext;

        public PlayersController(WmbaContext context, ApplicationDbContext appContext)
        {
            _context = context;
            _AppContext = appContext;

        }

        // GET: Players
        public async Task<IActionResult> Index(string SearchString, int? TeamID,
             int? page, int? pageSizeID, string actionButton, string sortDirection = "asc", string sortField = "")
        {
            //Count the number of filters applied - start by assuming no filters
            ViewData["Filtering"] = " btn-outline-dark";
            int numberFilters = 0;
            //Then in each "test" for filtering, add to the count of Filters applied

            //List of sort options.
            //NOTE: make sure this array has matching values to the column headings
            string[] sortOptions = new[] { "Name", "Team" };

            CreatePopulateDropDownLists();

            IQueryable<Player> players;

            if (User.IsInRole("Admin"))
            {
                players = _context.Players
                .Include(t => t.Team).ThenInclude(t => t.Division)
                .Include(t => t.Statistics)
                .Where(t => t.IsActive)
                .AsNoTracking();
            }
            else
            {
                var rolesTeamIDs = await UserRolesHelper.GetUserTeamIDs(_AppContext, User);
                var rolesDivisionIDs = await UserRolesHelper.GetUserDivisionIDs(_AppContext, User);

                players = _context.Players
                .Include(t => t.Team).ThenInclude(t => t.Division)
                .Include(t => t.Statistics)
                .Where(t => t.IsActive && (rolesTeamIDs.Contains(t.TeamID) || rolesDivisionIDs.Contains(t.Team.DivisionID)))
                .AsNoTracking();
            }

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
                                       || p.Team.TmName.ToUpper().Contains(SearchString.ToUpper()));

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
        [Authorize(Roles = "Admin,Convenor,Coach")]
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

            CreatePopulateDropDownLists();

            IQueryable<Player> players;

            if (User.IsInRole("Admin"))
            {
                players = _context.Players
                .Include(t => t.Team).ThenInclude(t => t.Division)
                .Include(t => t.Statistics)
                .Where(t => !t.IsActive)
                .AsNoTracking();
            }
            else
            {
                var rolesTeamIDs = await UserRolesHelper.GetUserTeamIDs(_AppContext, User);
                var rolesDivisionIDs = await UserRolesHelper.GetUserDivisionIDs(_AppContext, User);

                players = _context.Players
                .Include(t => t.Team).ThenInclude(t => t.Division)
                .Include(t => t.Statistics)
                .Where(t => !t.IsActive && (rolesTeamIDs.Contains(t.TeamID) || rolesDivisionIDs.Contains(t.Team.DivisionID)))
                .AsNoTracking();
            }

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

            if (!await UserRolesHelper.IsAuthorizedForPlayer(_AppContext, User, player))
                return RedirectToAction("Index", "Players");

            return View(player);
        }

        // GET: Players/Create
        [Authorize(Roles = "Admin,Convenor,Coach")]
        public IActionResult Create()
        {
            Player player = new Player();
            CreatePopulateDropDownLists(player);
            return View(player);
        }

        // POST: Players/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Convenor,Coach")]
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

            CreatePopulateDropDownLists(player);
            return View(player);
        }

        // GET: Players/Edit/5
        [Authorize(Roles = "Admin,Convenor,Coach")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Players == null)
            {
                return NotFound();
            }

            var player = await _context.Players
                .Include(p => p.Team).ThenInclude(p => p.Division)
                .FirstOrDefaultAsync(f => f.ID == id);

            if (player == null)
            {
                return NotFound();
            }

            if (!await UserRolesHelper.IsAuthorizedForPlayer(_AppContext, User, player))
                return RedirectToAction("Index", "Players");

            EditPopulateDropDownLists(player);
            return View(player);
        }

        // POST: Players/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Convenor,Coach")]
        public async Task<IActionResult> Edit(int id)
        {
            var playerToUpdate = await _context.Players
            .FirstOrDefaultAsync(m => m.ID == id);

            if (playerToUpdate == null)
            {
                return NotFound();
            }

            if (!await UserRolesHelper.IsAuthorizedForPlayer(_AppContext, User, playerToUpdate))
                return RedirectToAction("Index", "Players");

            if (await TryUpdateModelAsync<Player>(playerToUpdate, "",
                t => t.PlyrFirstName, t => t.PlyrLastName, t => t.TeamID, t => t.PlyrJerseyNumber, t => t.PlyrMemberID,
                t => t.StatisticID))
            {
                try
                {
                    playerToUpdate.PlyrJerseyNumber = null;
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
            EditPopulateDropDownLists(playerToUpdate);
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
        [Authorize(Roles = "Admin,Convenor,Coach")]
        public async Task<IActionResult> MakeInactive(int? id)
        {
            if (id == null || _context.Players == null)
                return NotFound();

            var player = await _context.Players
                .Include(p => p.Team)
                .Include(p => p.Statistics)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (player == null)
                return NotFound();

            return View(player);
        }

        // POST: Players/Inactive/5
        [HttpPost, ActionName("MakeInactive")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Convenor,Coach")]
        public async Task<IActionResult> MakeInactiveConfirmed(int id)
        {
            if (_context.Players == null)
            {
                return Problem("Entity set 'WmbaContext.Players' is null.");
            }

            var player = await _context.Players
                .Include(p => p.Team)
                .Include(p => p.Statistics)
                .FirstOrDefaultAsync(p => p.ID == id);

            if (player != null)
            {
                player.IsActive = false;
                _context.Players.Update(player);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        // GET: Players/Active/5
        [Authorize(Roles = "Admin,Convenor,Coach")]
        public async Task<IActionResult> MakeActive(int? id)
        {
            if (id == null || _context.Players == null)
                return NotFound();

            var player = await _context.Players
                .Include(p => p.Team)
                .Include(p => p.Statistics)
                .FirstOrDefaultAsync(m => m.ID == id);

            if (player == null)
                return NotFound();

            if (!await UserRolesHelper.IsAuthorizedForPlayer(_AppContext, User, player))
                return RedirectToAction("Index", "Players");

            return View(player);
        }

        // POST: Players/Active/5
        [HttpPost, ActionName("MakeActive")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Convenor,Coach")]
        public async Task<IActionResult> MakeActiveConfirmed(int id)
        {
            if (_context.Players == null)
            {
                return Problem("Entity set 'WmbaContext.Players' is null.");
            }
            var player = await _context.Players
                .Include(p => p.Team)
                .Include(p => p.Statistics)
                .FirstOrDefaultAsync(p => p.ID == id);

            if (player != null)
            {
                if (!await UserRolesHelper.IsAuthorizedForPlayer(_AppContext, User, player))
                    return RedirectToAction("Index", "Players");

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

            await _context.SaveChangesAsync();
            return RedirectToAction("InactiveIndex");
        }

        [Authorize(Roles = "Admin,Convenor,Coach")]
        public async Task<IActionResult> DownloadInactivePlayersReport()
        {

            IQueryable<dynamic> inactivePlayersData;

            if (User.IsInRole("Admin"))
            {
                inactivePlayersData = _context.Players
                .Include(t => t.Team).ThenInclude(t => t.Division)
                .Include(t => t.Statistics)
                .Where(t => t.IsActive)
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
            }
            else
            {
                var rolesTeamIDs = await UserRolesHelper.GetUserTeamIDs(_AppContext, User);
                var rolesDivisionIDs = await UserRolesHelper.GetUserDivisionIDs(_AppContext, User);

                inactivePlayersData = _context.Players
                .Include(t => t.Team).ThenInclude(t => t.Division)
                .Include(t => t.Statistics)
                .Where(t => t.IsActive && (rolesTeamIDs.Contains(t.TeamID) || rolesDivisionIDs.Contains(t.Team.DivisionID)))
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
            }



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
            CreatePopulateDropDownLists();
            return RedirectToAction("InactiveIndex");
        }


        #region SelectLists
        private SelectList TeamSelectList(int? selectedId)
        {
            return new SelectList(_context.Teams.Where(t => t.IsActive), "ID", "TmName", selectedId);
        }

        private SelectList EditDivisionSelectList(int? selectedId)
        {
            var divisions = _context.Divisions.OrderBy(d => d.ID).ToList();
            var currentDivsion = divisions.FirstOrDefault(d => d.ID == selectedId);
            var nextDivision = _context.Divisions.FirstOrDefault(d => d.ID > selectedId);
            List<Division> divisionList = new List<Division>();
            if (currentDivsion != null)
            {
                divisionList.Add(currentDivsion);
            }

            if (nextDivision != null)
            {
                divisionList.Add(nextDivision);
            }

            return new SelectList(divisionList, "ID", "DivName", selectedId);
        }

        private SelectList CreateDivisionSelectList(int? selectedId)
        {
            return new SelectList(_context.Divisions, "ID", "DivName", selectedId);
        }


        private SelectList StatisticSelectList(int? selectedId)
        {
            return new SelectList(_context.Statistics, "ID", "ID", selectedId);
        }
        #endregion
        private void EditPopulateDropDownLists(Player player = null)
        {
            ViewData["TeamID"] = TeamSelectList(player?.TeamID);
            ViewData["EditDivisionID"] = EditDivisionSelectList(player?.Team?.DivisionID);
            ViewData["StatisticID"] = StatisticSelectList(player?.StatisticID);
        }

        private void CreatePopulateDropDownLists(Player player = null)
        {
            ViewData["TeamID"] = TeamSelectList(player?.TeamID);
            ViewData["StatisticID"] = StatisticSelectList(player?.StatisticID);
            ViewData["CreateDivisionID"] = CreateDivisionSelectList(player?.Team?.DivisionID);
        }

        [HttpGet]
        public JsonResult GetTeamsByDivision(int divisionId)
        {
            var teams = _context.Teams
                .Include(t => t.Division)
                .Where(t => t.DivisionID == divisionId)
                .Select(t => new { value = t.ID, text = t.TmName })
                .ToList();

            return Json(teams);
        }


        //#region PositionCheckboxes
        //private void PopulateAssignedPositionCheckboxes(Player player)
        //{
        //    //For this to work, you must have Included the FunctionRooms 
        //    //in the Function
        //    var allOptions = _context.Positions;
        //    var currentOptionIDs = new HashSet<int>(player.PlayerPositions.Select(b => b.PositionID));
        //    var checkBoxes = new List<CheckOptionVM>();
        //    foreach (var option in allOptions)
        //    {
        //        checkBoxes.Add(new CheckOptionVM
        //        {
        //            ID = option.ID,
        //            DisplayText = option.PosName,
        //            Assigned = currentOptionIDs.Contains(option.ID)
        //        });
        //    }
        //    ViewData["PositionOptions"] = checkBoxes;
        //}

        ////For the posiionList
        //private void PopulateAssignedPositionLists(Player player)
        //{
        //    //For this to work, you must have Included the child collection in the parent object
        //    var allOptions = _context.Positions;
        //    var currentOptionsHS = new HashSet<int>(player.PlayerPositions.Select(b => b.PositionID));
        //    //Instead of one list with a boolean, we will make two lists
        //    var selected = new List<ListOptionVM>();
        //    var available = new List<ListOptionVM>();
        //    foreach (var r in allOptions)
        //    {
        //        if (currentOptionsHS.Contains(r.ID))
        //        {
        //            selected.Add(new ListOptionVM
        //            {
        //                ID = r.ID,
        //                DisplayText = r.PosName
        //            });
        //        }
        //        else
        //        {
        //            available.Add(new ListOptionVM
        //            {
        //                ID = r.ID,
        //                DisplayText = r.PosName
        //            });
        //        }
        //    }

        //    ViewData["selOpts"] = new MultiSelectList(selected.OrderBy(s => s.DisplayText), "ID", "DisplayText");
        //    ViewData["availOpts"] = new MultiSelectList(available.OrderBy(s => s.DisplayText), "ID", "DisplayText");
        //}

        //private void UpdatePlayerPositionsListboxes(string[] selectedOptions, Player playerToUpdate)
        //{
        //    if (selectedOptions == null)
        //    {
        //        playerToUpdate.PlayerPositions = new List<PlayerPosition>();
        //        return;
        //    }

        //    var selectedOptionsHS = new HashSet<string>(selectedOptions);
        //    var currentOptionsHS = new HashSet<int>(playerToUpdate.PlayerPositions.Select(b => b.PositionID));
        //    foreach (var r in _context.Positions)
        //    {
        //        if (selectedOptionsHS.Contains(r.ID.ToString()))//it is selected
        //        {
        //            if (!currentOptionsHS.Contains(r.ID))
        //            {
        //                playerToUpdate.PlayerPositions.Add(new PlayerPosition
        //                {
        //                    PositionID = r.ID,
        //                    PlayerID = playerToUpdate.ID
        //                });
        //            }
        //        }
        //        else //not selected
        //        {
        //            if (currentOptionsHS.Contains(r.ID))
        //            {
        //                PlayerPosition positionToRemove = playerToUpdate.PlayerPositions.FirstOrDefault(d => d.PositionID == r.ID);
        //                _context.Remove(positionToRemove);
        //            }
        //        }
        //    }
        //}
        //#endregion
        private bool PlayerExists(int id)
        {
            return _context.Players.Any(e => e.ID == id);
        }
    }
}
