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
        public async Task<IActionResult> Index()
        {
            var users = await (from u in _context.Users
                               .OrderBy(u => u.UserName)
                               select new UserVM
                               {
                                   ID = u.Id,
                                   UserName = u.UserName
                               })
                               .ToListAsync();
            foreach (var u in users)
            {
                var _user = await _userManager.FindByIdAsync(u.ID);
                u.UserRoles = (List<string>)await _userManager.GetRolesAsync(_user);
                u.UserRoles = u.UserRoles
                                .Where(r => r == "Admin"
                                    || r == "Convenor"
                                    || r == "Coach"
                                    || r == "ScoreKeeper"
                                    || r.EndsWith("U Convenor"))
                                .OrderBy(r => r)
                                .ToList();
                //Note: we needed the explicit cast above because GetRolesAsync() returns an IList<string>
            };

            return View(users.OrderBy(u => u.UserRoles.FirstOrDefault(ur => ur == "Admin" || ur == "Convenor" || ur == "Coach" || ur == "ScoreKeeper")));
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

            var teams = ((List<string>)await _userManager.GetRolesAsync(_user))
                                                .Where(r => r != "Admin"
                                                && r != "Convenor"
                                                && r != "Coach"
                                                && r != "ScoreKeeper"
                                                && !r.Contains("U Convenor"));
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
                ModelState.AddModelError(string.Empty,
                                "Unable to save changes.");
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
                IList<ApplicationRole> allRoles = _context.Roles.Where(r => r.Name == "Admin" || r.Name == "Convenor" || r.Name == "ScoreKeeper"
           || r.Name == "Coach" || r.Name == "9U Convenor" || r.Name == "11U Convenor"
           || r.Name == "13U Convenor" || r.Name == "15U Convenor" || r.Name == "18U Convenor" || r.Name.EndsWith(" Convenor")).ToList<ApplicationRole>();

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
