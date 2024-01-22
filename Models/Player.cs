using System.ComponentModel.DataAnnotations;

namespace wmbaApp.Models
{
    /// <summary>
    /// A class for players in a team
    /// - Nadav Hilu
    /// </summary>
    public class Player
    {
        public int ID { get; set; }
        [Required(ErrorMessage = "First name is required")]
        [Display(Name = "First Name")]
        [StringLength(50, ErrorMessage = "First name must be less than 50 characters.")]
        public string PlyrFirstName { get; set; }
        [Display(Name = "Last Name")]
        [StringLength(80, ErrorMessage = "Last name must be less than 80 characters.")]
        public string PlyrLastName { get; set; }
        [Display(Name = "Jersey Number")]
        [Range(1,99, ErrorMessage = "Jeersey numbers are between 1 and 99")]
        public int? PlyrJerseyNumber { get; set; }

        [Display(Name = "Team")]
        public int TmID { get; set; }
        public Team Team { get; set; }
        [Display(Name = "Statistics")]
        public int StatsID { get; set; }
        public Statistics Statistics { get; set; }

        [Display(Name = "Player Positions")]
        public ICollection<PlayerPosition> PlayerPositions { get; set; } = new HashSet<PlayerPosition>();
    }
}
