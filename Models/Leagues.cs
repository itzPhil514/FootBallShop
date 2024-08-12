using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FootBallShop.Models
{
    public class Leagues
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LeagueId { get; set; }

        [Required(ErrorMessage = "League name is required.")]
        [StringLength(100)] // Adding a length constraint to ensure no excessively long names
        public string LeagueName { get; set; }

        public string imgLeaguePath { get; set; }

        // Adding a collection of Clubs to represent the relationship
        public virtual ICollection<Clubs> Clubs { get; set; }

        // Optional: Adding a collection of Jerseys if you use it
        public virtual ICollection<Jerseys> Jersey { get; set; }
    }
}
