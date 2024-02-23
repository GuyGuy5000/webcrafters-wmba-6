using wmbaApp.Data;
using wmbaApp.Models;

namespace wmbaApp.Utilities
{
    /// <summary>
    /// A class for creating a matchup with both teams, based on a GameTeam object
    /// - Nadav Hilu
    /// </summary>
    public class GameMatchup
    {
        private readonly WmbaContext _context;
        public Game game;
        public Team teamOne;
        public Team teamTwo;
        public string teamOneLineUp;
        public string teamTwoLineUp;
        public int? teamOneScore;
        public int? teamTwoScore;


        public string FullVersus
        {
            get
            {
                if (!String.IsNullOrEmpty(this?.teamOne.TmName) && !String.IsNullOrEmpty(this?.teamTwo.TmName))
                    return $"{teamOne.TmName} VS. {teamTwo.TmName}";
                else if (!String.IsNullOrEmpty(this?.teamOne.TmName))
                    return $"{teamOne.TmName} VS.TBA";
                else
                    return $"TBA VS. {teamTwo.TmName}";
            }
        }

        public string Score
        {
            get => $"{teamOne.TmName}: {teamOneScore} - {teamTwo.TmName}: {teamTwoScore}";
        }

        public GameMatchup(WmbaContext context, GameTeam gameTeam)
        {
            if (gameTeam == null)
                throw new Exception("A GameTeam must be provided");

            _context = context;
            int gameID = gameTeam.GameID; //gameID
            int teamID = gameTeam.TeamID; //teamID
            GameTeam[] gameTeams = _context.GameTeams.Where(gt => gt.GameID == gameID).ToArray(); //Get both records that match the gameID


            //if both records were found that means there are two teams assigned to one game
            if (gameTeams.Length == 2)
            {
                //get both team's info using gameTeam.teamID
                game = _context.Games.FirstOrDefault(g => g.ID == gameID);
                teamOne = _context.Teams.FirstOrDefault(t => t.ID == gameTeams[0].TeamID);
                teamTwo = _context.Teams.FirstOrDefault(t => t.ID == gameTeams[1].TeamID);
                teamOneLineUp = gameTeams[0].GmtmLineup;
                teamTwoLineUp = gameTeams[1].GmtmLineup;
                teamOneScore = gameTeams[0].GmtmScore;
                teamTwoScore = gameTeams[1].GmtmScore;
            }
            else
                throw new Exception("Could not find a matchup using the given gameID");
        }

        public Team[] GetTeams()
            => new Team[] { teamOne, teamTwo };

        /// <summary>
        /// gets the most recent matchups based on a teams array.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="teams"></param>
        /// <returns></returns>
        public static List<GameMatchup> GetMatchups(WmbaContext context, Team[] teams)
        {
            List<GameMatchup> gameMatchups = new();
            foreach (Team team in teams) //each team has a list of games
                if (team.GameTeams.Count > 0) //if a team has games:
                {
                    GameMatchup gameMatchup = new(context, team.GameTeams.Last()); //gets the most recent gameTeam record
                    gameMatchups.Add(gameMatchup);
                }

            return gameMatchups;
        }

        /// <summary>
        /// Updates a game with the given team IDs
        /// </summary>
        /// <param name="gameToUpdate"></param>
        /// <param name="HomeTeamID"></param>
        /// <param name="AwayTeamID"></param>
        public void UpdateGameTeams(int? HomeTeamID, int? AwayTeamID)
        {
            foreach (Game game in _context.Games) //looks through games in the db
                if (game.ID == this.game.ID) //finds the correct record
                {
                    if (HomeTeamID.HasValue) //if a home team was provided
                    {
                        if (HomeTeamID != this.teamOne?.ID) //if the ID provided is different to the current ID
                        {
                            //add new team as a GameTeam record
                            this.game.GameTeams.Add(
                                new GameTeam
                                {
                                    GameID = this.game.ID,
                                    TeamID = HomeTeamID.Value,
                                });

                            //find and remove old team
                            GameTeam teamToRemove = this.game.GameTeams.FirstOrDefault(gt => gt.TeamID == teamOne?.ID);
                            if (teamToRemove != null)
                                _context.Remove(teamToRemove);
                            this.teamOne = _context.Teams.FirstOrDefault(t => t.ID == HomeTeamID);
                        }
                        if (AwayTeamID != this.teamTwo?.ID) //if the ID provided is different to the current ID
                        {
                            //add new team as a GameTeam record
                            this.game.GameTeams.Add(
                                new GameTeam
                                {
                                    GameID = this.game.ID,
                                    TeamID = AwayTeamID.Value,
                                });

                            //find and remove old team
                            GameTeam teamToRemove = this.game.GameTeams.FirstOrDefault(gt => gt.TeamID == teamTwo.ID);
                            if (teamToRemove != null)
                                _context.Remove(teamToRemove);
                            this.teamOne = _context.Teams.FirstOrDefault(t => t.ID == HomeTeamID);
                        }
                    }
                }
        }
    }
}
