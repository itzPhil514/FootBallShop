using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FootBallShop.Models
{
    public class Nations
    {
        [Key]
        public int NationId { get; set; }

        [Required(ErrorMessage = "Nation name is required.")]
        [StringLength(100)]
        public string Name { get; set; }

        [Required(ErrorMessage = "interLeaguesId is required.")]
        public int interLeaguesId { get; set; }

        [ForeignKey("interLeaguesId")]
        public virtual InterLeagues InterLeagues { get; set; }

        public string img { get; set; }

        public virtual ICollection<Jerseys> Jerseys { get; set; }
    }
}
