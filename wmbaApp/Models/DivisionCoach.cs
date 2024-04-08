using System.ComponentModel.DataAnnotations;

namespace wmbaApp.Models
{
    /// <summary>
    /// A class for representing the junction table between the Division and Coach tables 
    /// - Nadav Hilu
    /// </summary>
    public class DivisionCoach
    {
        [Display(Name = "Division")]
        public int DivisionID { get; set; }
        public Division Division { get; set; }

        [Display(Name = "Coach")]
        public int CoachID { get; set; }
        public Coach Coach { get; set; }

        [Display(Name = "Team")]
        public int TeamID { get; set; }
        public Team Team { get; set; }

    }
}
