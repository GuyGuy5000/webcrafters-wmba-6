using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using wmbaApp.CustomControllers;
using wmbaApp.Data;
using wmbaApp.Models;
using wmbaApp.Utilities;

namespace wmbaApp.Controllers
{
    [Authorize]
    public class TeamsController : ElephantController
    {
        private readonly WmbaContext _context;
        private readonly ApplicationDbContext _AppContext;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public TeamsController(WmbaContext context, ApplicationDbContext appContext, RoleManager<ApplicationRole> roleManager)
        {
            _context = context;
            _AppContext = appContext;
            _roleManager = roleManager;
        }

        // GET: Teams
        public async Task<IActionResult> Index(string SearchString, int? DivisionID,
             int? page, int? pageSizeID, string actionButton, string sortDirection = "asc", string sortField = "")
        {
            //Count the number of filters applied - start by assuming no filters
            ViewData["Filtering"] = " btn-outline-dark";
            int numberFilters = 0;

            if (TempData["SuccessMessage"] != null)
            {
                ViewBag.SuccessMessage = TempData["SuccessMessage"].ToString();
            }
            //Then in each "test" for filtering, add to the count of Filters applied

            //List of sort options.
            //NOTE: make sure this array has matching values to the column headings
            string[] sortOptions = new[] { "Team", "Division", "Coaches" };
            PopulateDropDownLists();

            IQueryable<Team> teams;

            if (User.IsInRole("Admin"))
            {
                teams = _context.Teams
                .Include(t => t.Division)
                .Include(t => t.DivisionCoaches).ThenInclude(t => t.Coach)
                .Include(t => t.Players)
                .Include(t => t.GameTeams).ThenInclude(t => t.Game)
                .Include(t => t.HomeGames)
                .Include(t => t.AwayGames)
                .Where(t => t.IsActive)
                .AsNoTracking();
            }
            else
            {
                var rolesTeamIDs = await UserRolesHelper.GetUserTeamIDs(_AppContext, User);
                var rolesDivisionIDs = await UserRolesHelper.GetUserDivisionIDs(_AppContext, User);

                teams = _context.Teams
                .Include(t => t.Division)
                .Include(t => t.DivisionCoaches).ThenInclude(t => t.Coach)
                .Include(t => t.Players)
                .Include(t => t.GameTeams).ThenInclude(t => t.Game)
                .Include(t => t.HomeGames)
                .Include(t => t.AwayGames)
                .Where(t => t.IsActive && (rolesTeamIDs.Contains(t.ID) || rolesDivisionIDs.Contains(t.DivisionID)))
                .AsNoTracking();
            }



            //Add as many filters as needed
            if (DivisionID.HasValue)
            {
                teams = teams.Where(t => t.DivisionID == DivisionID);
                numberFilters++;
            }

            if (!System.String.IsNullOrEmpty(SearchString))
            {
                teams = teams.Where(p => p.TmName.ToUpper().Contains(SearchString.ToUpper()));

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
                //Keep the Bootstrap collapse open
                //@ViewData["ShowFilter"] = " show";
            }

            //Before we sort, see if we have called for a change of filtering or sorting
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
            if (sortField == "Team")
            {

                if (sortDirection == "asc")
                {
                    teams = teams
                        .OrderBy(p => p.TmName);
                }
                else
                {
                    teams = teams
                        .OrderByDescending(p => p.TmName);
                }
            }
            else if (sortField == "Division")
            {
                if (sortDirection == "asc")
                {
                    teams = teams
                       .OrderBy(p => p.Division.DivName);

                }
                else
                {
                    teams = teams
                       .OrderByDescending(p => p.Division.DivName);

                }
            }

            //Set sort for next time
            ViewData["sortField"] = sortField;
            ViewData["sortDirection"] = sortDirection;

            int pageSize = PageSizeHelper.SetPageSize(HttpContext, pageSizeID, ControllerName());
            ViewData["pageSizeID"] = PageSizeHelper.PageSizeList(pageSize);
            var pagedData = await PaginatedList<Team>.CreateAsync(teams.AsNoTracking(), page ?? 1, pageSize);

            // Change the return statement to use the pagedData
            return View(pagedData);
        }

        // GET: Teams
        public async Task<IActionResult> InactiveIndex(string SearchString, int? DivisionID,
             int? page, int? pageSizeID, string actionButton, string sortDirection = "asc", string sortField = "")
        {
            //Count the number of filters applied - start by assuming no filters
            ViewData["Filtering"] = "btn-outline-dark";
            int numberFilters = 0;
            //Then in each "test" for filtering, add to the count of Filters applied
            if (TempData["SuccessMessage"] != null)
            {
                ViewBag.SuccessMessage = TempData["SuccessMessage"].ToString();
            }
            //List of sort options.
            //NOTE: make sure this array has matching values to the column headings
            string[] sortOptions = new[] { "Team", "Division", "Coaches" };
            PopulateDropDownLists();

            IQueryable<Team> teams;

            if (User.IsInRole("Admin"))
            {
                teams = _context.Teams
                .Include(t => t.Division)
                .Include(t => t.DivisionCoaches).ThenInclude(t => t.Coach)
                .Include(t => t.Players)
                .Include(t => t.GameTeams).ThenInclude(t => t.Game)
                .Include(t => t.HomeGames)
                .Include(t => t.AwayGames)
                .Where(t => !t.IsActive)
                .AsNoTracking();
            }
            else
            {
                var rolesTeamIDs = await UserRolesHelper.GetUserTeamIDs(_AppContext, User);
                var rolesDivisionIDs = await UserRolesHelper.GetUserDivisionIDs(_AppContext, User);

                teams = _context.Teams
                .Include(t => t.Division)
                .Include(t => t.DivisionCoaches).ThenInclude(t => t.Coach)
                .Include(t => t.Players)
                .Include(t => t.GameTeams).ThenInclude(t => t.Game)
                .Include(t => t.HomeGames)
                .Include(t => t.AwayGames)
                .Where(t => !t.IsActive && (rolesTeamIDs.Contains(t.ID) || rolesDivisionIDs.Contains(t.DivisionID)))
                .AsNoTracking();
            }

            //Add as many filters as needed
            if (DivisionID.HasValue)
            {
                teams = teams.Where(t => t.DivisionID == DivisionID);
                numberFilters++;
            }

            if (!System.String.IsNullOrEmpty(SearchString))
            {
                teams = teams.Where(p => p.TmName.ToUpper().Contains(SearchString.ToUpper()));

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
                //Keep the Bootstrap collapse open
                //@ViewData["ShowFilter"] = " show";
            }

            //Before we sort, see if we have called for a change of filtering or sorting
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
            if (sortField == "Team")
            {

                if (sortDirection == "asc")
                {
                    teams = teams
                        .OrderBy(p => p.TmName);
                }
                else
                {
                    teams = teams
                        .OrderByDescending(p => p.TmName);
                }
            }
            else if (sortField == "Division")
            {
                if (sortDirection == "asc")
                {
                    teams = teams
                       .OrderBy(p => p.Division.DivName);

                }
                else
                {
                    teams = teams
                       .OrderByDescending(p => p.Division.DivName);

                }
            }

            //Set sort for next time
            ViewData["sortField"] = sortField;
            ViewData["sortDirection"] = sortDirection;

            int pageSize = PageSizeHelper.SetPageSize(HttpContext, pageSizeID, ControllerName());
            ViewData["pageSizeID"] = PageSizeHelper.PageSizeList(pageSize);
            var pagedData = await PaginatedList<Team>.CreateAsync(teams.AsNoTracking(), page ?? 1, pageSize);

            // Change the return statement to use the pagedData
            return View(pagedData);
        }



        // GET: Teams/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Teams == null)
            {
                return NotFound();
            }
            if (TempData["SuccessMessage"] != null)
            {
                ViewBag.SuccessMessage = TempData["SuccessMessage"].ToString();
            }

            var team = await _context.Teams
           .Include(t => t.Division)
           .Include(t => t.DivisionCoaches).ThenInclude(t => t.Coach)
           .Include(t => t.Players)
           .Include(t => t.GameTeams).ThenInclude(t => t.Game)
           .AsNoTracking()
           .FirstOrDefaultAsync(m => m.ID == id);

            if (team == null)
            {
                return NotFound();
            }

            if (!await UserRolesHelper.IsAuthorizedForTeam(_AppContext, User, team))
                return RedirectToAction("Index", "Teams");

            return View(team);
        }

        // GET: Teams/Create
        [Authorize(Roles = "Admin,Convenor")]
        public IActionResult Create()
        {
            PopulateDropDownLists();
            return View();
        }

        // POST: Teams/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Convenor")]
        public async Task<IActionResult> Create([Bind("ID,TmName,TmAbbreviation,DivisionID")] Team team, int? coachID, string submitButton = "")
        {
            if (!await UserRolesHelper.IsAuthorizedForTeam(_AppContext, User, team))
                return RedirectToAction("Index", "Teams");

            PopulateDropDownLists();
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(team);
                    await _context.SaveChangesAsync();

                    // Set success message
                    TempData["SuccessMessage"] = "Team created successfully.";

                    if (coachID.HasValue)
                    {
                        var divisionCoach = new DivisionCoach() { DivisionID = team.DivisionID, CoachID = coachID.Value, TeamID = team.ID };

                        _context.DivisionCoaches.Add(divisionCoach);
                        team.DivisionCoaches.Add(divisionCoach);
                    }

                    await _context.SaveChangesAsync();
                    IdentityResult roleResult;
                    roleResult = await _roleManager.CreateAsync(new ApplicationRole(team.TmName, 0, team.ID));

                    if (!String.IsNullOrEmpty(submitButton))
                    {
                        if (submitButton == "Create and add players")
                            return RedirectToAction("Create", "PlayerTeam", new { TeamID = team.ID });
                        else
                            return RedirectToAction("Index", "PlayerTeam", new { TeamID = team.ID });
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
                        if (dex.InnerException.Message.Contains("TmName"))
                            ModelState.AddModelError("TmName", "A team with this name already exists. Please choose a different name."); //pass a message to the field that triggered the error
                        else if (dex.InnerException.Message.Contains("TmAbbreviation"))
                            ModelState.AddModelError("TmAbbreviation", "A team with this abbreviation already exists. Please choose a different abbreviation.");
                        else if (dex.InnerException.Message.Contains("DivisionCoaches"))
                            ModelState.AddModelError("DivisionCoaches", "This coach is already assigned to a team in this division. Please choose a different coach");
                    }
                    else
                        ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                    _context.Remove(team);
                    await _context.SaveChangesAsync();
                }
            }

            return View(team);
        }


        // GET: Teams/Edit/5 
        [Authorize(Roles = "Admin,Convenor")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Teams == null)
            {
                return NotFound();
            }

            var team = await _context.Teams
            .Include(t => t.Division)
            .Include(t => t.DivisionCoaches).ThenInclude(t => t.Coach)
            .Include(t => t.Players)
            .Include(t => t.GameTeams).ThenInclude(t => t.Game)
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.ID == id);
            if (team == null)
            {
                return NotFound();
            }

            if (!await UserRolesHelper.IsAuthorizedForTeam(_AppContext, User, team))
                return RedirectToAction("Index", "Teams");

            PopulateDropDownLists(team);

            return View(team);
        }

        // POST: Teams/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Convenor")]
        public async Task<IActionResult> Edit(int id, int? coachID)
        {
            var teamToUpdate = await _context.Teams
                .Include(t => t.Division)
                .Include(t => t.DivisionCoaches).ThenInclude(t => t.Coach)
                .Include(t => t.Players)
                .Include(t => t.AwayGames).ThenInclude(g => g.HomeLineup).ThenInclude(hl => hl.PlayerLineups)
                .Include(t => t.HomeGames).ThenInclude(g => g.HomeLineup).ThenInclude(hl => hl.PlayerLineups)
                .FirstOrDefaultAsync(m => m.ID == id);

            if (teamToUpdate == null)
            {
                return NotFound();
            }

            if (!await UserRolesHelper.IsAuthorizedForTeam(_AppContext, User, teamToUpdate))
                return RedirectToAction("Index", "Teams");

            PopulateDropDownLists(teamToUpdate);

            if (await TryUpdateModelAsync<Team>(teamToUpdate, "",
                t => t.TmName, t => t.DivisionID))
            {
                try
                {
                    if (coachID.HasValue)
                    {
                        var divisionCoachToUpdate = await _context.DivisionCoaches.FirstOrDefaultAsync(dc => dc.TeamID == teamToUpdate.ID);
                        var divisionCoach = teamToUpdate.DivisionCoaches.FirstOrDefault(dc => dc.TeamID == teamToUpdate.ID);

                        if (divisionCoachToUpdate != null)
                        {
                            _context.DivisionCoaches.Remove(divisionCoachToUpdate);
                            teamToUpdate.DivisionCoaches.Remove(divisionCoach);

                            _context.Update(teamToUpdate);
                            await _context.SaveChangesAsync();

                            var newDivisionCoach = new DivisionCoach() { DivisionID = teamToUpdate.DivisionID, CoachID = coachID.Value, TeamID = teamToUpdate.ID };

                            _context.DivisionCoaches.Add(newDivisionCoach);
                            teamToUpdate.DivisionCoaches.Add(newDivisionCoach);
                        }
                        else
                        {
                            var newDivisionCoach = new DivisionCoach() { DivisionID = teamToUpdate.DivisionID, CoachID = coachID.Value, TeamID = teamToUpdate.ID };

                            _context.DivisionCoaches.Add(newDivisionCoach);
                            teamToUpdate.DivisionCoaches.Add(newDivisionCoach);
                        }
                    }

                    _context.Update(teamToUpdate);
                    await _context.SaveChangesAsync();

                    var role = _roleManager.Roles.FirstOrDefault(r => r.TeamID == teamToUpdate.ID);
                    if (role != null)
                    {
                        role.Name = teamToUpdate.TmName;
                        role.NormalizedName = teamToUpdate.TmName.ToUpper();
                        _AppContext.Roles.Update(role);
                        await _AppContext.SaveChangesAsync();
                    }

                    //get all upcoming games that contain that team
                    var gamesToRemove = _context.Games
                        .Include(g => g.AwayTeam)
                        .Include(g => g.HomeTeam)
                        .Where(g => g.GameEndTime > DateTime.Now && (g.HomeTeamID == teamToUpdate.ID || g.AwayTeamID == teamToUpdate.ID));

                    foreach (var game in gamesToRemove)
                    {
                        //if the divisions don't match, remove the game
                        if (game.HomeTeam.DivisionID != game.AwayTeam.DivisionID)
                            _context.Games.Remove(game);
                    }
                    await _context.SaveChangesAsync();

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TeamExists(teamToUpdate.ID))
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
                        if (dex.InnerException.Message.Contains("TmName"))
                            ModelState.AddModelError("TmName", "A team with this name already exists. Please choose a different name."); //pass a message to the field that triggered the error
                        else if (dex.InnerException.Message.Contains("TmAbbreviation"))
                            ModelState.AddModelError("TmAbbreviation", "A team with this abbreviation already exists. Please choose a different abbreviation.");
                        else if (dex.InnerException.Message.Contains("DivisionCoaches"))
                            ModelState.AddModelError("DivisionCoaches", "This coach is already assigned to a team in this division. Please choose a different coach");
                    }
                    else
                        ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                    return View(teamToUpdate);
                }
            }
            ViewData["DivisionID"] = new SelectList(_context.Divisions, "ID", "DivName", teamToUpdate.DivisionID);
            TempData["SuccessMessage"] = "Team updated successfully.";
            return RedirectToAction("Details", new { teamToUpdate.ID });
        }

   
        // GET: Teams/Inactive/5
        [Authorize(Roles = "Admin,Convenor,Coach")]
        public async Task<IActionResult> MakeInactive(int? id)
        {
            if (id == null || _context.Teams == null)
                return NotFound();
            if (TempData["SuccessMessage"] != null)
            {
                ViewBag.SuccessMessage = TempData["SuccessMessage"].ToString();
            }

            var team = await _context.Teams
                     .Include(t => t.Division)
                     .Include(t => t.DivisionCoaches).ThenInclude(t => t.Coach)
                     .Include(t => t.Players)
                     .Include(t => t.GameTeams).ThenInclude(t => t.Game)
                     .AsNoTracking()
                     .FirstOrDefaultAsync(m => m.ID == id);

            if (team == null)
                return NotFound();

            if (!await UserRolesHelper.IsAuthorizedForTeam(_AppContext, User, team))
                return RedirectToAction("Index", "Teams");

            return View(team);
        }

        // POST: Teams/Inactive/5
        [HttpPost, ActionName("MakeInactive")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Convenor,Coach")]
        public async Task<IActionResult> MakeInactiveConfirmed(int id, string deactivate)
        {
            if (_context.Teams == null)
            {
                return Problem("Entity set 'WmbaContext.Players' is null.");
            }

            var team = await _context.Teams
                     .Include(t => t.Division)
                     .Include(t => t.DivisionCoaches).ThenInclude(t => t.Coach)
                     .Include(t => t.Players)
                     .Include(t => t.HomeGames)
                     .Include(t => t.AwayGames)
                     .AsNoTracking()
                     .FirstOrDefaultAsync(m => m.ID == id);

            if (!await UserRolesHelper.IsAuthorizedForTeam(_AppContext, User, team))
                return RedirectToAction("Index", "Teams");

            if (deactivate == "Deactivate team and active players")
            {
                if (team.HomeGames.Any(g => g.GameEndTime > DateTime.Now) || team.AwayGames.Any(g => g.GameEndTime > DateTime.Now))
                {
                    ModelState.AddModelError("FK2", $"Unable to make a team that has upcoming games inactive. Reassign or cancel the upcoming games for this team");
                }
                else
                {
                    foreach (Player p in team.Players)
                    {
                        if (p.IsActive)
                            p.IsActive = false;
                        _context.Players.Update(p);
                    }
                    if (team != null)
                    {
                        team.IsActive = false;
                        _context.Teams.Update(team);
                    }

                    await _context.SaveChangesAsync();
                    var role = _roleManager.Roles.FirstOrDefault(r => r.TeamID == team.ID);
                    if (role != null)
                    {
                        _AppContext.Roles.Remove(role);
                        await _AppContext.SaveChangesAsync();
                    }

                    return RedirectToAction(nameof(Index));
                }
            }
            else
            {
                int activePlayers = 0;
                foreach (Player p in team.Players)
                {
                    if (p.IsActive)
                        activePlayers++;
                }
                if (activePlayers > 0)
                {
                    ModelState.AddModelError("FK1", $"Unable to make a team that has active players inactive. Reassign the players assigned to this team, or make all active players inactive.");
                }

                int activeGames = 0;
                foreach (Game g in team.HomeGames)
                {
                    if (g.GameEndTime > DateTime.Now)
                    {
                        activeGames++;
                    }
                }
                foreach (Game g in team.AwayGames)
                {
                    if (g.GameEndTime > DateTime.Now)
                    {
                        activeGames++;
                    }
                }
                if (activeGames > 0)
                {
                    ModelState.AddModelError("FK2", $"Unable to make a team that has upcoming games inactive. Reassign or cancel the upcoming games for this team");
                }

                if (ModelState.IsValid)
                {
                    if (team != null)
                    {
                        team.IsActive = false;
                        _context.Teams.Update(team);
                    }

                    await _context.SaveChangesAsync();
                    var teamrole = _roleManager.Roles.FirstOrDefault(r => r.TeamID == team.ID);
                    if (teamrole != null)
                    {
                        _AppContext.Roles.Remove(teamrole);
                        await _AppContext.SaveChangesAsync();
                    }
                    return RedirectToAction(nameof(Index));
                }
                return View(team);
            }

            return View();
        }

        // GET: Teams/Active/5
        [Authorize(Roles = "Admin,Convenor,Coach")]
        public async Task<IActionResult> MakeActive(int? id)
        {
            if (id == null || _context.Teams == null)
                return NotFound();

            var team = await _context.Teams
                     .Include(t => t.Division)
                     .Include(t => t.DivisionCoaches).ThenInclude(t => t.Coach)
                     .Include(t => t.Players)
                     .Include(t => t.GameTeams).ThenInclude(t => t.Game)
                     .AsNoTracking()
                     .FirstOrDefaultAsync(m => m.ID == id);

            if (team == null)
                return NotFound();

            if (!await UserRolesHelper.IsAuthorizedForTeam(_AppContext, User, team))
                return RedirectToAction("Index", "Teams");

            return View(team);
        }

        // POST: Teams/Active/5
        [HttpPost, ActionName("MakeActive")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Convenor,Coach")]
        public async Task<IActionResult> MakeActiveConfirmed(int id)
        {
            if (_context.Teams == null)
            {
                return Problem("Entity set 'WmbaContext.Players' is null.");
            }

            var team = await _context.Teams
                     .Include(t => t.Division)
                     .Include(t => t.DivisionCoaches).ThenInclude(t => t.Coach)
                     .Include(t => t.Players)
                     .Include(t => t.GameTeams).ThenInclude(t => t.Game)
                     .AsNoTracking()
                     .FirstOrDefaultAsync(m => m.ID == id);

            if (team != null)
            {
                team.IsActive = true;
                _context.Teams.Update(team);
            }

            if (!await UserRolesHelper.IsAuthorizedForTeam(_AppContext, User, team))
                return RedirectToAction("Index", "Teams");

            await _context.SaveChangesAsync();
            IdentityResult roleResult;
            roleResult = await _roleManager.CreateAsync(new ApplicationRole(team.TmName, 0, team.ID));
            return RedirectToAction(nameof(Index));
        }

        #region SelectLists
        private SelectList DivisionSelectList(int? selectedId)
        {
            return new SelectList(_context.Divisions, "ID", "DivName", selectedId);
        }
        private SelectList CoachesSelectList(int? selectedId)
        {
            return new SelectList(_context.Coaches, "ID", "FullName", selectedId);
        }
        private void PopulateDropDownLists(Team team = null)
        {
            ViewData["DivisionID"] = DivisionSelectList(team?.DivisionID);
            ViewData["CoachID"] = CoachesSelectList(team?.DivisionCoaches?.FirstOrDefault()?.CoachID);
        }
        #endregion

        private bool TeamExists(int id)
        {
            return _context.Teams.Any(e => e.ID == id);
        }
    }
}
