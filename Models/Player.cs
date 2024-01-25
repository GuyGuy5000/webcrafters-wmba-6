using System.ComponentModel.DataAnnotations;

namespace wmbaApp.Models
{
    /// <summary>
    /// A class for players in a team
    /// - Nadav Hilu, Emmanuel James
    /// </summary>
    public class Player : IValidatableObject
    {
        #region Summaries
        [Display(Name = "Player Info")]
        public string Summary
        {
            get
            {
                return $"Name: {PlyrFirstName} {PlyrLastName}, Jersey Number: {PlyrJerseyNumber}, Team: {Team?.TmName}";
            }
        }

        [Display(Name = "Full Name")]
        public string FullName
        {
            get
            {
                return $"{PlyrFirstName} {PlyrLastName}";
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
        [Display(Name = "Date of Birth")]
        public DateTime? PlyrDOB { get; set; }

        [Display(Name = "Team")]
        public int TeamID { get; set; }
        public Team Team { get; set; }
        [Display(Name = "Statistics")]
        public int StatsID { get; set; }
        public Statistic Statistics { get; set; }

        [Display(Name = "Player Positions")]
        public ICollection<PlayerPosition> PlayerPositions { get; set; } = new HashSet<PlayerPosition>();

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (PlyrDOB >= DateTime.Now)
            {
                yield return new ValidationResult("Date of birth cannot in the future", new[] { "PlyrDOB" });
            }
        }
    }
}
