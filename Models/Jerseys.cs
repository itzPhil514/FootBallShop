using FootBallShop.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Twilio.TwiML.Voice;

namespace FootBallShop.Models
{
    public class Jerseys
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int JerseysId { get; set; }

        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(10)]
        public string Size { get; set; }

        [Range(0.01, 1000.00)]
        public decimal Price { get; set; }

        public int? LeagueId { get; set; }

        [ForeignKey("LeagueId")]
        public virtual Leagues League { get; set; }

        public int? ClubId { get; set; }

        [ForeignKey("ClubId")]
        public virtual Clubs Club { get; set; }

        public int? NationId { get; set; }

        [ForeignKey("NationId")]
        public virtual Nations Nation { get; set; }

        public int? interLeaguesId { get; set; }

        [ForeignKey("interLeaguesId")]
        public virtual InterLeagues InterLeague { get; set; }

        public string img { get; set; }
        public bool IsInter { get; set; }

        public string Category { get; set; }
    }
}
