using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using System.Drawing.Drawing2D;
using wmbaApp.Models;

namespace wmbaApp.Data
{
    /// <summary>
    /// A class for initializing the database and populating it with some data
    /// - Nadav Hilu, Esmael Sandoqa, Farooq Jidelola
    /// </summary>
    public static class WmbaInitializer
    {
        public static async void Seed(IApplicationBuilder applicationBuilder)
        {
            WmbaContext context = applicationBuilder.ApplicationServices.CreateScope()
                .ServiceProvider.GetRequiredService<WmbaContext>();

            try
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
               // context.Database.Migrate();
                Random random = new();

                //Division seed data
                string[] divisions = { "9U", "11U", "13U", "15U", "18U" };
                //add divisions if none exist
                if (!context.Divisions.Any())
                {
                    foreach (string division in divisions)
                    {
                        context.Divisions.Add(new Division { DivName = division });
                    }
                    context.SaveChanges();
                }

                //Teams seed data
                string[] teams = { "Bananas", "Dragons", "Firefrogs", "Rockhounds", "Tin Caps", "Bisons", "Bats", "Mudhens", "Iron Birds", "Green Jackets", "Anchors", "Aces", "Bees", "Blue Rocks", "Mariners", "Woodpeckers", "Rockies", "Pirates", "Green Jr Jackfish", "Navy Mustangs" };
                //add teams if none exist
                if (!context.Teams.Any())
                {
                    int teamsRemaining = teams.Length;
                    foreach (Division d in context.Divisions.ToList())
                    {
                        //if there are at least 4 more teams to add, add a range of 4 teams.
                        if (teamsRemaining >= 4)
                            context.Teams.AddRange(
                                new Team
                                {
                                    TmName = $"{teams[teamsRemaining - 1]}",
                                    DivisionID = d.ID
                                },
                                new Team
                                {
                                    TmName = $"{teams[teamsRemaining - 2]}",
                                    DivisionID = d.ID
                                },
                                new Team
                                {
                                    TmName = $"{teams[teamsRemaining - 3]}",
                                    DivisionID = d.ID
                                },
                                new Team
                                {
                                    TmName = $"{teams[teamsRemaining - 4]}",
                                    DivisionID = d.ID
                                });
                        teamsRemaining -= 4;
                    }
                    context.SaveChanges();
                }

                //Coach seed data
                string[] firstNames = new string[] { "Lyric", "Antoinette", "Kendal", "Vivian", "Ruth", "Jamison", "Emilia", "Natalee", "Yadiel", "Jakayla", "Lukas", "Moses", "Kyler", "Karla", "Chanel", "Tyler", "Camilla", "Quintin", "Braden", "Clarence", "Anthony", "Bruce", "Murphy", "Rosa", "John", "Leia", "Kylie", "Derek", "Luna", "Daisy", "Kody", "Blaine", "Jonas", "Lennon", "Marlee", "Chandler", "Louisa", "Harris", "Lia", "Byron", "Erin", "Maia", "Clark", "Kelly", "Robin", "Julio", "Abraham" };
                string[] lastNames = new string[] { "Watts", "Randall", "Arias", "Weber", "Stone", "Carlson", "Robles", "Frederick", "Parker", "Morris", "Soto", "Orozco", "Boyer", "Burns", "Cobb", "Blankenship", "Houston", "Estes", "Atkins", "Miranda", "Zuniga", "Ward", "Mayo", "Costa", "Reeves", "Cook", "Krueger", "Crane", "Watts", "Little", "Henderson", "Bishop" };
                //add coaches if none exist
                if (!context.Coaches.Any())
                {
                    int teamsCount = context.Teams.Count();
                    for (int i = 0; i <= teamsCount; i++)
                    {
                        context.Coaches.Add(
                            new Coach
                            {
                                CoachFirstName = firstNames[i],
                                CoachLastName = lastNames[i],
                                CoachEmail = $"{firstNames[i]}{lastNames[i]}@outlook.com"
                            });
                    }
                    context.SaveChanges();
                }

                //DivisionCoach seed data
                if (!context.DivisionCoaches.Any())
                {
                    Coach[] coachesArray = context.Coaches.ToArray();
                    Division[] divisionsArray = context.Divisions.ToArray();
                    int coachCounter = 0;

                    foreach (Division division in divisionsArray)
                    {
                        Team[] teamsArray = context.Teams.Where(t => t.DivisionID == division.ID).ToArray(); //get teams from selected division
                        foreach (Team team in teamsArray)
                        {
                            context.DivisionCoaches.Add(
                                 new DivisionCoach
                                 {
                                     CoachID = coachesArray[coachCounter].ID,
                                     DivisionID = division.ID,
                                     TeamID = team.ID
                                 });
                            coachCounter++;
                        }
                    }
                    context.SaveChanges();
                }

                //Player seed data
                //add players if none exist
                if (!context.Players.Any())
                {
                    //Create 9-13 players for each team
                    foreach (Team team in context.Teams.ToList())
                        for (int i = 0; i < random.Next(9, 14); i++)
                        {
                            context.Players.Add(
                                new Player
                                {
                                    PlyrFirstName = firstNames[random.Next(0, firstNames.Length - 1)],
                                    PlyrLastName = lastNames[random.Next(0, lastNames.Length - 1)],
                                    PlyrJerseyNumber = (random.Next(2) == 1) ? i + 1 : null,
                                    PlyrMemberID = new string(Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789", 8).Select(s => s[random.Next(s.Length)]).ToArray()),
                                    TeamID = team.ID
                                });
                        }
                    context.SaveChanges();
                }

                //Statistics seed data
                if (!context.Statistics.Any())
                {
                    foreach (Player player in context.Players.ToList())
                    {
                        int PA;
                        int AB;
                        context.Statistics.Add(
                            new Statistic
                            {
                                StatsGP = random.Next(80, 162),  // Assuming a baseball season with 162 games
                                StatsPA = PA = random.Next(500, 700), // A random range for plate appearances
                                StatsAB = AB = random.Next(PA-220, 520),  // A random range for at-bats
                                                                //next double creates a random number between 0.0 and 1.0
                                StatsAVG = Math.Round(random.NextDouble(), 3),  // Random AVG between 0 and 1 with 3 decimal places
                                StatsOBP = Math.Round(random.NextDouble(), 3),  // Random OBP between 0 and 1 with 3 decimal places
                                StatsOPS = Math.Round(random.NextDouble(), 3),  // Random OPS between 0 and 1 with 3 decimal places
                                StatsSLG = Math.Round(random.NextDouble(), 3),  // Random SLG between 0 and 1 with 3 decimal places
                                StatsH = random.Next(100, 200),
                                StatsR = random.Next(60, 100),
                                StatsK = random.Next(100, 200),
                                StatsHR = random.Next(0, 30),
                                StatsRBI = random.Next(30, 120),
                                StatsBB = random.Next(10, 50)
                            });
                        context.SaveChanges();
                        player.StatisticID = context.Statistics.Count();
                    }
                    context.SaveChanges();
                }

                //Position seed data
                string[] positions = { "Pitcher", "Infielder", "Shortstop", "First baseman", "Second baseman", "Third baseman", "Catcher", "Outfielder", "Left fielder", "Right fielder", "Center fielder", "Designated hitter", "Relief pitcher", "Pinch hitter", "Pinch runner" };
                //add positions if none exist
                //if (!context.Positions.Any())
                //{
                //    foreach (string p in positions)
                //    {
                //        context.Positions.Add(new Models.Position { PosName = p });
                //    }
                //    context.SaveChanges();
                //}

                //player action seed data
                string[] actions = { "Single", "Double", "Triple", "Home Run", "Ball", "Strike", "Foul Ball", "Foul Tip", "Fly Out", "Ground Out", "Hit By Pitch", "Intentional Walk", "Catcher Interference",
                    "Run" ,"First Base", "Second Base", "Third Base", "Out", "Walk"};
                if (!context.PlayerActions.Any())
                {
                    foreach (string actionName in actions)
                    {
                        context.PlayerActions.Add(new PlayerAction { PlayerActionName = actionName });
                    }
                    context.SaveChanges();
                }

                // Game Location seed data
                string[] locations = { "Chippawa Park Ball Diamond", "Maple Park Diamond 1", "Plymouth Park Ball Diamond", "PCMBA Rotary Complex", "Memorial Park Diamond 1", "Memorial Park Diamond 2", "Memorial Park Diamond 3", "Memorial Park Diamond 4", "Port Robinson Park Ball Diamond", "Treelawn Park Ball Diamond", "Southward Community Park Diamond 4" };

                // Check if GameLocations exist, if not, add them
                if (!context.GameLocations.Any())
                {
                    foreach (string locationName in locations)
                    {
                        context.GameLocations.Add(new GameLocation { Name = locationName });
                    }
                    context.SaveChanges();
                }

                //Game Seed data
                // Add games if none exist
                if (!context.Games.Any())
                {
                    List<Team> teamsList = context.Teams.ToList();
                    Dictionary<int, DateTime?> lastScheduledGameTimes = new Dictionary<int, DateTime?>();
                    HashSet<(int, int)> createdGamePairs = new HashSet<(int, int)>();

                    foreach (Team homeTeam in teamsList)
                    {
                        IEnumerable<Team> remainingTeamsInSameDivision = teamsList
                            .Where(t => t.ID != homeTeam.ID && t.DivisionID == homeTeam.DivisionID);

                        foreach (Team awayTeam in remainingTeamsInSameDivision)
                        {
                            // Check if the reverse game pair has already been created
                            if (!createdGamePairs.Contains((homeTeam.ID, awayTeam.ID)))
                            {
                                DateTime start = DateTime.Now.AddMinutes(-DateTime.Now.Minute).AddSeconds(-DateTime.Now.Second);

                                // checking to see the last game
                                DateTime? lastScheduledGameTime;
                                if (lastScheduledGameTimes.TryGetValue(homeTeam.ID, out lastScheduledGameTime) && lastScheduledGameTime.HasValue)
                                {
                                    // I'm making 2 days diff btw games
                                    start = lastScheduledGameTime.Value.AddHours(random.Next(48, 72));
                                }

                                // Get a random location from the array
                                string randomLocation = locations[random.Next(locations.Length)];

                                // Find the GameLocation by name
                                var gameLocation = context.GameLocations.FirstOrDefault(gl => gl.Name == randomLocation);

                                context.Games.Add(new Game
                                {
                                    AwayTeam = awayTeam,
                                    HomeTeam = homeTeam,
                                    GameStartTime = start,
                                    GameEndTime = start.AddMinutes(random.Next(120, 150)),
                                    GameLocationID = gameLocation?.ID ?? default,
                                    DivisionID = homeTeam.DivisionID,
                                    HomeLineup = new Lineup(),
                                    AwayLineup = new Lineup()
                                });

                                context.SaveChanges();

                                Game createdGame = context.Games.OrderBy(g => g.ID).Last();

                                // Add the players from the teams to each lineup
                                foreach (Player p in homeTeam.Players.Where(p => p.IsActive == true))
                                {
                                    if (createdGame.HomeLineup.PlayerLineups.Count == 9)
                                        break;
                                    createdGame.HomeLineup.PlayerLineups.Add(new PlayerLineup()
                                    {
                                        PlayerID = p.ID,
                                        LineupID = createdGame.HomeLineup.ID
                                    });
                                }

                                foreach (Player p in awayTeam.Players.Where(p => p.IsActive == true))
                                {
                                    if (createdGame.AwayLineup.PlayerLineups.Count == 9)
                                        break;
                                    createdGame.AwayLineup.PlayerLineups.Add(new PlayerLineup()
                                    {
                                        PlayerID = p.ID,
                                        LineupID = createdGame.AwayLineup.ID
                                    });
                                }
                                homeTeam.HomeGames.Add(createdGame);
                                awayTeam.AwayGames.Add(createdGame);
                                context.Teams.Update(homeTeam);
                                context.Teams.Update(awayTeam);
                                context.Games.Update(createdGame);
                                // Save the changes to the database
                                context.SaveChanges();

                                // Confirm the At and HT have changed roles
                                createdGamePairs.Add((awayTeam.ID, homeTeam.ID));

                                // Update the last scheduled game time for the home team
                                lastScheduledGameTimes[homeTeam.ID] = start;
                            }
                        }
                    }
                }

                //GameTeam seed data
                if (!context.GameTeams.Any())
                {
                    int teamID = context.Teams.FirstOrDefault().ID;

                    foreach (Game game in context.Games.ToList())
                    {
                        if (context.Teams.FirstOrDefault(t => t.ID == teamID) == null) //check to see that teamID exists in the database
                            teamID -= 2;

                        context.GameTeams.Add(
                            new GameTeam
                            {
                                GameID = game.ID,
                                TeamID = context.Teams.FirstOrDefault(t => t.ID == teamID).ID,
                                GmtmLineup = "TBA"
                            });

                        if (context.Teams.FirstOrDefault(t => t.ID == teamID).ID == context.Teams.Count()) //check to see that teamID isn't last in the database
                            teamID -= 2;

                        context.GameTeams.Add(
                            new GameTeam
                            {
                                GameID = game.ID,
                                TeamID = context.Teams.FirstOrDefault(t => t.ID == teamID + 1).ID,
                                GmtmLineup = "TBA"
                            });
                        teamID++;
                    }
                    context.SaveChanges();
                }



                //Roles and User Seed
                ApplicationDbContext appContext = applicationBuilder.ApplicationServices.CreateScope()
                    .ServiceProvider.GetRequiredService<ApplicationDbContext>();

                appContext.Database.Migrate();


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

                //Coach and Scorekeeper roles seed data
                if (appContext.Roles.Count() <= 9)
                {
                    foreach (Team team in context.Teams)
                    {
                        var coachRoleExist = await RoleManager.RoleExistsAsync(team.TmName + " Coach");
                        var scorekeeperRoleExist = await RoleManager.RoleExistsAsync(team.TmName + " Coach");
                        if (!coachRoleExist)
                            roleResult = await RoleManager.CreateAsync(new ApplicationRole(team.TmName, 0, team.ID));
                    }
                }


                //Coach and scorekeeper user seed data
                if (!appContext.Users.Any())
                {
                    var userManager = applicationBuilder.ApplicationServices.CreateScope()
                        .ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

                    foreach (Coach coach in context.Coaches)
                    {
                        if (userManager.FindByEmailAsync(coach.CoachEmail).Result == null)
                        {
                            IdentityUser user = new IdentityUser
                            {
                                UserName = $"{coach.CoachEmail}",
                                Email = $"{coach.CoachEmail}",
                                EmailConfirmed = true
                            };

                            IdentityResult result = userManager.CreateAsync(user, "Wmba@team6").Result;

                            if (result.Succeeded)
                            {
                                int? teamID = coach.DivisionCoaches.FirstOrDefault()?.TeamID;
                                if (teamID.HasValue)
                                    userManager.AddToRoleAsync(user, appContext.Roles.FirstOrDefault(r => r.TeamID == teamID).Name).Wait();
                                userManager.AddToRoleAsync(user, "Coach").Wait();
                            }
                        }
                    }

                    foreach (Team team in context.Teams)
                    {
                        if (userManager.FindByEmailAsync($"scorekeeper{team.ID}outlook.com").Result == null)
                        {
                            IdentityUser user = new IdentityUser
                            {
                                UserName = $"scorekeeper{team.ID}@outlook.com",
                                Email = $"scorekeeper{team.ID}@outlook.com",
                                EmailConfirmed = true
                            };

                            IdentityResult result = userManager.CreateAsync(user, "Wmba@team6").Result;

                            if (result.Succeeded)
                            {
                                userManager.AddToRoleAsync(user, appContext.Roles.FirstOrDefault(r => r.TeamID == team.ID).Name).Wait();
                                userManager.AddToRoleAsync(user, "Scorekeeper").Wait();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}
