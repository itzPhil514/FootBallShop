using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting; // Add this using directive for IWebHostEnvironment
using Microsoft.AspNetCore.Http; // Add this using directive for IFormFile
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FootBallShop.Models;
using FootBallShop2.Models;

namespace FootBallShop.Controllers
{
    public class LeaguesController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment; // Add this field

        public LeaguesController(AppDbContext context, IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
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
        public IActionResult Create()
        {
            return View();
        }

        // POST: Leagues/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Leagues league)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Console.WriteLine("Model is valid.");
                    if (league.imgLeague != null && league.imgLeague.Length > 0)
                    {
                        Console.WriteLine("Image is present.");
                        var fileName = Path.GetFileName(league.imgLeague.FileName);
                        var filePath = Path.Combine(_hostingEnvironment.WebRootPath, "img", fileName);

                        // Create directory if it does not exist
                        var directoryPath = Path.GetDirectoryName(filePath);
                        if (!Directory.Exists(directoryPath))
                        {
                            Directory.CreateDirectory(directoryPath);
                        }

                        // Save the file
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await league.imgLeague.CopyToAsync(stream);
                        }

                        // Store the file path in the database
                        league.imgLeaguePath = fileName;

                        TempData["SuccessMessage"] = "Image uploaded successfully.";
                    }
                    else
                    {
                        TempData["WarningMessage"] = "No image uploaded.";
                    }

                    _context.Add(league);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
                }
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Leagues league)
        {
            if (id != league.LeagueId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Handle file upload if a new file is provided
                    if (league.imgLeague != null && league.imgLeague.Length > 0)
                    {
                        var fileName = Path.GetFileName(league.imgLeague.FileName);
                        var filePath = Path.Combine(_hostingEnvironment.WebRootPath, "img/Leagues", fileName);

                        // Create directory if it does not exist
                        var directoryPath = Path.GetDirectoryName(filePath);
                        if (!Directory.Exists(directoryPath))
                        {
                            Directory.CreateDirectory(directoryPath);
                        }

                        // Save the file
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await league.imgLeague.CopyToAsync(stream);
                        }

                        // Update the file path
                        league.imgLeaguePath = fileName;
                    }

                    _context.Update(league);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LeaguesExists(league.LeagueId))
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
            return View(league);
        }

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
            }
            return RedirectToAction(nameof(Index));
        }

        private bool LeaguesExists(int id)
        {
            return _context.League.Any(e => e.LeagueId == id);
        }
    }
}
