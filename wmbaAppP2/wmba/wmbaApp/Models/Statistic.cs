using System.ComponentModel.DataAnnotations;

namespace wmbaApp.Models
{
    /// <summary>
    /// A class for all statistics that need to be tracked for players
    /// - Nadav Hilu
    /// </summary>
    public class Statistic
    {
        public int ID { get; set; }
        [Display(Name = "GP")]
        public int? StatsGP { get; set; } = 0;
        [Display(Name = "PA")]
        public int? StatsPA { get; set; } = 0;
        [Display(Name = "AB")]
        public int? StatsAB { get; set; } = 0;
        [Range(0, 1, ErrorMessage = "AVG must be between 0 and 1")]
        [Display(Name = "AVG")]
        public double? StatsAVG { get; set; } = 0d;
        [Range(0, 1, ErrorMessage = "OBP must be between 0 and 1")]
        [Display(Name = "OBP")]
        public double? StatsOBP { get; set; } = 0d;
        [Range(0, 1, ErrorMessage = "OPS must be between 0 and 1")]
        [Display(Name = "OPS")]
        public double? StatsOPS { get; set; } = 0d;
        [Range(0, 1, ErrorMessage = "SLG must be between 0 and 1")]
        [Display(Name = "SLG")]
        public double? StatsSLG { get; set; } = 0d;
        [Display(Name = "H")]
        public int? StatsH { get; set; } = 0;
        [Display(Name = "R")]
        public int? StatsR { get; set; } = 0;
        [Display(Name = "K")]
        public int? StatsK { get; set; } = 0;
        [Display(Name = "HR")]
        public int? StatsHR { get; set; } = 0;
        [Display(Name = "RBI")]
        public int? StatsRBI { get; set; } = 0;
        [Display(Name = "BB")]
        public int? StatsBB { get; set; } = 0;
        [Display(Name = "Rating")]
        public int Rating { get; set; }
        public ICollection<Player> Players { get; set; } = new HashSet<Player>();
    }
}
