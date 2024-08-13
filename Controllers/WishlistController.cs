using Microsoft.AspNetCore.Mvc;
using FootBallShop.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace FootBallShop.Controllers
{
    public class WishlistController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public WishlistController(AppDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Wishlist
        public IActionResult Index()
        {
            var userId = _userManager.GetUserId(User);
            List<Wishlist> wishlistItems;

            if (userId != null)
            {
                wishlistItems = _context.WishlistItems
                    .Include(w => w.jerseys)
                    .Where(w => w.UserId == userId)
                    .ToList();
            }
            else
            {
                wishlistItems = GetGuestWishlistItems();
            }

            return View(wishlistItems);
        }

        // POST: Wishlist/AddToWishlist
        [HttpPost]
        public IActionResult AddToWishlist(int TshirtId)
        {
            var userId = _userManager.GetUserId(User);
            var tshirt = _context.Jersey.FirstOrDefault(t => t.JerseysId == TshirtId);
            if (tshirt == null)
            {
                return NotFound();
            }

            if (userId != null)
            {
                var existingItem = _context.WishlistItems
                    .FirstOrDefault(w => w.jerseys.JerseysId == TshirtId && w.UserId == userId);

                if (existingItem == null)
                {
                    var wishlistItem = new Wishlist
                    {
                        jerseys = tshirt,
                        DateAdded = DateTime.Now,
                        UserId = userId
                    };

                    _context.WishlistItems.Add(wishlistItem);
                    _context.SaveChanges();
                }
            }
            else
            {
                var guestWishlistItems = GetGuestWishlistItems();
                var existingItem = guestWishlistItems.FirstOrDefault(w => w.jerseys.JerseysId == TshirtId);

                if (existingItem == null)
                {
                    guestWishlistItems.Add(new Wishlist
                    {
                        jerseys = tshirt,
                        DateAdded = DateTime.Now
                    });

                    SaveGuestWishlistItems(guestWishlistItems);
                }
            }

            return RedirectToAction("Index");
        }

        // POST: Wishlist/RemoveFromWishlist
        [HttpPost]
        public IActionResult RemoveFromWishlist(int TshirtId)
        {
            var userId = _userManager.GetUserId(User);

            if (userId != null)
            {
                var wishlistItem = _context.WishlistItems
                    .FirstOrDefault(w => w.jerseys.JerseysId == TshirtId && w.UserId == userId);

                if (wishlistItem != null)
                {
                    _context.WishlistItems.Remove(wishlistItem);
                    _context.SaveChanges();
                }
            }
            else
            {
                var guestWishlistItems = GetGuestWishlistItems();
                var wishlistItem = guestWishlistItems.FirstOrDefault(w => w.jerseys.JerseysId == TshirtId);

                if (wishlistItem != null)
                {
                    guestWishlistItems.Remove(wishlistItem);
                    SaveGuestWishlistItems(guestWishlistItems);
                }
            }

            return RedirectToAction("Index");
        }

        // GET: Wishlist/TotalItems
        public IActionResult TotalItems()
        {
            var userId = _userManager.GetUserId(User);
            int totalItems;

            if (userId != null)
            {
                totalItems = _context.WishlistItems.Count(w => w.UserId == userId);
            }
            else
            {
                totalItems = GetGuestWishlistItems().Count;
            }

            return Json(totalItems);
        }

        // Retrieve wishlist items from the guest cookie
        private List<Wishlist> GetGuestWishlistItems()
        {
            var cookie = Request.Cookies["GuestWishlist"];
            if (string.IsNullOrEmpty(cookie))
            {
                return new List<Wishlist>();
            }

            return JsonConvert.DeserializeObject<List<Wishlist>>(cookie);
        }

        // Save wishlist items to the guest cookie
        private void SaveGuestWishlistItems(List<Wishlist> wishlistItems)
        {
            var cookieOptions = new Microsoft.AspNetCore.Http.CookieOptions
            {
                Expires = DateTime.Now.AddDays(30),
                HttpOnly = true,
                Secure = true
            };

            var serializedWishlistItems = JsonConvert.SerializeObject(wishlistItems);
            Response.Cookies.Append("GuestWishlist", serializedWishlistItems, cookieOptions);
        }
    }
}
