using System.ComponentModel.DataAnnotations;

namespace wmbaApp.Models
{
    /// <summary>
    /// A class for individual games that teams are assigned to.
    /// - Nadav Hilu
    /// </summary>
    public class GameTeam
    {
        [Display(Name = "Team")]
        public int TeamID { get; set; }
        public Team Team { get; set; }

        [Display(Name = "Game")]
        public int GameID { get; set; }
        public Game Game { get; set; }

        [Required(ErrorMessage = "A lineup is required")]
        [Display(Name = "Lineup")]
        [StringLength(2000)]
        public string GmtmLineup { get; set; }
        [Display(Name = "Score")]
        public int? GmtmScore { get; set; }
    }
}
