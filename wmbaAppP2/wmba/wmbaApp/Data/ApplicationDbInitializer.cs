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
                //context.Database.EnsureDeleted();
                context.Database.Migrate();

                //Creating Roles
                var RoleManager = applicationBuilder.ApplicationServices.CreateScope()
                    .ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
                //RookieConvenor = RookieC, IntermediateC = IntermediateConvenor
                string[] roleNames = { "Admin", "Coach", "ScoreKeeper", "Convenor", "Rookie Convenor", "Intermediate Convenor" };

                IdentityResult roleResult;
                foreach (var roleName in roleNames)
                {
                    var roleExist = await RoleManager.RoleExistsAsync(roleName);
                    if (!roleExist)
                    {
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

                //User:Coach
                if (userManager.FindByEmailAsync("coach@outlook.com").Result == null)
                {
                    IdentityUser user = new IdentityUser
                    {
                        UserName = "coach@outlook.com",
                        Email = "coach@outlook.com",
                        EmailConfirmed = true
                    };

                    IdentityResult result = userManager.CreateAsync(user, "Wmba@team6").Result;

                    if (result.Succeeded)
                    {
                        userManager.AddToRoleAsync(user, "Coach").Wait();
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

                //User:ScoreKeeper
                if (userManager.FindByEmailAsync("scorekeeper@outlook.com").Result == null)
                {
                    IdentityUser user = new IdentityUser
                    {
                        UserName = "scorekeeper@outlook.com",
                        Email = "scorekeeper@outlook.com",
                        EmailConfirmed = true
                    };

                    IdentityResult result = userManager.CreateAsync(user, "Wmba@team6").Result;

                    if (result.Succeeded)
                    {
                        userManager.AddToRoleAsync(user, "ScoreKeeper").Wait();
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
                        userManager.AddToRoleAsync(user, "Rookie Convenor").Wait();
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
                        userManager.AddToRoleAsync(user, "Intermediate Convenor").Wait();
                    }
                }

                //User:Convenor
                if (userManager.FindByEmailAsync("convenor@outlook.com").Result == null)
                {
                    IdentityUser user = new IdentityUser
                    {
                        UserName = "convenor@outlook.com",
                        Email = "convenor@outlook.com",
                        EmailConfirmed = true
                    };

                    IdentityResult result = userManager.CreateAsync(user, "Wmba@team6").Result;

                    if (result.Succeeded)
                    {
                        userManager.AddToRoleAsync(user, "Convenor").Wait();
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
