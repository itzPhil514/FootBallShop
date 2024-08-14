using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FootBallShop.Models
{
    public class InterLeagues
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int interLeaguesId { get; set; }

        [Required(ErrorMessage = "International League name is required")]
        public string interLeaguesName { get; set;}
        
        public string img {get; set;}

        public virtual ICollection<Jerseys> Jersey { get; set; }
        public virtual ICollection<Nations> Nation { get; set; }

    }
}
