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
        public static void Seed(IApplicationBuilder applicationBuilder)
        {
            WmbaContext context = applicationBuilder.ApplicationServices.CreateScope()
                .ServiceProvider.GetRequiredService<WmbaContext>();

            try
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
                //context.Database.Migrate();
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
                                CoachLastName = lastNames[i]
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
                        context.Statistics.Add(
                            new Statistic
                            {
                                StatsGP = random.Next(1, 162),  // Assuming a baseball season with 162 games
                                StatsPA = random.Next(1, 600), // A random range for plate appearances
                                StatsAB = random.Next(1, 500),  // A random range for at-bats
                                                                //next double creates a random number between 0.0 and 1.0
                                StatsAVG = Math.Round(random.NextDouble(), 3),  // Random AVG between 0 and 1 with 3 decimal places
                                StatsOBP = Math.Round(random.NextDouble(), 3),  // Random OBP between 0 and 1 with 3 decimal places
                                StatsOPS = Math.Round(random.NextDouble(), 3),  // Random OPS between 0 and 1 with 3 decimal places
                                StatsSLG = Math.Round(random.NextDouble(), 3),  // Random SLG between 0 and 1 with 3 decimal places
                                StatsH = random.Next(0, 200),
                                StatsR = random.Next(0, 100),
                                StatsK = random.Next(0, 200),
                                StatsHR = random.Next(0, 50),
                                StatsRBI = random.Next(0, 1000),
                                StatsBB = random.Next(0, 50)
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
                string[] actions = { "Single", "Double", "Triple", "Home Run", "Ball", "Strike", "Foul Ball", "Foul Tip", "Hit By Pitch", "Intentional Walk", "Catcher Interference",
                    "Home Base" ,"First Base", "Second Base", "Third Base", "Out", "Walk"};
                if (!context.PlayerActions.Any())
                {
                    foreach (string actionName in actions)
                    {
                        context.PlayerActions.Add(new PlayerAction { PlayerActionName = actionName });
                    }
                    context.SaveChanges();
                }

                // Game seed data
                string[] locations = { "Chippawa Park Ball Diamond", "Maple Park Diamond 1", "Plymouth Park Ball Diamond", "PCMBA Rotary Complex", "Memorial Park Diamond 1", "Memorial Park Diamond 2", "Memorial Park Diamond 3", "Memorial Park Diamond 4", "Port Robinson Park Ball Diamond", "Treelawn Park Ball Diamond", "Southward Community Park Diamond 4" };

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
                                DateTime start = DateTime.Now;

                                // checking to see the last game
                                DateTime? lastScheduledGameTime;
                                if (lastScheduledGameTimes.TryGetValue(homeTeam.ID, out lastScheduledGameTime) && lastScheduledGameTime.HasValue)
                                {
                                    // I'm making 2 days diff btw games
                                    start = lastScheduledGameTime.Value.AddHours(random.Next(48, 72));
                                }

                                context.Games.Add(new Game
                                {
                                    AwayTeam = awayTeam,
                                    HomeTeam = homeTeam,
                                    GameStartTime = start,
                                    GameEndTime = start.AddMinutes(random.Next(120, 150)),
                                    GameLocation = locations[random.Next(locations.Length - 1)],
                                    DivisionID = homeTeam.DivisionID,
                                    HomeLineup = new Lineup(),
                                    AwayLineup = new Lineup()
                                });

                                context.SaveChanges();

                                Game createdGame = context.Games.OrderBy(g => g.ID).Last();

                                // Add the players from the teams to each lineup
                                foreach (Player p in homeTeam.Players)
                                {
                                    if (createdGame.HomeLineup.PlayerLineups.Count == 9)
                                        break;
                                    createdGame.HomeLineup.PlayerLineups.Add(new PlayerLineup()
                                    {
                                        PlayerID = p.ID,
                                        LineupID = createdGame.HomeLineup.ID
                                    });
                                }

                                foreach (Player p in awayTeam.Players)
                                {
                                    if (createdGame.AwayLineup.PlayerLineups.Count == 9)
                                        break;
                                    createdGame.AwayLineup.PlayerLineups.Add(new PlayerLineup()
                                    {
                                        PlayerID = p.ID,
                                        LineupID = createdGame.AwayLineup.ID
                                    });
                                }

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
                                TeamID = context.Teams.FirstOrDefault(t => t.ID == teamID).ID + 1,
                                GmtmLineup = "TBA"
                            });
                        teamID++;
                    }
                    context.SaveChanges();
                }
            }
            catch (Exception)
            {

            }
        }
    }
}
