using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using wmbaApp.CustomControllers;
using wmbaApp.Data;
using wmbaApp.Models;

namespace wmbaApp.Controllers
{
    public class InActivePlayersController : ElephantController
    {
        private readonly WmbaContext _context;

        public InActivePlayersController(WmbaContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var inactivePlayers = _context.InActivePlayers.ToList();
            return View(inactivePlayers);
        }

        public IActionResult MakeActive(int id)
        {
            var inactivePlayer = _context.InActivePlayers
                .Include(ip => ip.Teams)
                .Include(ip => ip.Statistics)
                .FirstOrDefault(m => m.Id == id);

            if (inactivePlayer == null)
            {
                return NotFound();
            }

            if (inactivePlayer.Teams != null && inactivePlayer.Statistics != null)
            {
                var player = new Player
                {
                    ID = inactivePlayer.Id,
                    PlyrFirstName = inactivePlayer.FirstName,
                    PlyrLastName = inactivePlayer.LastName,
                    PlyrJerseyNumber = inactivePlayer.JerseyNumber,
                    PlyrMemberID = inactivePlayer.MemberID,
                    TeamID = inactivePlayer.Teams.ID,
                    StatisticID = inactivePlayer.Statistics.ID
                };

                _context.Players.Add(player);
                _context.InActivePlayers.Remove(inactivePlayer);
                _context.SaveChanges();

                return RedirectToAction("Index", "Players");
            }
            else
            {
                return BadRequest("Invalid Team or Statistics information for inactive player");
            }
        }

    }
}
