using Microsoft.AspNetCore.Mvc;

namespace wmbaApp.Controllers
{
    public class LookupController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
