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
    public class NationsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public NationsController(AppDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Nations
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Nation.Include(n => n.InterLeagues);
            return View(await appDbContext.ToListAsync());
        }

        public async Task<IActionResult> NationJersey(int id)
        {
            var nation = await _context.Nation
                .Include(n => n.Jerseys)
                .FirstOrDefaultAsync(n => n.NationId == id);

            if (nation == null)
            {
                return NotFound();
            }

            return View(nation);
        }


        // GET: Nations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nations = await _context.Nation
                .Include(n => n.InterLeagues)
                .FirstOrDefaultAsync(m => m.NationId == id);
            if (nations == null)
            {
                return NotFound();
            }

            return View(nations);
        }

        // GET: Nations/Create
        public IActionResult Create()
        {
            ViewData["interLeaguesId"] = new SelectList(_context.InterLeague, "interLeaguesId", "interLeaguesName");
            return View();
        }

        // POST: Nations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Nations nations)
        {
            if (HttpContext.Request.Form.Files.Count > 0)
            {
                var file = HttpContext.Request.Form.Files[0];

                if (file.Length > 0)
                {
                    var originalFileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    var fileExtension = Path.GetExtension(originalFileName);
                    var uniqueFileName = originalFileName;

                    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "img/nations");
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }

                    nations.img = uniqueFileName;
                }
            }

            _context.Nation.Add(nations);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        // GET: Nations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nations = await _context.Nation.FindAsync(id);
            if (nations == null)
            {
                return NotFound();
            }
            ViewData["interLeaguesId"] = new SelectList(_context.InterLeague, "interLeaguesId", "interLeaguesName", nations.interLeaguesId);
            return View(nations);
        }

        // POST: Nations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("NationId,Name,interLeaguesId,img")] Nations nations)
        {
            if (id != nations.NationId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(nations);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NationsExists(nations.NationId))
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
            ViewData["interLeaguesId"] = new SelectList(_context.InterLeague, "interLeaguesId", "interLeaguesName", nations.interLeaguesId);
            return View(nations);
        }

        // GET: Nations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nations = await _context.Nation
                .Include(n => n.InterLeagues)
                .FirstOrDefaultAsync(m => m.NationId == id);
            if (nations == null)
            {
                return NotFound();
            }

            return View(nations);
        }

        // POST: Nations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var nations = await _context.Nation.FindAsync(id);
            if (nations != null)
            {
                _context.Nation.Remove(nations);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NationsExists(int id)
        {
            return _context.Nation.Any(e => e.NationId == id);
        }
    }
}
