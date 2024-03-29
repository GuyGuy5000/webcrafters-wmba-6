using wmbaApp.CustomControllers;
using wmbaApp.Data;
using wmbaApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using wmbaApp.Data.Migrations;
using wmbaApp.Models;

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
                u.UserRoles.Sort();
                //Note: we needed the explicit cast above because GetRolesAsync() returns an IList<string>
            };

            return View(users.OrderBy(u => u.UserRoles.FirstOrDefault()));
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
            try
            {
                await UpdateUserRoles(selectedRoles, user);
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

            PopulateAssignedTeamData(user);
            return View(user);
        }

        // POST: Users/AssignTeams/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignTeams(string Id, string[] selectedRoles)
        {
            var _user = await _userManager.FindByIdAsync(Id);   //IdentityRole
            UserVM user = new UserVM
            {
                ID = _user.Id,
                UserName = _user.UserName,
                UserRoles = (List<string>)await _userManager.GetRolesAsync(_user)
            };
            try
            {
                await UpdateUserRoles(selectedRoles, user);
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty,
                                "Unable to save changes.");
            }

            PopulateAssignedTeamData(user);
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

        private void PopulateAssignedTeamData(UserVM user)
        {
            //Prepare checkboxes for all Roles
            var allTeams = _WmbaContext.Teams
                                .Where(t => t.IsActive)
                                .ToList<Team>();

            var currentRoles = user.UserRoles;
            var viewModel = new List<RoleVM>();
            foreach (var r in allTeams)
            {
                viewModel.Add(new RoleVM
                {
                    RoleID = r.ID.ToString(),
                    RoleName = r.TmName,
                    Assigned = currentRoles.Contains(r.TmName)
                });
            }
            ViewBag.Teams = viewModel;
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
            
            foreach (Division d in allDivisions)
            {
                //create new viewModel
                var viewModel = new List<RoleVM>();
                 
                //for every team in this division
                foreach (Team t in d.Teams)
                {
                    //if the team roles list contains the team
                    if  (allTeamRoles.Any(tr => tr.TeamID == t.ID))
                    {
                        viewModel.Add(new RoleVM
                        {
                            RoleID = t.ID.ToString(),
                            RoleName = t.TmName,
                            Assigned = currentRoles.Contains(t.TmName)
                        });
                    }
                }

                //create a dynamic ViewData model that holds all teams in that division
                ViewData[$"{d.DivName}"] = viewModel;
            }

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
