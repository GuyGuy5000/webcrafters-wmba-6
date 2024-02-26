using System.ComponentModel.DataAnnotations;

namespace wmbaApp.Models
{
    /// <summary>
    /// A class for generic game information
    /// - Nadav Hilu, Emmanuel James
    /// </summary>
    public class Game : IValidatableObject
    {
        #region Summaries
        [Display(Name = "Game Details")]
        public string Summary
        {
            get
            {
                return $"Starts: {this?.GameStartTime}, Ends: {this?.GameEndTime}, Location: {this?.GameLocation}";
            }
        }

        [Display(Name = "Start")]
        public string TimeSummary
        {
            get
            {
                return GameStartTime?.ToString("h:mm tt") + " to " + this?.GameEndTime;
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
        public DateTime? GameStartTime { get; set; }
<<<<<<< HEAD


        [Display(Name = "Game Ends")]
        public DateTime? GameEndTime { get; set; }
=======
<<<<<<< HEAD
        [Display(Name = "Game Ends")]
        public DateTime? GameEndTime { get; set; }
=======


        [Display(Name = "Game Ends")]
        public DateTime? GameEndTime { get; set; }
>>>>>>> 3b13cb3 (fixed merged solution issue)

        [Display(Name = "Active")]
        public bool IsActive { get; set; } = true;

<<<<<<< HEAD
=======
>>>>>>> 29e156e (fixed merged solution issue)
>>>>>>> 3b13cb3 (fixed merged solution issue)
        [StringLength(50, ErrorMessage = "Game location must be less than 50 characters")]
        public string GameLocation { get; set; }

        [Display(Name = "Game Teams")]
        public ICollection<GameTeam> GameTeams { get; set; } = new HashSet<GameTeam>();


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
        }
    }
}
