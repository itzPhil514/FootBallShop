﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FootBallShop.Models;
using Microsoft.AspNetCore.Hosting;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Identity;
using System.IO;

namespace FootBallShop.Controllers
{
    public class ClubsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly UserManager<IdentityUser> _userManager;

        public ClubsController(AppDbContext context, IWebHostEnvironment webHostEnvironment, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _userManager = userManager;
        }

        // GET: Clubs
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Club.Include(c => c.League);
            return View(await appDbContext.ToListAsync());
        }
        public async Task<IActionResult> List_Clubs()
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Clubs clubs)
        {
            if (HttpContext.Request.Form.Files.Count > 0)
            {
                var file = HttpContext.Request.Form.Files[0];

                if (file.Length > 0)
                {
                    var originalFileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    var fileExtension = Path.GetExtension(originalFileName);
                    var uniqueFileName = originalFileName;

                    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "img/clubs");
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }

                    clubs.img = uniqueFileName;
                }
            }

            _context.Club.Add(clubs);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Jersey(int id)
        {
            var club = await _context.Club
                .Include(c => c.Jersey)
                .FirstOrDefaultAsync(c => c.ClubId == id);

            if (club == null)
            {
                return NotFound();
            }

            return View(club);
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

            // Check if the user is in the Admin role
            if (!User.Identity.IsAuthenticated || !await _userManager.IsInRoleAsync(await _userManager.GetUserAsync(User), "Admin"))
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            ViewData["LeagueId"] = new SelectList(_context.League, "LeagueId", "LeagueName", clubs.LeagueId);
            return View(clubs);
        }

        // POST: Clubs/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ClubId,Name,LeagueId,img")] Clubs clubs)
        {
            if (HttpContext.Request.Form.Files.Count > 0)
            {
                var file = HttpContext.Request.Form.Files[0];

                if (file.Length > 0)
                {
                    var originalFileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    var fileExtension = Path.GetExtension(originalFileName);
                    var uniqueFileName = originalFileName;

                    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "img/clubs");
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }

                    clubs.img = uniqueFileName;
                }
            }

            _context.Club.Update(clubs);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
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

            // Check if the user is in the Admin role
            if (!User.Identity.IsAuthenticated || !await _userManager.IsInRoleAsync(await _userManager.GetUserAsync(User), "Admin"))
            {
                return RedirectToAction("AccessDenied", "Account"); // Redirect to an access denied page
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
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ClubsExists(int id)
        {
            return _context.Club.Any(e => e.ClubId == id);
        }
    }
}
