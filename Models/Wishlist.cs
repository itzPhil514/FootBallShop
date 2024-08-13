using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace FootBallShop.Models
{
    public class Wishlist
    {
        [Key]
        public int WishlistId { get; set; }

        [Required]
        public int jerseysId { get; set; }
        public Jerseys jerseys { get; set; }

        [Required]
        public string UserId { get; set; }
        public IdentityUser User { get; set; }

        public DateTime DateAdded { get; set; }
    }
}
