using System.ComponentModel.DataAnnotations;

namespace wmbaApp.Models
{
    public class PlayerAction
    {
        public int PlayerActionID { get; set; }

        public string PlayerActionName { get; set; }


        [Display(Name = "Play By Play")]
        public ICollection<PlayByPlay> PlayByPlays { get; set; } = new HashSet<PlayByPlay>();

    }
}
