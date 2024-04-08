using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using wmbaApp.Models;

namespace wmbaApp.Data
{
    public static class ApplicationDbInitializer
    {
        public static async void Seed(IApplicationBuilder applicationBuilder)
        {
            ApplicationDbContext context = applicationBuilder.ApplicationServices.CreateScope()
                .ServiceProvider.GetRequiredService<ApplicationDbContext>();
            try
            {
                //Creating database and apply migration if database does not exist

                //Creating Roles
                var RoleManager = applicationBuilder.ApplicationServices.CreateScope()
                    .ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
                //RookieConvenor = RookieC, IntermediateC = IntermediateConvenor
                string[] roleNames = { "Admin", "Coach", "ScoreKeeper", "Convenor", "9U Convenor", "11U Convenor", "13U Convenor", "15U Convenor", "18U Convenor" };

                IdentityResult roleResult;
                foreach (var roleName in roleNames)
                {
                    var roleExist = await RoleManager.RoleExistsAsync(roleName);
                    if (!roleExist)
                    {
                        if (roleName == "9U Convenor")
                            roleResult = await RoleManager.CreateAsync(new ApplicationRole(roleName, 1, 0)); //hard coded IDs for demo purposes
                        if (roleName == "11U Convenor")
                            roleResult = await RoleManager.CreateAsync(new ApplicationRole(roleName, 2, 0)); //hard coded IDs for demo purposes
                        if (roleName == "13U Convenor")
                            roleResult = await RoleManager.CreateAsync(new ApplicationRole(roleName, 3, 0)); //hard coded IDs for demo purposes
                        if (roleName == "15U Convenor")
                            roleResult = await RoleManager.CreateAsync(new ApplicationRole(roleName, 4, 0)); //hard coded IDs for demo purposes
                        if (roleName == "18U Convenor")
                            roleResult = await RoleManager.CreateAsync(new ApplicationRole(roleName, 5, 0)); //hard coded IDs for demo purposes
                        else
                            roleResult = await RoleManager.CreateAsync(new ApplicationRole(roleName));
                    }
                }

                //Creating Users
                var userManager = applicationBuilder.ApplicationServices.CreateScope()
                    .ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

                //User:Admin
                if (userManager.FindByEmailAsync("admin@outlook.com").Result == null)
                {
                    IdentityUser user = new IdentityUser
                    {
                        UserName = "admin@outlook.com",
                        Email = "admin@outlook.com",
                        EmailConfirmed = true
                    };

                    IdentityResult result = userManager.CreateAsync(user, "Wmba@team6").Result;

                    if (result.Succeeded)
                    {
                        userManager.AddToRoleAsync(user, "Admin").Wait();
                    }
                }

                //User:Coach:Navy Mustangs
                if (userManager.FindByEmailAsync("lwatts@outlook.com").Result == null)
                {
                    IdentityUser user = new IdentityUser
                    {
                        UserName = "lwatts@outlook.com",
                        Email = "lwatts@outlook.com",
                        EmailConfirmed = true
                    };

                    IdentityResult result = userManager.CreateAsync(user, "Wmba@team6").Result;

                    if (result.Succeeded)
                    {
                        userManager.AddToRoleAsync(user, "Coach").Wait();
                    }
                }

                //User:RookieConvenor
                if (userManager.FindByEmailAsync("rookie1convenor@outlook.com").Result == null)
                {
                    IdentityUser user = new IdentityUser
                    {
                        UserName = "rookie1convenor@outlook.com",
                        Email = "rookie1convenor@outlook.com",
                        EmailConfirmed = true
                    };

                    IdentityResult result = userManager.CreateAsync(user, "Wmba@team6").Result;

                    if (result.Succeeded)
                    {
                        userManager.AddToRoleAsync(user, "Convenor").Wait();
                        userManager.AddToRoleAsync(user, "9U Convenor").Wait();
                    }
                }

                //User:IntermediateConvenor
                if (userManager.FindByEmailAsync("intermediate1convenor@outlook.com").Result == null)
                {
                    IdentityUser user = new IdentityUser
                    {
                        UserName = "intermediate1convenor@outlook.com",
                        Email = "intermediate1convenor@outlook.com",
                        EmailConfirmed = true
                    };

                    IdentityResult result = userManager.CreateAsync(user, "Wmba@team6").Result;

                    if (result.Succeeded)
                    {
                        userManager.AddToRoleAsync(user, "Convenor").Wait();
                        userManager.AddToRoleAsync(user, "11U Convenor").Wait();
                        userManager.AddToRoleAsync(user, "13U Convenor").Wait();
                    }
                }

                //User:Convenor
                if (userManager.FindByEmailAsync("seniorconvenor@outlook.com").Result == null)
                {
                    IdentityUser user = new IdentityUser
                    {
                        UserName = "seniorconvenor@outlook.com",
                        Email = "seniorconvenor@outlook.com",
                        EmailConfirmed = true
                    };

                    IdentityResult result = userManager.CreateAsync(user, "Wmba@team6").Result;

                    if (result.Succeeded)
                    {
                        userManager.AddToRoleAsync(user, "Convenor").Wait();
                        userManager.AddToRoleAsync(user, "15U Convenor").Wait();
                        userManager.AddToRoleAsync(user, "18U Convenor").Wait();
                    }
                }


                //User:Me
                if (userManager.FindByEmailAsync("fjidelola1@ncstudents.niagaracollege.ca").Result == null)
                {
                    IdentityUser user = new IdentityUser
                    {
                        UserName = "fjidelola1@ncstudents.niagaracollege.ca",
                        Email = "fjidelola1@ncstudents.niagaracollege.ca",
                        EmailConfirmed = true
                    };

                    IdentityResult result = userManager.CreateAsync(user, "Wmba@team6").Result;

                    if (result.Succeeded)
                    {
                        userManager.AddToRoleAsync(user, "Admin").Wait();
                    }
                }

                //User:Leon
                if (userManager.FindByEmailAsync("lkammegnekamdem1@ncstudents.niagaracollege.ca").Result == null)
                {
                    IdentityUser user = new IdentityUser
                    {
                        UserName = "lkammegnekamdem1@ncstudents.niagaracollege.ca",
                        Email = "lkammegnekamdem1@ncstudents.niagaracollege.ca",
                        EmailConfirmed = true
                    };

                    IdentityResult result = userManager.CreateAsync(user, "Wmba@team6").Result;

                    if (result.Succeeded)
                    {
                        userManager.AddToRoleAsync(user, "Admin").Wait();
                    }
                }

                //User: Prof.David Stovell
                if (userManager.FindByEmailAsync("dstovell@niagaracollege.ca").Result == null)
                {
                    IdentityUser user = new IdentityUser
                    {
                        UserName = "dstovell@niagaracollege.ca",
                        Email = "dstovell@niagaracollege.ca",
                        EmailConfirmed = true
                    };

                    IdentityResult result = userManager.CreateAsync(user, "Wmba@team6").Result;

                    if (result.Succeeded)
                    {
                        userManager.AddToRoleAsync(user, "Admin").Wait();
                    }
                }

                //User: Prof.Mark Hardwick
                if (userManager.FindByEmailAsync("mhardwick@niagaracollege.ca").Result == null)
                {
                    IdentityUser user = new IdentityUser
                    {
                        UserName = "mhardwick@niagaracollege.ca",
                        Email = "mhardwick@niagaracollege.ca",
                        EmailConfirmed = true
                    };

                    IdentityResult result = userManager.CreateAsync(user, "Wmba@team6").Result;

                    if (result.Succeeded)
                    {
                        userManager.AddToRoleAsync(user, "Admin").Wait();
                    }
                }

                //User: Prof Dave Kendall
                if (userManager.FindByEmailAsync("dkendell@niagaracollege.ca").Result == null)
                {
                    IdentityUser user = new IdentityUser
                    {
                        UserName = "dkendell@niagaracollege.ca",
                        Email = "dkendell@niagaracollege.ca",
                        EmailConfirmed = true
                    };

                    IdentityResult result = userManager.CreateAsync(user, "Wmba@team6").Result;

                    if (result.Succeeded)
                    {
                        userManager.AddToRoleAsync(user, "Admin").Wait();
                    }
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.GetBaseException().Message);
            }
        }
    }
}
