using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace wmbaApp.Models
{
    /// <summary>
    /// Conjunction table for Lineup and the Players
    /// Farooq Jidelola
    /// </summary>
    public class PlayerLineup
    {
        public int ID { get; set; }

        public int LineupID { get; set; }
        public Lineup Lineup { get; set; }


        public int? PlayerID { get; set; }
        public Player Player { get; set; }
    }
}
