using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Claims;
using wmbaApp.Data;

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

            //get all roles belonging to the user, using userRole IDs
            return appContext.Roles
                    .Where(r => userRoles.Contains(r.Id))
                    .Select(r => r.TeamID)
                    .ToList();
        }

        /// <summary>
        /// Validates that a user is assigned to a team using the teamID parameter
        /// </summary>
        /// <param name="appContext"></param>
        /// <param name="user"></param>
        /// <param name="teamID"></param>
        /// <returns></returns>
        public async static Task<bool> IsAuthorizedForTeam(ApplicationDbContext appContext, ClaimsPrincipal user, int teamID)
        {
            //get user
            var account = await appContext.Users.FirstOrDefaultAsync(u => u.UserName == user.Identity.Name);

            //get all user role IDs
            var userRoles = appContext.UserRoles
                                .Where(u => u.UserId == account.Id)
                                .Select(ur => ur.RoleId)
                                .ToList();

            //get all teamIDs assigned to the roles of the user
            var roleTeamIDs = appContext.Roles
                            .Where(r => userRoles.Contains(r.Id))
                            .Select(r => r.TeamID)
                            .ToList();

            //returns bool if any roleTeamIDs match the value in teamID parameter
            if (roleTeamIDs.Contains(teamID))
                return true;
            else
                return false;
        }

    }
}
