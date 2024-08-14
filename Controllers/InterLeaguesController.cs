using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FootBallShop.Models;
using System.Net.Http.Headers;

namespace FootBallShop.Controllers
{
    public class InterLeaguesController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public InterLeaguesController(AppDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: InterLeagues
        public async Task<IActionResult> Index()
        {
            return View(await _context.InterLeague.ToListAsync());
        }

        // GET: InterLeagues/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var interLeagues = await _context.InterLeague
                .FirstOrDefaultAsync(m => m.interLeaguesId == id);
            if (interLeagues == null)
            {
                return NotFound();
            }

            return View(interLeagues);
        }

        // GET: InterLeagues/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: InterLeagues/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<IActionResult> Create(InterLeagues interleague)
        {
            if (HttpContext.Request.Form.Files.Count > 0)
            {
                var file = HttpContext.Request.Form.Files[0];

                if (file.Length > 0)
                {
                    var originalFileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    var fileExtension = Path.GetExtension(originalFileName);
                    var uniqueFileName = originalFileName;

                    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "img/interleagues");
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }

                    interleague.img = uniqueFileName;
                }
            }
            _context.InterLeague.Add(interleague);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> NationsByInterLeague(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var interleague = await _context.InterLeague
                .Include(l => l.Nation)
                .FirstOrDefaultAsync(m => m.interLeaguesId == id);

            if (interleague == null)
            {
                return NotFound();
            }

            return View(interleague);
        }

        // GET: InterLeagues/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var interLeagues = await _context.InterLeague.FindAsync(id);
            if (interLeagues == null)
            {
                return NotFound();
            }
            return View(interLeagues);
        }

        // POST: InterLeagues/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("interLeaguesId,interLeaguesName,img")] InterLeagues interLeagues)
        {
            if (id != interLeagues.interLeaguesId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(interLeagues);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InterLeaguesExists(interLeagues.interLeaguesId))
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
            return View(interLeagues);
        }

        // GET: InterLeagues/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var interLeagues = await _context.InterLeague
                .FirstOrDefaultAsync(m => m.interLeaguesId == id);
            if (interLeagues == null)
            {
                return NotFound();
            }

            return View(interLeagues);
        }

        // POST: InterLeagues/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var interLeagues = await _context.InterLeague.FindAsync(id);
            if (interLeagues != null)
            {
                _context.InterLeague.Remove(interLeagues);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool InterLeaguesExists(int id)
        {
            return _context.InterLeague.Any(e => e.interLeaguesId == id);
        }
    }
}
