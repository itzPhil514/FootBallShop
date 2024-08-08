using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FootBallShop.Models;

namespace FootBallShop.Controllers
{
    public class ClubsController : Controller
    {
        private readonly AppDbContext _context;

        public ClubsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Clubs
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Club.Include(c => c.League);
            return View(await appDbContext.ToListAsync());
        }

        // GET: Clubs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var clubs = await _context.Club
                .Include(c => c.League)
                .FirstOrDefaultAsync(m => m.ClubId == id);
            if (clubs == null)
            {
                return NotFound();
            }

            return View(clubs);
        }

        // GET: Clubs/Create
        public IActionResult Create()
        {
            ViewData["LeagueId"] = new SelectList(_context.League, "LeagueId", "LeagueName");
            return View();
        }

        // POST: Clubs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ClubId,Name,LeagueId,img")] Clubs clubs)
        {
            if (ModelState.IsValid)
            {
                _context.Add(clubs);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["LeagueId"] = new SelectList(_context.League, "LeagueId", "LeagueName", clubs.LeagueId);
            return View(clubs);
        }

        // GET: Clubs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var clubs = await _context.Club.FindAsync(id);
            if (clubs == null)
            {
                return NotFound();
            }
            ViewData["LeagueId"] = new SelectList(_context.League, "LeagueId", "LeagueName", clubs.LeagueId);
            return View(clubs);
        }

        // POST: Clubs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ClubId,Name,LeagueId,img")] Clubs clubs)
        {
            if (id != clubs.ClubId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(clubs);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClubsExists(clubs.ClubId))
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
            ViewData["LeagueId"] = new SelectList(_context.League, "LeagueId", "LeagueName", clubs.LeagueId);
            return View(clubs);
        }

        // GET: Clubs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var clubs = await _context.Club
                .Include(c => c.League)
                .FirstOrDefaultAsync(m => m.ClubId == id);
            if (clubs == null)
            {
                return NotFound();
            }

            return View(clubs);
        }

        // POST: Clubs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var clubs = await _context.Club.FindAsync(id);
            if (clubs != null)
            {
                _context.Club.Remove(clubs);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ClubsExists(int id)
        {
            return _context.Club.Any(e => e.ClubId == id);
        }
    }
}
