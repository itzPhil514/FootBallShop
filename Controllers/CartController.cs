using Microsoft.AspNetCore.Mvc;
using FootBallShop.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace FootBallShop.Controllers
{
    public class CartController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public CartController(AppDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Cart
        public IActionResult Index()
        {
            var userId = _userManager.GetUserId(User);
            var cartItems = new List<Cart>();

            if (userId != null)
            {
                cartItems = _context.CartItems
                    .Include(c => c.jersey)
                    .Where(c => c.UserId == userId)
                    .ToList();
            }
            else
            {
                var guestCartItems = GetGuestCartItems();
                if (guestCartItems != null)
                {
                    cartItems = guestCartItems;
                }
            }

            return View(cartItems);
        }

        // POST: Cart/AddToCart
        [HttpPost]
        public IActionResult AddToCart(int TshirtId, string size, int quantity)
        {
            var userId = _userManager.GetUserId(User);
            var tshirt = GetTshirtById(TshirtId);
            if (tshirt == null)
            {
                return NotFound();
            }

            if (userId != null)
            {
                var cartItem = _context.CartItems
                    .FirstOrDefault(c => c.JerseysId == TshirtId && c.Size == size && c.UserId == userId);
                if (cartItem != null)
                {
                    cartItem.Quantity += quantity;
                }
                else
                {
                    _context.CartItems.Add(new Cart
                    {
                        JerseysId = TshirtId,
                        jersey = tshirt,
                        Quantity = quantity,
                        Size = size,
                        DateCreation = DateTime.Now,
                        UserId = userId
                    });
                }

                _context.SaveChanges();
            }
            else
            {
                var guestCartItems = GetGuestCartItems() ?? new List<Cart>();
                var cartItem = guestCartItems
                    .FirstOrDefault(c => c.JerseysId == TshirtId && c.Size == size);
                if (cartItem != null)
                {
                    cartItem.Quantity += quantity;
                }
                else
                {
                    guestCartItems.Add(new Cart
                    {
                        JerseysId = TshirtId,
                        jersey = tshirt,
                        Quantity = quantity,
                        Size = size,
                        DateCreation = DateTime.Now
                    });
                }

                SaveGuestCartItems(guestCartItems);
            }

            return RedirectToAction("Index");
        }

        // POST: Cart/RemoveFromCart
        [HttpPost]
        public IActionResult RemoveFromCart(int TshirtId, string size)
        {
            var userId = _userManager.GetUserId(User);

            if (userId != null)
            {
                var cartItem = _context.CartItems
                    .FirstOrDefault(c => c.JerseysId == TshirtId && c.Size == size && c.UserId == userId);
                if (cartItem != null)
                {
                    _context.CartItems.Remove(cartItem);
                    _context.SaveChanges();
                }
            }
            else
            {
                var guestCartItems = GetGuestCartItems();
                var cartItem = guestCartItems
                    .FirstOrDefault(c => c.JerseysId == TshirtId && c.Size == size);
                if (cartItem != null)
                {
                    guestCartItems.Remove(cartItem);
                    SaveGuestCartItems(guestCartItems);
                }
            }

            return RedirectToAction("Index");
        }

        // POST: Cart/UpdateQuantity
        [HttpPost]
        public IActionResult UpdateQuantity(int TshirtId, string size, int quantity)
        {
            var userId = _userManager.GetUserId(User);

            if (userId != null)
            {
                var cartItem = _context.CartItems
                    .FirstOrDefault(c => c.JerseysId == TshirtId && c.Size == size && c.UserId == userId);
                if (cartItem != null)
                {
                    cartItem.Quantity = quantity;
                    if (quantity <= 0)
                    {
                        _context.CartItems.Remove(cartItem);
                    }
                    _context.SaveChanges();
                }
            }
            else
            {
                var guestCartItems = GetGuestCartItems();
                var cartItem = guestCartItems
                    .FirstOrDefault(c => c.JerseysId == TshirtId && c.Size == size);
                if (cartItem != null)
                {
                    cartItem.Quantity = quantity;
                    if (quantity <= 0)
                    {
                        guestCartItems.Remove(cartItem);
                    }
                    SaveGuestCartItems(guestCartItems);
                }
            }

            return RedirectToAction("Index");
        }

        // POST: Cart/IncreaseQuantity
        [HttpPost]
        public IActionResult IncreaseQuantity(int TshirtId, string size)
        {
            var userId = _userManager.GetUserId(User);

            if (userId != null)
            {
                var cartItem = _context.CartItems
                    .FirstOrDefault(c => c.JerseysId == TshirtId && c.Size == size && c.UserId == userId);
                if (cartItem != null)
                {
                    cartItem.Quantity++;
                    _context.SaveChanges();
                }
            }
            else
            {
                var guestCartItems = GetGuestCartItems();
                var cartItem = guestCartItems
                    .FirstOrDefault(c => c.JerseysId == TshirtId && c.Size == size);
                if (cartItem != null)
                {
                    cartItem.Quantity++;
                    SaveGuestCartItems(guestCartItems);
                }
            }

            return RedirectToAction("Index");
        }

        // POST: Cart/DecreaseQuantity
        [HttpPost]
        public IActionResult DecreaseQuantity(int TshirtId, string size)
        {
            var userId = _userManager.GetUserId(User);

            if (userId != null)
            {
                var cartItem = _context.CartItems
                    .FirstOrDefault(c => c.JerseysId == TshirtId && c.Size == size && c.UserId == userId);
                if (cartItem != null)
                {
                    if (cartItem.Quantity > 1)
                    {
                        cartItem.Quantity--;
                    }
                    else
                    {
                        _context.CartItems.Remove(cartItem);
                    }
                    _context.SaveChanges();
                }
            }
            else
            {
                var guestCartItems = GetGuestCartItems();
                var cartItem = guestCartItems
                    .FirstOrDefault(c => c.JerseysId == TshirtId && c.Size == size);
                if (cartItem != null)
                {
                    if (cartItem.Quantity > 1)
                    {
                        cartItem.Quantity--;
                    }
                    else
                    {
                        guestCartItems.Remove(cartItem);
                    }
                    SaveGuestCartItems(guestCartItems);
                }
            }

            return RedirectToAction("Index");
        }

        // GET: Cart/TotalQuantity
        public IActionResult TotalQuantity()
        {
            var userId = _userManager.GetUserId(User);
            int totalQuantity = 0;

            if (userId != null)
            {
                totalQuantity = _context.CartItems
                    .Where(c => c.UserId == userId)
                    .Sum(c => c.Quantity);
            }
            else
            {
                var guestCartItems = GetGuestCartItems();
                if (guestCartItems != null)
                {
                    totalQuantity = guestCartItems.Sum(c => c.Quantity);
                }
            }

            return Json(totalQuantity);
        }

        private Jerseys GetTshirtById(int id)
        {
            return _context.Jersey.FirstOrDefault(t => t.JerseysId == id);
        }

        // POST: Cart/Checkout
        [HttpPost]
        public IActionResult Checkout()
        {
            var userId = _userManager.GetUserId(User);

            if (userId != null)
            {
                var cartItems = _context.CartItems
                    .Where(c => c.UserId == userId)
                    .ToList();

                // Payment logic here

                _context.CartItems.RemoveRange(cartItems);
                _context.SaveChanges();
            }
            else
            {
                var guestCartItems = GetGuestCartItems();

                // Payment logic here

                if (guestCartItems != null)
                {
                    guestCartItems.Clear();
                    SaveGuestCartItems(guestCartItems);
                }
            }

            return RedirectToAction("Index");
        }

        private List<Cart> GetGuestCartItems()
        {
            var cookie = Request.Cookies["GuestCart"];
            if (string.IsNullOrEmpty(cookie))
            {
                return new List<Cart>();
            }

            return JsonConvert.DeserializeObject<List<Cart>>(cookie);
        }

        private void SaveGuestCartItems(List<Cart> cartItems)
        {
            var cookieOptions = new CookieOptions
            {
                Expires = DateTime.Now.AddDays(30),
                HttpOnly = true,
                Secure = true
            };

            var serializedCartItems = JsonConvert.SerializeObject(cartItems);
            Response.Cookies.Append("GuestCart", serializedCartItems, cookieOptions);
        }
    }
}
