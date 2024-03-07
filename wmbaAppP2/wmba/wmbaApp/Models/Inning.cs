using System.ComponentModel.DataAnnotations;

namespace wmbaApp.Models
{
    public class Inning
    {
        public int InningID { get; set; }

        public int HomeTeamScore { get; set; } = 0;
        public int AwayTeamScore { get; set; } = 0;


        [Display(Name = "Game")]
        public int gameID { get; set; }
        public Game Game { get; set; }

        [Display(Name = "Play By Play")]
        public ICollection<PlayByPlay> PlayByPlays { get; set; } = new HashSet<PlayByPlay>();
    }
}
