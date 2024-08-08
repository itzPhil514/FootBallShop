using FootBallShop.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FootBallShop2.Models
{
    public class Leagues
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LeagueId { get; set; }

        [Required(ErrorMessage = "League name is required.")]
        public string LeagueName { get; set; }

        [NotMapped]
        public IFormFile imgLeague { get; set; }
        public string imgLeaguePath { get; set; }

        public virtual ICollection<Jerseys> Jersey { get; set; }
    }
}
