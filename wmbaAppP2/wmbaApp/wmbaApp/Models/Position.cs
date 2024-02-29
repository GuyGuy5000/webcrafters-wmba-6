using System.ComponentModel.DataAnnotations;

namespace wmbaApp.Models
{
    /// <summary>
    /// A class for positions that a player can be assigned
    /// - Nadav Hilu
    /// </summary>
    public class Position
    {
        public int ID { get; set; }
        [Required(ErrorMessage = "A position name is required")]
        [Display(Name = "Position")]
        public string PosName { get; set; }

        [Display(Name = "Player Positions")]
        public ICollection<PlayerPosition> PlayerPositions { get; set; } = new HashSet<PlayerPosition>();
    }
}
