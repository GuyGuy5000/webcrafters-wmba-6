using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using wmbaApp.Data;
using wmbaApp.ViewModels;

namespace wmbaApp.Models
{
    /// <summary>
    /// A class for generic game information
    /// - Nadav Hilu, Emmanuel James, Farooq Jidelola
    /// </summary>
    public class Game : IValidatableObject
    {

        #region Summaries

        [Display(Name = "Games")]
        public string FullVersus
        {
            get
            {
                if (HomeTeam != null && AwayTeam != null)
                {
                    return $"{HomeTeam.TmName} VS {AwayTeam.TmName}";
                }
                else if (HomeTeam != null)
                {
                    return $"{HomeTeam.TmName} VS. TBA";
                }
                else if (AwayTeam != null)
                {
                    return $"TBA VS. {AwayTeam.TmName}";
                }
                else
                {
                    return "TBA VS. TBA";
                }
            }
        }

        [Display(Name = "Date")]
        public string Summary
        {
            get
            {
                return $" {this?.StartTimeSummary}";
            }
        }

        [Display(Name = "Start")]
        public string StartTimeSummary
        {
            get
            {
                return GameStartTime.Value.ToString("MMMM d yyyy hh:mmtt");
            }
        }

        [Display(Name = "End")]
        public string EndTimeSummary
        {
            get
            {
                if (GameEndTime == null)
                {
                    return "Unknown";
                }
                else
                {
                    string endtime = GameEndTime.GetValueOrDefault().ToString("h:mm tt");
                    TimeSpan difference = ((TimeSpan)(GameEndTime - GameStartTime));
                    int days = difference.Days;
                    if (days > 0)
                    {
                        return endtime + " (" + days + " day" + (days > 1 ? "s" : "") + " later)";
                    }
                    else
                    {
                        return endtime;
                    }
                }
            }
        }

        [Display(Name = "Game Duration")]
        public string Duration
        {
            get
            {
                if (GameEndTime == null)
                {
                    return "";
                }
                else
                {
                    TimeSpan d = ((TimeSpan)(GameEndTime - GameStartTime));
                    string duration = "";
                    if (d.Minutes > 0) //Shows the minutes if there are any
                    {
                        duration = d.Minutes.ToString() + " min";
                    }
                    if (d.Hours > 0) //Shows the hours if there are any
                    {
                        duration = d.Hours.ToString() + " hr" + (d.Hours > 1 ? "s" : "")
                            + (d.Minutes > 0 ? ", " + duration : ""); //Puts a ", " between hours and minutes if there are both
                    }
                    if (d.Days > 0) //Shows the days if there are any
                    {
                        duration = d.Days.ToString() + " day" + (d.Days > 1 ? "s" : "")
                            + (d.Hours > 0 || d.Minutes > 0 ? ", " + duration : ""); //Puts a ", " between days and hours/minutes if there are any
                    }

                    return duration;
                }
            }
        }
        #endregion

        public int ID { get; set; }

        [Display(Name = "Game Starts")]
        [Required(ErrorMessage = "Set a Start Time")]
        public DateTime? GameStartTime { get; set; }

        [Display(Name = "Game Ends")]
        [Required(ErrorMessage = "Set an End Time")]
        public DateTime? GameEndTime { get; set; }

        [Display(Name = "Game Location")]
        public int GameLocationID { get; set; }
        [Display(Name = "Game Location:")]
        public GameLocation GameLocation { get; set; }

        [Display(Name = "Home Team Score")]
        public int HomeTeamScore { get; set; } = 0;

        [Display(Name = "Visitor Team Score")]
        public int AwayTeamScore { get; set; } = 0;

        [Display(Name = "Current Inning")]
        public int CurrentInning { get; set; } = 0;

        [Display(Name = "Game Status")]
        public bool HasStarted { get; set; } = false;



        [Display(Name = "Home:")]
        [Required(ErrorMessage = "Home Team name is required")]
        public int HomeTeamID { get; set; }
        [ForeignKey("HomeTeamID")]
        [Display(Name = "Home:")]
        public Team HomeTeam { get; set; }

        [Display(Name = "Visitor:")]
        [Required(ErrorMessage = "Visitor Team name is required")]
        public int AwayTeamID { get; set; }
        [ForeignKey("AwayTeamID")]
        [Display(Name = "Visitor:")]
        public Team AwayTeam { get; set; }

        [Display(Name = "Division:")]
        public int DivisionID { get; set; }

        [ForeignKey("DivisionID")]
        [Display(Name = "Division:")]
        public Division Division { get; set; }



        [Display(Name = "Game Teams")]
        public ICollection<GameTeam> GameTeams { get; set; } = new HashSet<GameTeam>();



        [Display(Name = "Home Lineup")]
        public int? HomeLineupID { get; set; }
        public Lineup HomeLineup { get; set; }



        [Display(Name = "Visitor Lineup")]
        public int? AwayLineupID { get; set; }
        public Lineup AwayLineup { get; set; }


        [Display(Name = "Innings")]
        public ICollection<Inning> Innings { get; set; } = new HashSet<Inning>();

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // Game Date cannot be before January 1st, 2024.
            if (GameStartTime < DateTime.Parse("2024-01-01"))
            {
                yield return new ValidationResult("Date cannot be before January 1st, 2024.", new[] { "GameStartTime" });
            }

            // Game Date cannot be more than 1 year in the future from the current date.
            if (GameStartTime > DateTime.Now.AddYears(1))
            {
                yield return new ValidationResult("Date cannot be more than 1 year in the future.", new[] { "GameStartTime" });
            }

            // Game cannot end before it starts
            if (GameEndTime < GameStartTime)
            {
                yield return new ValidationResult("Game cannot end before it starts.", new[] { "GameEndTime" });
            }

            if (HomeTeamID == AwayTeamID)
            {
                yield return new ValidationResult("Home Team and Visitor Team cannot be the same Team.", new[] { "HomeTeamID", "AwayTeamID" });
            }

        }
    }
}