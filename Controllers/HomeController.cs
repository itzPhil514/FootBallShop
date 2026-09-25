using FootBallShop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace FootBallShop.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public JsonResult SearchAjax(string query, string type = "all")
        {
            if (string.IsNullOrEmpty(query))
                return Json(new List<object>());

            var results = new List<object>();

            // Jerseys
            if (type == "all" || type == "jerseys")
            {
                var jerseys = _context.Jersey
                    .Where(j => j.Name.Contains(query))
                    .Select(j => new
                    {
                        id = j.JerseysId,
                        name = j.Name,
                        img = j.img,
                        type = "jersey",
                        url = "/Jerseys/Details/" + j.JerseysId,
                        subtitle = j.IsInter ? "International" : "Club jersey"
                    })
                    .Take(4)
                    .ToList<object>();
                results.AddRange(jerseys);
            }

            // Nations
            if (type == "all" || type == "nations")
            {
                var nations = _context.Nation
                    .Include(n => n.InterLeagues)
                    .Where(n => n.Name.Contains(query))
                    .Select(n => new
                    {
                        id = n.NationId,
                        name = n.Name,
                        img = "nations/" + n.img,
                        type = "nation",
                        url = "/Nations/NationJersey/" + n.NationId,
                        subtitle = n.InterLeagues != null ? n.InterLeagues.interLeaguesName : "National team"
                    })
                    .Take(3)
                    .ToList<object>();
                results.AddRange(nations);
            }

            // Clubs
            if (type == "all" || type == "clubs")
            {
                var clubs = _context.Club
                    .Include(c => c.League)
                    .Where(c => c.Name.Contains(query))
                    .Select(c => new
                    {
                        id = c.ClubId,
                        name = c.Name,
                        img = "clubs/" + c.img,
                        type = "club",
                        url = "/Clubs/Jersey/" + c.ClubId,
                        subtitle = c.League != null ? c.League.LeagueName : "Club"
                    })
                    .Take(3)
                    .ToList<object>();
                results.AddRange(clubs);
            }

            return Json(results);
        }

        public IActionResult Index()
        {
            var randomJerseys = _context.Jersey
                .Include(t => t.League)
                .OrderBy(t => Guid.NewGuid())
                .Take(8)
                .ToList();
            return View(randomJerseys);
        }

        public IActionResult Privacy() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}