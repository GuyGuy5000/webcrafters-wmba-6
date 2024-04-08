using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Claims;
using wmbaApp.Data;
using wmbaApp.Models;

namespace wmbaApp.Utilities
{
    /// <summary>
    /// Class to retrieve user roles and properties within Identity tables
    /// </summary>
    public static class UserRolesHelper
    {

        /// <summary>
        /// Retrieves all division IDs based on the user parameter
        /// </summary>
        /// <param name="appContext"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public async static Task<List<int>> GetUserDivisionIDs(ApplicationDbContext appContext, ClaimsPrincipal user)
        {
            //get user
            var account = await appContext.Users.FirstOrDefaultAsync(u => u.UserName == user.Identity.Name);

            //get all user role IDs
            var userRoles = appContext.UserRoles
                                .Where(u => u.UserId == account.Id)
                                .Select(ur => ur.RoleId)
                                .ToList();

            //get all roles belonging to the user, using userRole IDs
            return appContext.Roles
                    .Where(r => userRoles.Contains(r.Id))
                    .Select(r => r.DivID)
                    .ToList();
        }


        /// <summary>
        ///  Retrieves all team IDs based on the user parameter
        /// </summary>
        /// <param name="appContext"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public async static Task<List<int>> GetUserTeamIDs(ApplicationDbContext appContext, ClaimsPrincipal user)
        {
            //get user
            var account = await appContext.Users.FirstOrDefaultAsync(u => u.UserName == user.Identity.Name);

            //get all user role IDs
            var userRoles = appContext.UserRoles
                                .Where(u => u.UserId == account.Id)
                                .Select(ur => ur.RoleId)
                                .ToList();

            var roles = appContext.Roles
                            .Where(r => userRoles.Contains(r.Id))
                            .ToList();

            //get all roles belonging to the user, using userRole IDs
            return appContext.Roles
                    .Where(r => userRoles.Contains(r.Id))
                    .Select(r => r.TeamID)
                    .ToList();
        }

        /// <summary>
        /// used to validate that a user is assigned to a division using the division parameter
        /// </summary>
        /// <param name="appContext"></param>
        /// <param name="user"></param>
        /// <param name="division"></param>
        /// <returns>Returns true if user has a role containing the division's ID</returns>
        public async static Task<bool> IsAuthorizedForDivision(ApplicationDbContext appContext, ClaimsPrincipal user, Division division)
        {
            //get user
            var account = await appContext.Users.FirstOrDefaultAsync(u => u.UserName == user.Identity.Name);

            //get all user role IDs
            var userRoles = appContext.UserRoles
                                .Where(u => u.UserId == account.Id)
                                .Select(ur => ur.RoleId)
                                .ToList();

            //get all roles assigned to the roles of the user
            var roles = appContext.Roles
                            .Where(r => userRoles.Contains(r.Id))
                            .ToList();

            //admin role is always authorized
            if (roles.Any(r => r.Name.ToLower() == "admin"))
                return true;

            //returns bool if any roles match the value in teamID parameter
            if (roles.Any(r => r.DivID == division.ID))
                return true;
            else
                return false;
        }


        /// <summary>
        /// used to validate that a user is assigned to a team using the team parameter
        /// </summary>
        /// <param name="appContext"></param>
        /// <param name="user"></param>
        /// <param name="team"></param>
        /// <returns>Returns true if user has a role containing the teamID</returns>
        public async static Task<bool> IsAuthorizedForTeam(ApplicationDbContext appContext, ClaimsPrincipal user, Team team)
        {
            //get user
            var account = await appContext.Users.FirstOrDefaultAsync(u => u.UserName == user.Identity.Name);

            //get all user role IDs
            var userRoles = appContext.UserRoles
                                .Where(u => u.UserId == account.Id)
                                .Select(ur => ur.RoleId)
                                .ToList();

            //get all roles assigned to the roles of the user
            var roles = appContext.Roles
                            .Where(r => userRoles.Contains(r.Id))
                            .ToList();

            if (roles.Any(r => r.Name.ToLower() == "admin")) //admin role is always authorized
                return true;

            if (roles.Any(r => r.DivID == team.DivisionID)) //if a role has access to the whole division, the team is included
                return true;

            //returns bool if any roleTeamIDs match the value in teamID parameter
            if (roles.Any(r => r.TeamID == team.ID))
                return true;
            else
                return false;
        }

        /// <summary>
        /// used to validate that a user is assigned to a game using the game parameter
        /// </summary>
        /// <param name="appContext"></param>
        /// <param name="user"></param>
        /// <param name="game"></param>
        /// <returns>Returns true if user has a role containing the teams in the game</returns>
        public async static Task<bool> IsAuthorizedForGame(ApplicationDbContext appContext, ClaimsPrincipal user, Game game)
        {
            //get user
            var account = await appContext.Users.FirstOrDefaultAsync(u => u.UserName == user.Identity.Name);

            //get all user role IDs
            var userRoles = appContext.UserRoles
                                .Where(u => u.UserId == account.Id)
                                .Select(ur => ur.RoleId)
                                .ToList();

            //get all roles assigned to the roles of the user
            var roles = appContext.Roles
                            .Where(r => userRoles.Contains(r.Id))
                            .ToList();

            if (roles.Any(r => r.Name.ToLower() == "admin")) //admin role is always authorized
                return true;

            if (roles.Any(r => r.DivID == game.DivisionID)) //if a role has access to the whole division, the team is included
                return true;

            //returns bool if any roleTeamIDs match the value in teamID parameter
            if (roles.Any(r => r.TeamID == game.AwayTeamID || r.TeamID == game.HomeTeamID))
                return true;
            else
                return false;
        }


        /// <summary>
        /// used to validate that a user is assigned to a player using the player parameter
        /// </summary>
        /// <param name="appContext"></param>
        /// <param name="user"></param>
        /// <param name="player"></param>
        /// <returns>Returns true if user has a role containing the teams in the game</returns>
        public async static Task<bool> IsAuthorizedForPlayer(ApplicationDbContext appContext, ClaimsPrincipal user, Player player)
        {
            //get user
            var account = await appContext.Users.FirstOrDefaultAsync(u => u.UserName == user.Identity.Name);

            //get all user role IDs
            var userRoles = appContext.UserRoles
                                .Where(u => u.UserId == account.Id)
                                .Select(ur => ur.RoleId)
                                .ToList();

            //get all roles assigned to the roles of the user
            var roles = appContext.Roles
                            .Where(r => userRoles.Contains(r.Id))
                            .ToList();

            if (roles.Any(r => r.Name.ToLower() == "admin")) //admin role is always authorized
                return true;

            if (roles.Any(r => r.DivID == player.Team.DivisionID)) //if a role has access to the whole division, the team is included
                return true;

            //returns bool if any roleTeamIDs match the value in teamID parameter
            if (roles.Any(r => r.TeamID == player.TeamID))
                return true;
            else
                return false;
        }
    }
}
