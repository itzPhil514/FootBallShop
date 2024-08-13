using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Twilio.TwiML.Voice;
using FootBallShop.Models;

namespace FootBallShop.Models
{
    public class Clubs
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ClubId { get; set; }

        [Required(ErrorMessage = "Club name is required.")]
        [StringLength(100)]
        public string Name { get; set; }

        [Required(ErrorMessage = "League is required.")]
        public int LeagueId { get; set; }

        [ForeignKey("LeagueId")]
        public virtual Leagues League { get; set; }

        public string img { get; set; }

        public virtual ICollection<Jerseys> Jersey { get; set; }
    }
}

