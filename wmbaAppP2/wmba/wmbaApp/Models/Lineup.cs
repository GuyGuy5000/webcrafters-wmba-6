// <summary>
/// Farooq Jidelola
/// </summary>
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace wmbaApp.Models
{
    public class Lineup
    {
        public int ID { get; set; }

        public ICollection<PlayerLineup> PlayerLineups { get; set; } = new HashSet<PlayerLineup>();


        [Display(Name = "Home Team Lineup:")]
        [InverseProperty("HomeLineup")]
        public ICollection<Game> HomeGames { get; set; } = new HashSet<Game>();



        [Display(Name = "Visitor Team Lineup:")]
        [InverseProperty("AwayLineup")]
        public ICollection<Game> AwayGames { get; set; } = new HashSet<Game>();
    }
}
