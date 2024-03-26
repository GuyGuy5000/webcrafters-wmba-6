using Microsoft.AspNetCore.Identity;

namespace wmbaApp.Models
{
    public class ApplicationRole : IdentityRole
    {
        public int DivID { get; set; }
        public int TeamID { get; set; }


        public ApplicationRole() : base()
        {
            DivID = 0;
            TeamID = 0;
        }

        public ApplicationRole(string roleName) : base(roleName)
        {
            DivID = 0;
            TeamID = 0;
        }

        public ApplicationRole (string roleName, int divID, int teamID) : base (roleName)
        {
            DivID = divID;
            TeamID = teamID;
        }
    }
}
