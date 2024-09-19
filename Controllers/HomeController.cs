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
        public JsonResult SearchAjax(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                return Json(new List<Jerseys>());
            }

            var searchResults = _context.Jersey
                                 .Include(j => j.Club)
                                 .Where(j => j.Name.Contains(query))
                                 .Select(j => new {
                                     j.Name,
                                     j.img
                                 })
                                 .Take(5)
                                 .ToList();

            return Json(searchResults);
        }

        public IActionResult Index()
        {
            var allLeagues = _context.League.ToList();
            var randomTshirts = _context.Jersey.Include(t => t.League).OrderBy(t => Guid.NewGuid()).Take(4).ToList();
            //var allContinents = _context.Continents.ToList(); // Fetch all continents

            ViewBag.AllLeagues = allLeagues;
            //ViewBag.AllContinents = allContinents; // Pass continents to view

            return View(randomTshirts);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
