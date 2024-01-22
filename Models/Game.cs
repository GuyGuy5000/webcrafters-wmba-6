using System.ComponentModel.DataAnnotations;

namespace wmbaApp.Models
{
    /// <summary>
    /// A class for generic game information
    /// </summary>
    public class Game
    {
        public int ID { get; set; }
        [Display(Name = "Game Starts")]
        public DateTime GameStartTime { get; set; }
        [Display(Name = "Game Ends")]
        public DateTime GameEndTime { get; set; }
        [StringLength(50, ErrorMessage = "Game location must be less than 50 characters")]
        public string GameLocation { get; set; }

        [Display(Name = "Game Teams")]
        public ICollection<GameTeam> GameTeams { get; set; } = new HashSet<GameTeam>();
    }
}
