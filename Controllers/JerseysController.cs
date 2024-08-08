using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FootBallShop.Models;
using FootBallShop2.Models;

namespace FootBallShop.Controllers
{
    public class JerseysController : Controller
    {
        private readonly AppDbContext _context;

        public JerseysController(AppDbContext context)
        {
            _context = context;
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
            ViewData["TeamId"] = new SelectList(_context.Club, "ClubId", "Name");
            ViewData["LeagueId"] = new SelectList(_context.League, "LeagueId", "LeagueName");
            return View();
        }

        // POST: Jerseys/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("JerseysId,Name,Size,Price,LeagueId,TeamId,img")] Jerseys jerseys)
        {
            if (ModelState.IsValid)
            {
                _context.Add(jerseys);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["TeamId"] = new SelectList(_context.Club, "ClubId", "Name", jerseys.TeamId);
            ViewData["LeagueId"] = new SelectList(_context.League, "LeagueId", "LeagueName", jerseys.LeagueId);
            return View(jerseys);
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
            ViewData["TeamId"] = new SelectList(_context.Club, "ClubId", "Name", jerseys.TeamId);
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
            ViewData["TeamId"] = new SelectList(_context.Club, "ClubId", "Name", jerseys.TeamId);
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
    }
}
