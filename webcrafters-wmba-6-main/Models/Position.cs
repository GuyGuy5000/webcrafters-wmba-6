using System.ComponentModel.DataAnnotations;

namespace wmbaApp.Models
{
    /// <summary>
    /// A class for positions that a player can be assigned
    /// - Nadav Hilu
    /// - Emmanuel James
    /// </summary>
    public class Position
    {

        [Display(Name = "Position Summary")]
        public string Summary
        {
            get
            {
                return $"ID: {ID}, Position: {PosName}";
            }
        }

        public int ID { get; set; }
        [Required(ErrorMessage = "A position name is required")]
        [Display(Name = "Position")]
        public string PosName { get; set; }

        [Display(Name = "Player Positions")]
        public ICollection<PlayerPosition> PlayerPositions { get; set; } = new HashSet<PlayerPosition>();
    }
}
