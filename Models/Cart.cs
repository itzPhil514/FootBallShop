using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace FootBallShop.Models
{
    public class Cart
    {
        [Key]
        public int CartId { get; set; }

        public int JerseysId { get; set; }
        public Jerseys jersey { get; set; }

        [Required(ErrorMessage = "Quantity is required.")]
        public int Quantity { get; set; }

        public string Size { get; set; }

        public DateTime DateCreation { get; set; }

        public string UserId { get; set; }
        public IdentityUser User { get; set; }
    }
}
