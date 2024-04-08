using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using wmbaApp.CustomControllers;
using wmbaApp.Data;
using wmbaApp.Models;

namespace wmbaApp.Controllers
{
    public class GameLocationController : LookupsController
    {
        private readonly WmbaContext _context;

        public GameLocationController(WmbaContext context)
        {
            _context = context;
        }

        // GET: GameLocations
        public IActionResult Index()
        {
            return Redirect(ViewData["returnURL"].ToString());
        }

        // GET: GameLocations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.GameLocations == null)
            {
                return NotFound();
            }

            var gameLocation = await _context.GameLocations
                .FirstOrDefaultAsync(m => m.ID == id);
            if (gameLocation == null)
            {
                return NotFound();
            }

            return View(gameLocation);
        }

        // GET: GameLocations/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: GameLocations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Name")] GameLocation gameLocation)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(gameLocation);
                    await _context.SaveChangesAsync();
                    return Redirect(ViewData["returnURL"].ToString());
                }
            }
            catch (DbUpdateException dex)
            {
                if (dex.GetBaseException().Message.Contains("UNIQUE constraint failed"))
                {
                    ModelState.AddModelError("Name", "Unable to save changes. Remember, you cannot have duplicate Equipment Names.");
                }
                else
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                }
            }
            //Decide if we need to send the Validaiton Errors directly to the client
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

            return View(gameLocation);
        }

        // GET: GameLocations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.GameLocations == null)
            {
                return NotFound();
            }

            var gameLocation = await _context.GameLocations.FindAsync(id);

            if (gameLocation == null)
            {
                return NotFound();
            }
            return View(gameLocation);
        }

        // POST: GameLocations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Name")] GameLocation gameLocation)
        {
            var gameLocationToUpdate = await _context.GameLocations.FirstOrDefaultAsync(p => p.ID == id);

            //Check that you got it or exit with a not found error
            if (gameLocationToUpdate == null)
            {
                return NotFound();
            }

            //Try updating it with the values posted
            if (await TryUpdateModelAsync<GameLocation>(gameLocationToUpdate, "",
                p => p.Name))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    return Redirect(ViewData["returnURL"].ToString());
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GameLocationExists(gameLocationToUpdate.ID))
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
                    if (dex.GetBaseException().Message.Contains("UNIQUE constraint failed"))
                    {
                        ModelState.AddModelError("Name", "Unable to save changes. Remember, you cannot have duplicate Equipment Names.");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                    }
                }

            }
            return View(gameLocationToUpdate);
        }

        // GET: GameLocations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.GameLocations == null)
            {
                return NotFound();
            }

            var gameLocation = await _context.GameLocations
                .FirstOrDefaultAsync(m => m.ID == id);
            if (gameLocation == null)
            {
                return NotFound();
            }

            return View(gameLocation);
        }

        // POST: GameLocations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.GameLocations == null)
            {
                return Problem("Entity set 'WmbaContext.GameLocations'  is null.");
            }
            var gameLocation = await _context.GameLocations.FindAsync(id);
            if (gameLocation != null)
            {
                _context.GameLocations.Remove(gameLocation);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> InsertFromExcel(IFormFile theExcel)
        {
            string feedBack = string.Empty;
            if (theExcel != null)
            {
                string mimeType = theExcel.ContentType;
                long fileLength = theExcel.Length;
                if (!(mimeType == "" || fileLength == 0))//Looks like we have a file!!!
                {
                    if (mimeType.Contains("excel") || mimeType.Contains("spreadsheet"))
                    {
                        ExcelPackage excel;
                        using (var memoryStream = new MemoryStream())
                        {
                            await theExcel.CopyToAsync(memoryStream);
                            excel = new ExcelPackage(memoryStream);
                        }

                        var workSheet = excel.Workbook.Worksheets[0];
                        var start = workSheet.Dimension.Start;
                        var end = workSheet.Dimension.End;
                        int successCount = 0;
                        int errorCount = 0;
                        if (workSheet.Cells[1, 1].Text == "Diamond")
                        {
                            for (int row = start.Row + 1; row <= end.Row; row++)
                            {
                                GameLocation e = new GameLocation();
                                try
                                {
                                    // Row by row...
                                    e.Name = workSheet.Cells[row, 1].Text;
                                    _context.GameLocations.Add(e);
                                    _context.SaveChanges();
                                    successCount++;
                                }
                                catch (DbUpdateException dex)
                                {
                                    errorCount++;
                                    if (dex.GetBaseException().Message.Contains("UNIQUE constraint failed"))
                                    {
                                        feedBack += "Error: Record " + e.Name + " was rejected as a duplicate."
                                                + "<br />";
                                    }
                                    else
                                    {
                                        feedBack += "Error: Record " + e.Name + " caused an error."
                                                + "<br />";
                                    }
                                    //Here is the trick to using SaveChanges in a loop.  You must remove the 
                                    //offending object from the cue or it will keep raising the same error.
                                    _context.Remove(e);
                                }
                                catch (Exception ex)
                                {
                                    errorCount++;
                                    if (ex.GetBaseException().Message.Contains("correct format"))
                                    {
                                        feedBack += "Error: Record " + e.Name + " was rejected becuase the standard charge was not in the correct format."
                                                + "<br />";
                                    }
                                    else
                                    {
                                        feedBack += "Error: Record " + e.Name + " caused and error."
                                                + "<br />";
                                    }
                                }
                            }
                            feedBack += "Finished Importing " + (successCount + errorCount).ToString() +
                                " Records with " + successCount.ToString() + " inserted and " +
                                errorCount.ToString() + " rejected";
                        }
                        else
                        {
                            feedBack = "Error: You may have selected the wrong file to upload.<br /> Remember, you must have the headings 'Name' and 'Standard Charge' in the first two cells of the first row.";
                        }
                    }
                    else
                    {
                        feedBack = "Error: That file is not an Excel spreadsheet.";
                    }
                }
                else
                {
                    feedBack = "Error:  file appears to be empty";
                }
            }
            else
            {
                feedBack = "Error: No file uploaded";
            }

            TempData["Feedback"] = feedBack + "<br /><br />";

            //Note that we are assuming that you are using the Preferred Approach to Lookup Values
            //And the custom LookupsController
            return Redirect(ViewData["returnURL"].ToString());
        }


        private bool GameLocationExists(int id)
        {
            return _context.GameLocations.Any(e => e.ID == id);
        }
    }
}
