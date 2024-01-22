using System.ComponentModel.DataAnnotations;

namespace wmbaApp.Models
{
    /// <summary>
    /// A class for individual games that teams are assigned to.
    /// - Nadav Hilu
    /// - Emmanuel James
    /// </summary>
    public class GameTeam
    {
        [Display(Name = "Game Team Summary")]
        public string Summary
        {
            get
            {
                return $"Team: {Team?.TeamName}, Game ID: {Game?.ID}, Lineup: {GmtmLineup}, Score: {GmtmScore ?? 0}";
            }
        }

        [Display(Name = "Team")]
        public int TmID { get; set; }
        public Team Team { get; set; }

        [Display(Name = "Game")]
        public int GmID { get; set; }
        public Game Game { get; set; }

        [Required(ErrorMessage = "A lineup is required")]
        [Display(Name = "Lineup")]
        [StringLength(2000)]
        public string GmtmLineup { get; set; }
        [Display(Name = "Score")]
        public int? GmtmScore { get; set; }
    }
}
