using System.ComponentModel.DataAnnotations;

namespace wmbaApp.Models
{
    public class InActivePlayer
    {
        public int Id { get; set; }

        [Display(Name = "Full Name")]
        public string FullName
        {
            get
            {
                return $"{FirstName} {this?.LastName}";
            }
        }
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public int? JerseyNumber { get; set; }

        public string MemberID { get; set; }

        public int TeamID { get; set; }
        public Team Teams { get; set; }

        public int StatisticID { get; set; }
        public Statistic Statistics { get; set; }
    }
}
