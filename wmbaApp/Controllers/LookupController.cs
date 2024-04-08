using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using wmbaApp.CustomControllers;
using wmbaApp.Data;

namespace wmbaApp.Controllers
{
    [Authorize(Roles ="Admin,Convenor")]
    public class LookupController : CognizantController
    {
        private readonly WmbaContext _context;

        public LookupController(WmbaContext context)
        {
            _context = context;
        }

        public IActionResult Index(string Tab = "Information-Tab")
        {
            //Note: select the tab you want to load by passing in
            ViewData["Tab"] = Tab;
            return View();
        }

        public PartialViewResult GameLocation()
        {
            ViewData["GameLocationID"] = new
                SelectList(_context.GameLocations
                .OrderBy(a => a.Name), "ID", "Summary");
            return PartialView("_GameLocation");
        }
    }
}
