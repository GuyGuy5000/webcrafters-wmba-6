using System.ComponentModel.DataAnnotations;

namespace wmbaApp.Models
{
    public class PlayByPlay
    {
        public int PlayByPlayID { get; set; }


        [Display(Name = "Player")]
        public int PlayerID { get; set; }
        public Player Player { get; set; }


        [Display(Name = "Player Action")]
        public int PlayerActionID { get; set; }
        public PlayerAction PlayerAction { get; set; }


        [Display(Name = "Inning")]
        public int InningID { get; set; }
        public Inning Inning { get; set; }

    }
}
