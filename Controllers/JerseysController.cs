using FootBallShop.Models;
using Microsoft.AspNetCore.Authorization;
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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var jerseys = await _context.Jersey
                .Include(j => j.Club).Include(j => j.League)
                .Include(j => j.Nation).Include(j => j.InterLeague)
                .ToListAsync();
            return View(jerseys);
        }

        // GET: Jerseys/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var jersey = await _context.Jersey
                .Include(j => j.Club).Include(j => j.League)
                .Include(j => j.Nation).Include(j => j.InterLeague)
                .FirstOrDefaultAsync(m => m.JerseysId == id);

            if (jersey == null) return NotFound();
            return View(jersey);
        }

        // GET: Jerseys/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Jerseys/Create
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Jerseys jerseys)
        {
            // Handle image upload
            if (HttpContext.Request.Form.Files.Count > 0)
            {
                var file = HttpContext.Request.Form.Files[0];
                if (file.Length > 0)
                {
                    var originalFileName = ContentDispositionHeaderValue
                        .Parse(file.ContentDisposition).FileName.Trim('"');
                    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "img/jerseys");
                    Directory.CreateDirectory(uploadsFolder);
                    var filePath = Path.Combine(uploadsFolder, originalFileName);
                    using var fs = new FileStream(filePath, FileMode.Create);
                    await file.CopyToAsync(fs);
                    jerseys.img = originalFileName;
                }
            }

            // Clear irrelevant FK errors based on IsInter flag
            if (jerseys.IsInter)
            {
                ModelState.Remove("LeagueId");
                ModelState.Remove("ClubId");
                jerseys.LeagueId = null;
                jerseys.ClubId = null;
            }
            else
            {
                ModelState.Remove("interLeaguesId");
                ModelState.Remove("NationId");
                jerseys.interLeaguesId = null;
                jerseys.NationId = null;
            }

            ModelState.Remove("img");
            ModelState.Remove("Club");
            ModelState.Remove("League");
            ModelState.Remove("Nation");
            ModelState.Remove("InterLeague");
            ModelState.Remove("Size");

            _context.Jersey.Add(jerseys);
            await _context.SaveChangesAsync();
            TempData["Success"] = $"Jersey \"{jerseys.Name}\" created successfully.";
            return RedirectToAction("Index", "Admin");
        }

        // GET: Jerseys/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var jersey = await _context.Jersey.FindAsync(id);
            if (jersey == null) return NotFound();

            ViewData["ClubId"] = new SelectList(_context.Club, "ClubId", "Name", jersey.ClubId);
            ViewData["LeagueId"] = new SelectList(_context.League, "LeagueId", "LeagueName", jersey.LeagueId);
            ViewData["NationId"] = new SelectList(_context.Nation, "NationId", "Name", jersey.NationId);
            ViewData["interLeaguesId"] = new SelectList(_context.InterLeague, "interLeaguesId", "interLeaguesName", jersey.interLeaguesId);
            return View(jersey);
        }

        // POST: Jerseys/Edit/5
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("JerseysId,Name,Price,LeagueId,ClubId,NationId,interLeaguesId,IsInter,Category,img")] Jerseys jerseys)
        {
            if (id != jerseys.JerseysId) return NotFound();

            // Handle image upload — keep existing if no new file
            if (HttpContext.Request.Form.Files.Count > 0)
            {
                var file = HttpContext.Request.Form.Files[0];
                if (file.Length > 0)
                {
                    var originalFileName = ContentDispositionHeaderValue
                        .Parse(file.ContentDisposition).FileName.Trim('"');
                    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "img/jerseys");
                    Directory.CreateDirectory(uploadsFolder);
                    var filePath = Path.Combine(uploadsFolder, originalFileName);
                    using var fs = new FileStream(filePath, FileMode.Create);
                    await file.CopyToAsync(fs);
                    jerseys.img = originalFileName;
                }
            }

            ModelState.Remove("img");
            ModelState.Remove("Club");
            ModelState.Remove("League");
            ModelState.Remove("Nation");
            ModelState.Remove("InterLeague");
            ModelState.Remove("Size");

            if (jerseys.IsInter)
            {
                ModelState.Remove("LeagueId");
                ModelState.Remove("ClubId");
                jerseys.LeagueId = null;
                jerseys.ClubId = null;
            }
            else
            {
                ModelState.Remove("interLeaguesId");
                ModelState.Remove("NationId");
                jerseys.interLeaguesId = null;
                jerseys.NationId = null;
            }

            try
            {
                _context.Update(jerseys);
                await _context.SaveChangesAsync();
                TempData["Success"] = $"Jersey \"{jerseys.Name}\" updated successfully.";
                return RedirectToAction("Jerseys", "Admin");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Jersey.Any(e => e.JerseysId == jerseys.JerseysId))
                    return NotFound();
                throw;
            }
        }

        // GET: Jerseys/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var jersey = await _context.Jersey
                .Include(j => j.Club).Include(j => j.League)
                .Include(j => j.Nation).Include(j => j.InterLeague)
                .FirstOrDefaultAsync(m => m.JerseysId == id);
            if (jersey == null) return NotFound();
            return View(jersey);
        }

        // POST: Jerseys/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var jersey = await _context.Jersey.FindAsync(id);
            if (jersey != null)
            {
                _context.Jersey.Remove(jersey);
                await _context.SaveChangesAsync();
                TempData["Success"] = $"Jersey deleted.";
            }
            return RedirectToAction("Jerseys", "Admin");
        }

        // AJAX endpoints
        [HttpGet]
        public JsonResult GetInternationalLeaguesAndNations()
        {
            var interLeagues = _context.InterLeague
                .Select(il => new { value = il.interLeaguesId, text = il.interLeaguesName }).ToList();
            return Json(new { interLeagues });
        }

        [HttpGet]
        public JsonResult GetRegularLeaguesAndClubs()
        {
            var leagues = _context.League
                .Select(l => new { value = l.LeagueId, text = l.LeagueName }).ToList();
            return Json(new { leagues });
        }

        [HttpGet]
        public JsonResult GetClubsByLeague(int leagueId)
        {
            var clubs = _context.Club
                .Where(c => c.LeagueId == leagueId)
                .Select(c => new { value = c.ClubId, text = c.Name }).ToList();
            return Json(new { clubs });
        }

        [HttpGet]
        public JsonResult GetNationsByInterLeague(int interLeagueId)
        {
            var nations = _context.Nation
                .Where(n => n.interLeaguesId == interLeagueId)
                .Select(n => new { value = n.NationId, text = n.Name }).ToList();
            return Json(new { nations });
        }

        [HttpGet]
        public JsonResult GetTeamsByLeague(int leagueId)
        {
            var teams = _context.Club
                .Where(t => t.LeagueId == leagueId)
                .Select(t => new { teamId = t.ClubId, teamName = t.Name }).ToList();
            return Json(teams);
        }
    }
}