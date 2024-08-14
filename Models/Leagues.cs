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
        [StringLength(100)]
        public string LeagueName { get; set; }

        public string imgLeaguePath { get; set; }

        public virtual ICollection<Clubs> Clubs { get; set; }

        public virtual ICollection<Jerseys> Jersey { get; set; }
    }
}
