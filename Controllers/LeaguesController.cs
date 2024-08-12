using FootBallShop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;

namespace FootBallShop.Controllers
{
    public class LeaguesController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public LeaguesController(AppDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Leagues
        public async Task<IActionResult> Index()
        {
            return View(await _context.League.ToListAsync());
        }

        // GET: Leagues/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var leagues = await _context.League
                .FirstOrDefaultAsync(m => m.LeagueId == id);
            if (leagues == null)
            {
                return NotFound();
            }

            return View(leagues);
        }

        // GET: Leagues/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Leagues/Create
        [HttpPost]
        public async Task<IActionResult> Create(Leagues league)
        {
            if (HttpContext.Request.Form.Files.Count > 0)
            {
                var file = HttpContext.Request.Form.Files[0];

                if (file.Length > 0)
                {
                    var originalFileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    var fileExtension = Path.GetExtension(originalFileName);
                    var uniqueFileName = originalFileName;

                    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "img/leagues");
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }

                    league.imgLeaguePath = uniqueFileName; 
                }
            }
            _context.League.Add(league);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        // GET: Leagues/Clubs/5
        public async Task<IActionResult> Clubs(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var league = await _context.League
                .Include(l => l.Clubs)
                .FirstOrDefaultAsync(m => m.LeagueId == id);

            if (league == null)
            {
                return NotFound();
            }

            return View(league);
        }


        // GET: Leagues/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var league = await _context.League.FindAsync(id);
            if (league == null)
            {
                return NotFound();
            }
            return View(league);
        }

        // POST: Leagues/Edit/5


        // GET: Leagues/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var league = await _context.League
                .FirstOrDefaultAsync(m => m.LeagueId == id);
            if (league == null)
            {
                return NotFound();
            }

            return View(league);
        }

        // POST: Leagues/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var league = await _context.League.FindAsync(id);
            if (league != null)
            {
                _context.League.Remove(league);
                await _context.SaveChangesAsync();

                // Reorder the LeagueIds
                var leagues = await _context.League.OrderBy(l => l.LeagueId).ToListAsync();

                int newId = 1;
                foreach (var item in leagues)
                {
                    if (item.LeagueId != newId)
                    {
                        item.LeagueId = newId;
                    }
                    newId++;
                }

                // Update the database with the reordered IDs
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

    }
}
