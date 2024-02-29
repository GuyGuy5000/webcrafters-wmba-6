using System.ComponentModel.DataAnnotations;

namespace wmbaApp.Models
{
    /// <summary>
    /// A class for representing the junction table between the Player and Position tables 
    /// - Nadav Hilu
    /// </summary>
    public class PlayerPosition
    {
        [Display(Name = "Player")]
        public int PlayerID { get; set; }
        public Player Player { get; set; }

        [Display(Name = "Position")]
        public int PositionID { get; set; }
        public Position Position { get; set; }
    }
}
