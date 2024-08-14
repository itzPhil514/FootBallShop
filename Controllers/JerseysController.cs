using FootBallShop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;

namespace FootBallShop.Controllers
{
    public class JerseysController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public JerseysController(AppDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Jerseys
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Jersey.Include(j => j.Club).Include(j => j.League);
            return View(await appDbContext.ToListAsync());
        }

        // GET: Jerseys/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jerseys = await _context.Jersey
                .Include(j => j.Club)
                .Include(j => j.League)
                .FirstOrDefaultAsync(m => m.JerseysId == id);
            if (jerseys == null)
            {
                return NotFound();
            }

            return View(jerseys);
        }

        // GET: Jerseys/Create
        public IActionResult Create()
        {
            // Assuming you fetch these lists based on your logic
            ViewBag.Leagues = _context.League
                .Select(l => new SelectListItem { Value = l.LeagueId.ToString(), Text = l.LeagueName })
                .ToList();

            ViewBag.Clubs = _context.Club
                .Select(c => new SelectListItem { Value = c.ClubId.ToString(), Text = c.Name })
                .ToList();

            ViewBag.InterLeagues = _context.InterLeague
                .Select(il => new SelectListItem { Value = il.interLeaguesId.ToString(), Text = il.interLeaguesName })
                .ToList();

            ViewBag.Nations = _context.Nation
                .Select(n => new SelectListItem { Value = n.NationId.ToString(), Text = n.Name })
                .ToList();

            return View();
        }


        // POST: Jerseys/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Jerseys jerseys)
        {
            if (HttpContext.Request.Form.Files.Count > 0)
            {
                var file = HttpContext.Request.Form.Files[0];

                if (file.Length > 0)
                {
                    var originalFileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    var fileExtension = Path.GetExtension(originalFileName);
                    var uniqueFileName = originalFileName;

                    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "img/jerseys");
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }

                    jerseys.img = uniqueFileName; // Assign the unique filename to the img property
                }
            }

            _context.Jersey.Add(jerseys);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        // GET: Jerseys/GetTeamsByLeague
        public IActionResult GetTeamsByLeague(int leagueId)
        {
            var teams = _context.Club
                .Where(t => t.LeagueId == leagueId)
                .Select(t => new { teamId = t.ClubId, teamName = t.Name })
                .ToList();

            return Json(teams);
        }



        // GET: Jerseys/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jerseys = await _context.Jersey.FindAsync(id);
            if (jerseys == null)
            {
                return NotFound();
            }
            ViewData["ClubId"] = new SelectList(_context.Club, "ClubId", "Name", jerseys.ClubId);
            ViewData["LeagueId"] = new SelectList(_context.League, "LeagueId", "LeagueName", jerseys.LeagueId);
            return View(jerseys);
        }

        // POST: Jerseys/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("JerseysId,Name,Size,Price,LeagueId,TeamId,img")] Jerseys jerseys)
        {
            if (id != jerseys.JerseysId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(jerseys);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!JerseysExists(jerseys.JerseysId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClubId"] = new SelectList(_context.Club, "ClubId", "Name", jerseys.ClubId);
            ViewData["LeagueId"] = new SelectList(_context.League, "LeagueId", "LeagueName", jerseys.LeagueId);
            return View(jerseys);
        }

        // GET: Jerseys/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jerseys = await _context.Jersey
                .Include(j => j.Club)
                .Include(j => j.League)
                .FirstOrDefaultAsync(m => m.JerseysId == id);
            if (jerseys == null)
            {
                return NotFound();
            }

            return View(jerseys);
        }

        // POST: Jerseys/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var jerseys = await _context.Jersey.FindAsync(id);
            if (jerseys != null)
            {
                _context.Jersey.Remove(jerseys);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool JerseysExists(int id)
        {
            return _context.Jersey.Any(e => e.JerseysId == id);
        }
        [HttpGet]
        public JsonResult GetInternationalLeaguesAndNations()
        {
            var interLeagues = _context.InterLeague
                .Select(il => new { value = il.interLeaguesId, text = il.interLeaguesName })
                .ToList();

            var nations = _context.Nation
                .Select(n => new { value = n.NationId, text = n.Name })
                .ToList();

            return Json(new { interLeagues, nations });
        }

        [HttpGet]
        public JsonResult GetRegularLeaguesAndClubs()
        {
            var leagues = _context.League
                .Select(l => new { value = l.LeagueId, text = l.LeagueName })
                .ToList();

            var clubs = _context.Club
                .Select(c => new { value = c.ClubId, text = c.Name })
                .ToList();

            return Json(new { leagues, clubs });
        }
    }
}
