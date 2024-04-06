using wmbaApp.CustomControllers;
using wmbaApp.Data;
using wmbaApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using wmbaApp.Data.Migrations;
using wmbaApp.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using NuGet.Packaging;
using wmbaApp.Utilities;

namespace wmbaApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserRoleController : CognizantController
    {
        private readonly ApplicationDbContext _context;
        private readonly WmbaContext _WmbaContext;
        private readonly UserManager<IdentityUser> _userManager;

        public UserRoleController(ApplicationDbContext context, UserManager<IdentityUser> userManager, WmbaContext wmbaContext)
        {
            _context = context;
            _userManager = userManager;
            _WmbaContext = wmbaContext;
        }


        // GET: User
        public async Task<IActionResult> Index(int? page, int? pageSizeID, string SearchString,
             string actionButton, string sortDirection = "asc", string sortField = "")
        {

            PopulateDropDownLists();
            //Count the number of filters applied - start by assuming no filters
            ViewData["Filtering"] = " btn-outline-dark";
            int numberFilters = 0;
            //Then in each "test" for filtering, add to the count of Filters applied

            //List of sort options.
            //NOTE: make sure this array has matching values to the column headings
            string[] sortOptions = new[] { "UserName", "UserRoles" };

            var users = _context.Users
                            .OrderBy(u => u.UserName)
                            .Select(u => new UserVM
                            {
                                ID = u.Id,
                                UserName = u.UserName,
                                UserRoles = new List<string>(
                                    _userManager.GetRolesAsync(_userManager.FindByIdAsync(u.Id).Result).Result
                                )
                            });


            //Give feedback about the state of the filters
            if (numberFilters != 0)
            {
                //Toggle the Open/Closed state of the collapse depending on if we are filtering
                ViewData["Filtering"] = " btn-outline-dark";
                //Show how many filters have been applied
                ViewData["numberFilters"] = "(" + numberFilters.ToString()
                    + " Filter" + (numberFilters > 1 ? "s" : "") + " Applied)";
            }


            if (!System.String.IsNullOrEmpty(SearchString))
            {
                var upperSearchString = SearchString.ToUpper();
                users = users.Where(u => u.UserName.ToUpper().Contains(upperSearchString));
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
            if (sortField == "UserName")
                if (sortDirection == "asc")
                {
                    users = users
                    .OrderBy(u => u.UserName);
                }
                else
                {
                    users = users
                    .OrderByDescending(u => u.UserName);
                }


            ViewData["sortField"] = sortField;
            ViewData["sortDirection"] = sortDirection;


            int pageSize = PageSizeHelper.SetPageSize(HttpContext, pageSizeID, ControllerName());
            ViewData["pageSizeID"] = PageSizeHelper.PageSizeList(pageSize);

            var pagedData = await PaginatedList<UserVM>.CreateAsync(users.AsQueryable(), page ?? 1, pageSize);

            return View(pagedData);
        }



        #region Edit
        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new BadRequestResult();
            }
            var _user = await _userManager.FindByIdAsync(id);//IdentityRole
            if (_user == null)
            {
                return NotFound();
            }


            // Get the currently authenticated user
            var currentUser = await _userManager.GetUserAsync(User);
            // Check if the user being edited is the same as the currently authenticated user
            if (_user.Id == currentUser.Id)
            {
                // You can handle this according to your application's logic.
                // For example, redirect to an error page or display an error message.
                return RedirectToAction("Index");
            }

            UserVM user = new UserVM
            {
                ID = _user.Id,
                UserName = _user.UserName,
                UserRoles = (List<string>)await _userManager.GetRolesAsync(_user)
            };
            PopulateAssignedRoleData(user);
            PopulateConvenorRoles(user);
            return View(user);
        }

        // POST: Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string Id, string[] selectedRoles)
        {
            var _user = await _userManager.FindByIdAsync(Id);   //IdentityRole
            UserVM user = new UserVM
            {
                ID = _user.Id,
                UserName = _user.UserName,
                UserRoles = (List<string>)await _userManager.GetRolesAsync(_user)
            };

            var teams = new List<string>();
            if (selectedRoles.Contains("Coach") || selectedRoles.Contains("ScoreKeeper"))
                teams = (List<string>)(await _userManager.GetRolesAsync(_user))
                                                    .Where(r => r != "Admin"
                                                    && r != "Convenor"
                                                    && r != "Coach"
                                                    && r != "ScoreKeeper"
                                                    && !r.Contains("U Convenor")).ToList();

            List<string> allSelectedRoles = new List<string>(selectedRoles);

            allSelectedRoles.AddRange(teams);

            try
            {
                await UpdateUserRoles(allSelectedRoles.ToArray(), user);
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty,
                                "Unable to save changes.");
            }

            PopulateAssignedRoleData(user);
            PopulateConvenorRoles(user);
            return View(user);
        }
        #endregion

        #region AssignTeams
        // GET: Users/AssignTeams/5
        public async Task<IActionResult> AssignTeams(string id)
        {
            if (id == null)
            {
                return new BadRequestResult();
            }
            var _user = await _userManager.FindByIdAsync(id);//IdentityRole
            if (_user == null)
            {
                return NotFound();
            }

            // Get the currently authenticated user
            var currentUser = await _userManager.GetUserAsync(User);
            // Check if the user being edited is the same as the currently authenticated user
            if (_user.Id == currentUser.Id)
            {
                // You can handle this according to your application's logic.
                // For example, redirect to an error page or display an error message.
                return RedirectToAction("Index");
            }

            UserVM user = new UserVM
            {
                ID = _user.Id,
                UserName = _user.UserName,
                UserRoles = (List<string>)await _userManager.GetRolesAsync(_user)
            };

            PopulateTeamRoles(user);
            return View(user);
        }

        // POST: Users/AssignTeams/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignTeams(string Id, string[] selectedTeams)
        {
            var _user = await _userManager.FindByIdAsync(Id);   //IdentityRole
            UserVM user = new UserVM
            {
                ID = _user.Id,
                UserName = _user.UserName,
                UserRoles = (List<string>)await _userManager.GetRolesAsync(_user)
            };
            var teams = ((List<string>)await _userManager.GetRolesAsync(_user))
                                                            .Where(r => r == "Admin"
                                                            || r == "Convenor"
                                                            || r == "Coach"
                                                            || r == "ScoreKeeper"
                                                            || r.Contains("U Convenor"));
            List<string> allSelectedTeams = new List<string>(selectedTeams);
            allSelectedTeams.AddRange(teams);

            try
            {
                await UpdateUserRoles(allSelectedTeams.ToArray(), user);
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "Unable to save changes.");
            }

            PopulateTeamRoles(user);
            return View(user);
        }
        #endregion

        private void PopulateConvenorRoles(UserVM user)
        {
            // Prepare checkboxes for additional roles for Convenor
            var additionalRolesForConvenor = _context.Roles.Where(r => r.Name.Contains("U Convenor"));
            var currentRoles = user.UserRoles;
            var viewModel = new List<RoleVM>();

            foreach (var r in additionalRolesForConvenor)
            {
                viewModel.Add(new RoleVM
                {
                    RoleID = r.Id,
                    RoleName = r.Name,
                    Assigned = currentRoles.Contains(r.Name)
                });
            }
            ViewBag.ConvenorExtras = viewModel;
        }

        //|| r.Name == "9U Convenor" || r.Name == "11U Convenor"
        //   || r.Name == "13U Convenor" || r.Name == "15U Convenor" || r.Name == "18U Convenor"

        private void PopulateAssignedRoleData(UserVM user)
        {
            //Prepare checkboxes for all Roles
            var allRoles = _context.Roles
                                .Where(r => r.Name == "Admin"
                                || r.Name == "Convenor"
                                || r.Name == "ScoreKeeper"
                                || r.Name == "Coach")
                                .ToList<ApplicationRole>();
            var currentRoles = user.UserRoles;
            var viewModel = new List<RoleVM>();
            foreach (var r in allRoles)
            {
                viewModel.Add(new RoleVM
                {
                    RoleID = r.Id,
                    RoleName = r.Name,
                    Assigned = currentRoles.Contains(r.Name)
                });
            }
            ViewBag.Roles = viewModel;
        }

        private void PopulateTeamRoles(UserVM user)
        {
            //get all divisions and include all active teams
            var allDivisions = _WmbaContext.Divisions
                                    .Include(d => d.Teams.Where(t => t.IsActive))
                                    .ToList<Division>();
            //get all team roles
            var allTeamRoles = _context.Roles
                                .Where(r => r.Name == "Admin"
                                || r.Name == "Convenor"
                                || r.Name == "ScoreKeeper"
                                || r.Name == "Coach"
                                || !r.Name.Contains("U Convenor"));
            var currentRoles = user.UserRoles;

            var divisionsList = new List<Division>();
            foreach (Division d in allDivisions)
            {
                //create new viewModel to hold teams in d
                var viewModel = new List<RoleVM>();

                //for every team in this division
                foreach (Team t in d.Teams)
                {
                    //if the team roles list contains the team
                    if (allTeamRoles.Any(tr => tr.TeamID == t.ID))
                    {
                        viewModel.Add(new RoleVM
                        {
                            RoleID = t.ID.ToString(),
                            RoleName = t.TmName,
                            Assigned = currentRoles.Contains(t.TmName)
                        });
                    }
                }
                //create a dynamic ViewData key that holds all teams in that division
                string selectedRole = viewModel.FirstOrDefault(r => r.Assigned)?.RoleName;


                ViewData[$"{d.DivName}"] = new SelectList(viewModel, "RoleName", "RoleName", selectedRole);
                divisionsList.Add(d);
            }
            //Holds all divisons to be looped through in the view
            ViewData[$"DivisionsList"] = divisionsList;
        }

        private async Task UpdateUserRoles(string[] selectedRoles, UserVM userToUpdate)
        {
            var UserRoles = userToUpdate.UserRoles;//Current roles use is in
            var _user = await _userManager.FindByIdAsync(userToUpdate.ID);//IdentityUser

            if (selectedRoles == null)
            {
                //No roles selected so just remove any currently assigned
                foreach (var r in UserRoles)
                {
                    await _userManager.RemoveFromRoleAsync(_user, r);
                }
            }
            else
            {
                //At least one role checked so loop through all the roles
                //and add or remove as required

                //We need to do this next line because foreach loops don't always work well
                //for data returned by EF when working async.  Pulling it into an IList<>
                //first means we can safely loop over the colleciton making async calls and avoid
                //the error 'New transaction is not allowed because there are other threads running in the session'
                IList<ApplicationRole> allRoles = _context.Roles.ToList<ApplicationRole>();

                foreach (var r in allRoles)
                {
                    if (selectedRoles.Contains(r.Name))
                    {
                        if (!UserRoles.Contains(r.Name))
                        {
                            await _userManager.AddToRoleAsync(_user, r.Name);
                        }
                    }
                    else
                    {
                        if (UserRoles.Contains(r.Name))
                        {
                            await _userManager.RemoveFromRoleAsync(_user, r.Name);
                        }
                    }
                }
            }
        }

        private void PopulateDropDownLists(UserVM user = null)
        {
            int? userId = user?.ID != null ? int.Parse(user.ID) : null;
            ViewData["ID"] = UserSelectList(userId);
        }

        private SelectList UserSelectList(int? selectedId)
        {
            return new SelectList(_context.Users, "ID", "UserName", selectedId);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
                _userManager.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
