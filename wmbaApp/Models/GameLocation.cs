using System.ComponentModel.DataAnnotations;

namespace wmbaApp.Models
{
    public class GameLocation
    {
        public string Summary => Name;
        public int ID { get; set; }

        [StringLength(50, ErrorMessage = "Game location must be less than 50 characters")]
        [Required(ErrorMessage = "Game must have a Location.")]
        [Display(Name = "Game Location")]
        public string Name { get; set; }



        [Display(Name = "Game:")]
        public ICollection<Game> Games { get; set; } = new HashSet<Game>();
    }
}
