using System.ComponentModel.DataAnnotations;
using wmbaApp.ViewModels;

namespace wmbaApp.Models
{
    /// <summary>
    /// A class for players in a team
    /// - Nadav Hilu, Emmanuel James
    /// </summary>
    public class Player
    {
        #region Summaries
        [Display(Name = "Player Info")]
        public string Summary
        {
            get
            {
                return $"Name: {PlyrFirstName} {this?.PlyrLastName}, Jersey Number: {this?.PlyrJerseyNumber}, Team: {Team?.TmName}";
            }
        }

        [Display(Name = "Full Name")]
        public string FullName
        {
            get
            {
                return $"{PlyrFirstName} {this?.PlyrLastName}";
            }
        }

        [Display(Name = "Game Player")]
        public string AssignPlayer
        {
            get
            {
                return $"{PlyrFirstName} {this?.PlyrLastName} - [Jersey# : {this?.PlyrJerseyNumber} ]";
            }
        }
        #endregion

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
        [Display(Name = "Member ID")]
        [RegularExpression("^.{8}$", ErrorMessage = "Member ID is 8 characters consisting of letters and numbers.")]
        [Required(ErrorMessage = "Member ID is required")]
        public string PlyrMemberID { get; set; }
        [Display(Name = "Active")]
        public bool IsActive { get; set; } = true;
        [Display(Name = "Team")]
        public int TeamID { get; set; }
        public Team Team { get; set; }
        [Display(Name = "Statistics")]
        public int? StatisticID { get; set; }
        public Statistic Statistics { get; set; }



        [Display(Name = "Player Lineups")]
        public ICollection<PlayerLineup> PlayerLineups { get; set; } = new HashSet<PlayerLineup>();

    }
}
