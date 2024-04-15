using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using wmbaApp.CustomControllers;
using wmbaApp.Data;
using wmbaApp.Models;
using wmbaApp.Utilities;

namespace wmbaApp.Controllers
{
    [Authorize(Roles = "Admin,Convenor")]
    public class DivisionsController : ElephantController
    {
        private readonly WmbaContext _context;
        private readonly ApplicationDbContext _AppContext;
        private readonly RoleManager<ApplicationRole> _roleManager;


        public DivisionsController(WmbaContext context, ApplicationDbContext appContext, RoleManager<ApplicationRole> roleManager)
        {
            _context = context;
            _AppContext = appContext;
            _roleManager = roleManager;
        }

        // GET: Divisions
        public async Task<IActionResult> Index(int? page, int? pageSizeID)
        {
            IQueryable<Division> divisions;
            if (TempData["SuccessMessage"] != null)
            {
                ViewBag.SuccessMessage = TempData["SuccessMessage"].ToString();
            }

            if (User.IsInRole("Admin"))
            {
                divisions = _context.Divisions
                .Include(d => d.DivisionCoaches)
                .Include(d => d.Teams)
                .AsNoTracking();
            }
            else
            {
                //get all roles belonging to the user
                var rolesDivIDs = await UserRolesHelper.GetUserDivisionIDs(_AppContext, User);

                divisions = _context.Divisions
                .Include(d => d.DivisionCoaches)
                .Include(d => d.Teams)
                .Where(d => rolesDivIDs.Contains(d.ID)) //check for a matching division ID inside of the list of roles division IDs
                .AsNoTracking();
            }



            int pageSize = PageSizeHelper.SetPageSize(HttpContext, pageSizeID, ControllerName());
            ViewData["pageSizeID"] = PageSizeHelper.PageSizeList(pageSize);
            var pagedData = await PaginatedList<Division>.CreateAsync(divisions.AsNoTracking(), page ?? 1, pageSize);

            return View(pagedData);
        }

        // GET: Divisions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Divisions == null)
            {
                return NotFound();
            }

            var division = await _context.Divisions
                .FirstOrDefaultAsync(m => m.ID == id);
            if (division == null)
            {
                return NotFound();
            }

            if (!await UserRolesHelper.IsAuthorizedForDivision(_AppContext, User, division))
                return RedirectToAction("Index", "Divisions");

            return View(division);
        }

        // GET: Divisions/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Divisions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,DivName")] Division division)
        {
            if (!await UserRolesHelper.IsAuthorizedForDivision(_AppContext, User, division))
                return RedirectToAction("Index", "Divisions");

            try
            {
                if (ModelState.IsValid)
                {
                    if (!string.IsNullOrEmpty(division.DivName))
                    {
                        division.DivName = division.DivName.ToUpper();
                    }

                    _context.Add(division);
                    await _context.SaveChangesAsync();
                    IdentityResult roleResult;
                    roleResult = await _roleManager.CreateAsync(new ApplicationRole(division.DivName + " Convenor", division.ID, 0));
                    TempData["SuccessMessage"] = "Division created successfully.";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error creating players. {ex.Message}";
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
            return View(division);
        }

        // GET: Divisions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Divisions == null)
            {
                return NotFound();
            }

            var division = await _context.Divisions.FindAsync(id);
            if (division == null)
            {
                return NotFound();
            }

            if (!await UserRolesHelper.IsAuthorizedForDivision(_AppContext, User, division))
                return RedirectToAction("Index", "Divisions");

            return View(division);
        }

        // POST: Divisions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id)
        {
            var divisionsToUpdate = await _context.Divisions.FirstOrDefaultAsync(p => p.ID == id);


            if (divisionsToUpdate == null)
            {
                return NotFound();
            }

            if (!await UserRolesHelper.IsAuthorizedForDivision(_AppContext, User, divisionsToUpdate))
                return RedirectToAction("Index", "Divisions");

            if (await TryUpdateModelAsync<Division>(divisionsToUpdate, "",
                d => d.DivName))
            {
                try
                {
                    divisionsToUpdate.DivName = divisionsToUpdate.DivName.ToUpper();
                    _context.Update(divisionsToUpdate);
                    await _context.SaveChangesAsync();
                    var role = _roleManager.Roles.FirstOrDefault(r => r.DivID == divisionsToUpdate.ID);
                    if (role != null)
                    {
                        role.Name = divisionsToUpdate.DivName + " Convenor";
                        role.NormalizedName = divisionsToUpdate.DivName.ToUpper() + " Convenor".ToUpper();
                        _AppContext.Roles.Update(role);
                        await _AppContext.SaveChangesAsync();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DivisionExists(divisionsToUpdate.ID))
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
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Error creating players. {ex.Message}";
                }
            }
            TempData["SuccessMessage"] = "Division edited successfully.";
            return RedirectToAction("Details", new { divisionsToUpdate.ID });

        }

        // GET: Divisions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Divisions == null)
            {
                return NotFound();
            }

            var division = await _context.Divisions
                .FirstOrDefaultAsync(m => m.ID == id);
            if (division == null)
            {
                return NotFound();
            }

            if (!await UserRolesHelper.IsAuthorizedForDivision(_AppContext, User, division))
                return RedirectToAction("Index", "Divisions");

            return View(division);
        }

        // POST: Divisions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Divisions == null)
            {
                return Problem("There are no Divisions to delete.");
            }
            var division = await _context.Divisions.FindAsync(id);

            if (!await UserRolesHelper.IsAuthorizedForDivision(_AppContext, User, division))
                return RedirectToAction("Index", "Divisions");

            try
            {
                if (division != null)
                {
                    _context.Divisions.Remove(division);
                }
                await _context.SaveChangesAsync();
                var role = _roleManager.Roles.FirstOrDefault(r => r.DivID == division.ID);
                if (role != null)
                {
                    _AppContext.Roles.Remove(role);
                    await _AppContext.SaveChangesAsync();
                }
                TempData["SuccessMessage"] = "Division deleted successfully.";
                return Redirect(ViewData["returnURL"].ToString());
            }
            catch (DbUpdateException dex)
            {
                if (dex.GetBaseException().Message.Contains("FOREIGN KEY constraint failed"))
                {
                    ModelState.AddModelError("", "Unable to Delete Division. Remember, you cannot delete a Division that is used in the system.");
                }
                else
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Unable to delete this dividion, {ex.Message}";
            }

            return View(division);
        }

        private bool DivisionExists(int id)
        {
            return _context.Divisions.Any(e => e.ID == id);
        }
    }
}
