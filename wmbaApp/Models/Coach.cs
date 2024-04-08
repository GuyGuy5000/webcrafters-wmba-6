using System.ComponentModel.DataAnnotations;

namespace wmbaApp.Models
{
    /// <summary>
    /// A class for coaches. This class will be coupled with the User table from .NET Identity to create a "coach" role in the future
    /// - Nadav Hilu, Emmanuel James
    /// </summary>
    public class Coach
    {
        #region Summaries
        [Display(Name = "Team Details")]
        public string Summary
        {
            get
            {
                return $"{this?.CoachFirstName} {this?.CoachLastName}, Email: {this?.CoachEmail}, Phone: {this?.CoachPhone}";
            }
        }


        [Display(Name = "Full Name")]
        public string FullName
        {
            get
            {
                return $"{this?.CoachFirstName} {this?.CoachLastName}";
            }
        }


        [Display(Name = "Phone")]
        public string PhoneFormatted
        {
            get
            {
                if (!String.IsNullOrEmpty(CoachPhone))
                    return "(" + CoachPhone.Substring(0, 3) + ") " + CoachPhone.Substring(3, 3) + "-" + CoachPhone[6..];
                else
                    return "";
            }
        }
        #endregion
        public int ID { get; set; }
        [Display(Name = "First Name")]
        [StringLength(50, ErrorMessage = "First name must be less than 50 characters.")]
        public string CoachFirstName { get; set; }
        [Display(Name = "Last Name")]
        [StringLength(80, ErrorMessage = "Last name must be less than 80 characters.")]
        public string CoachLastName { get; set; }
        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string CoachEmail { get; set; }
        [Display(Name = "Phone")]
        [StringLength(10,MinimumLength = 10, ErrorMessage = "Phone must be 10 digits")]
        [Phone(ErrorMessage = "Invalid phone number")]
        public string CoachPhone { get; set; }

        [Display(Name = "Coach")]
        public ICollection<DivisionCoach> DivisionCoaches { get; set; } = new HashSet<DivisionCoach>();
    }
}
