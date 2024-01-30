using System.ComponentModel.DataAnnotations;

namespace wmbaApp.Models
{
    /// <summary>
    /// A class for teams that are in a division
    /// - Nadav Hilu, Emmanuel James
    /// </summary>
    public class Team
    {
        #region Summaries
        [Display(Name = "Team Summary")]
        public string Summary
        {
            get
            {
                return $"{TmName} ({TmAbbreviation?.ToUpper()}), Players: {Players.Count}";
            }
        }
        #endregion

        public int ID { get; set; }
        [Display(Name = "Team Name")]
        [Required(ErrorMessage = "Team name is required")]
        [StringLength(80, ErrorMessage = "Team name must be less than 80 characters.")]
        public string TmName { get; set; }
        [Display(Name = "Team ABBV")]
        [StringLength(3, ErrorMessage = "Team abbreviation is limited to 3 characters.")]
        public string TmAbbreviation { get; set; }

        [Display(Name = "Division")]
        public int DivisionID { get; set; }
        public Division Division { get; set; }

        [Display(Name = "Division Coaches")]
        public ICollection<DivisionCoach> DivisionCoaches { get; set; } = new HashSet<DivisionCoach>();
        public ICollection<Player> Players { get; set; } = new HashSet<Player>();

        [Display(Name = "Game Teams")]
        public ICollection<GameTeam> GameTeams { get; set; } = new HashSet<GameTeam>();

    }
}
