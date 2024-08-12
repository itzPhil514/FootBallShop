using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FootBallShop.Models;
using Microsoft.AspNetCore.Hosting;
using System.Net.Http.Headers;
using Twilio.TwiML.Voice;

namespace FootBallShop.Controllers
{
    public class ClubsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ClubsController(AppDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
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

                    clubs.img = uniqueFileName; // Assign the unique filename to the img property
                }
            }

            _context.Club.Add(clubs);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }



        // GET: Clubs/Edit/5
        /*  public async Task<IActionResult> Edit(int? id)
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
      }*/
    }
}