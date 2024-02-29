using System.ComponentModel.DataAnnotations;

namespace wmbaApp.Models
{
    /// <summary>
    /// A class for the divisions that teams are split up into
    /// - Nadav Hilu, Emmanuel James
    /// </summary>
    public class Division
    {
        #region Summaries
        public string Summary
        {
            get
            {
                return $"{DivName} - {Teams.Count} Teams";
            }
        }
        #endregion

        public int ID {  get; set; }
        [Required(ErrorMessage = "A division name is required")]
        [Display(Name = "Division Name")]
        public string DivName { get; set; }

        [Display(Name = "Division Coaches")]
        public ICollection<DivisionCoach> DivisionCoaches { get; set; } = new HashSet<DivisionCoach>();
        public ICollection<Team> Teams { get; set; } = new HashSet<Team>();
    }
}
