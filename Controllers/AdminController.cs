using FootBallShop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FootBallShop.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(AppDbContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: /Admin
        public async Task<IActionResult> Index()
        {
            var vm = new AdminDashboardViewModel
            {
                TotalJerseys = await _context.Jersey.CountAsync(),
                TotalClubs = await _context.Club.CountAsync(),
                TotalLeagues = await _context.League.CountAsync(),
                TotalNations = await _context.Nation.CountAsync(),
                TotalInterLeagues = await _context.InterLeague.CountAsync(),
                TotalUsers = await _userManager.Users.CountAsync(),
                RecentJerseys = await _context.Jersey
                    .Include(j => j.Club).Include(j => j.Nation)
                    .Include(j => j.League).Include(j => j.InterLeague)
                    .OrderByDescending(j => j.JerseysId)
                    .Take(8).ToListAsync()
            };
            return View(vm);
        }

        // GET: /Admin/Jerseys
        public async Task<IActionResult> Jerseys(string q)
        {
            var query = _context.Jersey
                .Include(j => j.Club).Include(j => j.Nation)
                .Include(j => j.League).Include(j => j.InterLeague)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
                query = query.Where(j => j.Name.Contains(q) ||
                                         (j.Club != null && j.Club.Name.Contains(q)) ||
                                         (j.Nation != null && j.Nation.Name.Contains(q)));

            ViewBag.Q = q;
            return View(await query.OrderByDescending(j => j.JerseysId).ToListAsync());
        }

        // GET: /Admin/Leagues
        public async Task<IActionResult> Leagues(string q)
        {
            var query = _context.League.Include(l => l.Clubs).AsQueryable();
            if (!string.IsNullOrWhiteSpace(q))
                query = query.Where(l => l.LeagueName.Contains(q));
            ViewBag.Q = q;
            return View(await query.ToListAsync());
        }

        // GET: /Admin/Clubs
        public async Task<IActionResult> Clubs(string q)
        {
            var query = _context.Club.Include(c => c.League).AsQueryable();
            if (!string.IsNullOrWhiteSpace(q))
                query = query.Where(c => c.Name.Contains(q) ||
                                         c.League.LeagueName.Contains(q));
            ViewBag.Q = q;
            return View(await query.ToListAsync());
        }

        // GET: /Admin/InterLeagues
        public async Task<IActionResult> InterLeagues(string q)
        {
            var query = _context.InterLeague.Include(il => il.Nation).AsQueryable();
            if (!string.IsNullOrWhiteSpace(q))
                query = query.Where(il => il.interLeaguesName.Contains(q));
            ViewBag.Q = q;
            return View(await query.ToListAsync());
        }

        // GET: /Admin/Nations
        public async Task<IActionResult> Nations(string q)
        {
            var query = _context.Nation.Include(n => n.InterLeagues).AsQueryable();
            if (!string.IsNullOrWhiteSpace(q))
                query = query.Where(n => n.Name.Contains(q) ||
                                         n.InterLeagues.interLeaguesName.Contains(q));
            ViewBag.Q = q;
            return View(await query.ToListAsync());
        }

        // GET: /Admin/Users
        public async Task<IActionResult> Users(string q)
        {
            var users = await _userManager.Users.ToListAsync();
            if (!string.IsNullOrWhiteSpace(q))
                users = users.Where(u => u.Email.Contains(q) || u.UserName.Contains(q)).ToList();

            var vms = new List<AdminUserViewModel>();
            foreach (var u in users)
            {
                var roles = await _userManager.GetRolesAsync(u);
                vms.Add(new AdminUserViewModel
                {
                    Id = u.Id,
                    Email = u.Email,
                    UserName = u.UserName,
                    EmailConfirmed = u.EmailConfirmed,
                    Roles = roles.ToList()
                });
            }

            ViewBag.Q = q;
            return View(vms);
        }

        // POST: /Admin/ToggleAdmin
        [HttpPost]
        public async Task<IActionResult> ToggleAdmin(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            // Prevent removing own admin
            var currentUserId = _userManager.GetUserId(User);
            if (userId == currentUserId)
            {
                TempData["Error"] = "You cannot remove your own admin role.";
                return RedirectToAction("Users");
            }

            if (await _userManager.IsInRoleAsync(user, "Admin"))
                await _userManager.RemoveFromRoleAsync(user, "Admin");
            else
                await _userManager.AddToRoleAsync(user, "Admin");

            return RedirectToAction("Users");
        }

        // POST: /Admin/DeleteUser
        [HttpPost]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var currentUserId = _userManager.GetUserId(User);
            if (userId == currentUserId)
            {
                TempData["Error"] = "You cannot delete your own account.";
                return RedirectToAction("Users");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user != null) await _userManager.DeleteAsync(user);
            return RedirectToAction("Users");
        }
    }

    // ── ViewModels ────────────────────────────────────────────
    public class AdminDashboardViewModel
    {
        public int TotalJerseys { get; set; }
        public int TotalClubs { get; set; }
        public int TotalLeagues { get; set; }
        public int TotalNations { get; set; }
        public int TotalInterLeagues { get; set; }
        public int TotalUsers { get; set; }
        public List<Jerseys> RecentJerseys { get; set; }
    }

    public class AdminUserViewModel
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public bool EmailConfirmed { get; set; }
        public List<string> Roles { get; set; }
    }
}